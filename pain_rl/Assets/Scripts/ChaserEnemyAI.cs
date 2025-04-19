using UnityEngine;
using System.Collections;

public class ChaserEnemyAI : MonoBehaviour
{
    [Header("VALUES")]
    [SerializeField] public float speed = 2f;
    [SerializeField] public float agroRange = 5f;
    [SerializeField] public float wanderRange = 3f;
    [SerializeField] public float chaseSpeedMultiplier = 2f;
    [SerializeField] public float timeToEndChase = 2f;   //DEÐÝÞÝRSE !!! FADE DEÐÝÞÝR.
    [SerializeField] public int rayCount = 15;
    [SerializeField] public float coneAngle = 75f;


    [Header("ASSIGN")]
    [SerializeField] public GameObject Exclamation;

    [Header("DONT TOUCHH")]
    public bool isWandering = false;
    public bool isChasing = false;
    private float baseSpeed;
    private Transform player;
    private GameObject playerObj;
    private Animator animator;
    private int animDirection;
    private Rigidbody2D rb;
    private Vector2 direction;
    private Transform target;
    private Coroutine wanderCoroutine;
    private Coroutine chaseTimer;
    private Coroutine targetResetCoroutine;
    private bool hasPlayedAnim = false;
    private Health health;
    private EntitySFX entitySFX;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        entitySFX = GetComponent<EntitySFX>();

        playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (target == null)
        {
            GameObject targetObj = new GameObject("Target");
            target = targetObj.transform;

            // Find the "Targets" GameObject in the scene
            GameObject targetsParent = GameObject.Find("Targets");

            // If "Targets" exists, set it as the parent; otherwise, create it
            if (targetsParent == null)
            {
                targetsParent = new GameObject("Targets");
            }

            target.parent = targetsParent.transform;
        }


        StartWandering();
        baseSpeed = speed;
        chaseTimer = null;

        health.healthSlider.enabled = false;
        health.healthSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        if(GetComponent<Health>().dead == false)
        {
            direction = (target.position - transform.position).normalized;
            SetDirection(direction);
            float distance = Vector2.Distance(transform.position, target.position);
            float stopDistance = 0.3f; // The deadzone range

            if (isChasing || isWandering)
            {
                if (distance > stopDistance) // Only update movement if outside the deadzone
                {
                    rb.linearVelocity = direction * speed;
                    animator.SetBool("isMoving", true);
                }
                else // If within the deadzone, stop moving
                {
                    rb.linearVelocity = Vector2.zero;
                    animator.SetBool("isMoving", false);
                }
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                animator.SetBool("isMoving", false);
            }

            if (CanSeePlayer(agroRange))
            {
                if (isWandering)
                {
                    isWandering = false;
                    StopWandering();
                }
                ChasePlayer();
            }
            else
            {
                if (chaseTimer == null)
                {
                    chaseTimer = StartCoroutine(StopChasingTimer());
                }
                if (isChasing)
                {
                    ContinueChase();
                    Exclamation.GetComponent<Animator>().SetBool("fading", true);
                }
            }

            if (isChasing)
            {
                speed = baseSpeed * chaseSpeedMultiplier;

                if (!health.healthSlider.enabled)
                {
                    health.healthSlider.enabled = true;
                    health.healthSlider.gameObject.SetActive(true);
                }

                Exclamation.SetActive(true);
            }
            else
            {
                speed = baseSpeed;
                Exclamation.SetActive(false);
            }
        }


        else
        {
            StopAllCoroutines();
        }

    }



    public void ChasePlayer()
    {
        if (player != null)
        {
            target.position = player.position;
            transform.position += (Vector3)direction * speed * Time.deltaTime;
            animator.SetBool("isMoving", true);
            isChasing = true;

            if (chaseTimer != null)
            {
                StopCoroutine(chaseTimer);
                chaseTimer = null;
            }
            if (targetResetCoroutine != null)
            {
                StopCoroutine(targetResetCoroutine);
            }

            if (!hasPlayedAnim)
            {
                hasPlayedAnim = true;
                StartCoroutine(ExclamationDelay());
            }

            if (Exclamation != null)
            {
                Exclamation.GetComponent<Animator>().SetBool("fading", false);
            }
        }
    }

    IEnumerator ExclamationDelay()
    {
        yield return new WaitForSeconds(0.018f);
        Exclamation.GetComponent<Animator>().SetTrigger("trigger");
        entitySFX.EnemyConfusedSFX();
    }
    void ContinueChase()
    {
        if (player != null)
        {
            target.position = player.position;
            transform.position += (Vector3)direction * speed * Time.deltaTime;
            animator.SetBool("isMoving", true);
            isChasing = true;
        }

    }

    IEnumerator StopChasingTimer()
    {

        yield return new WaitForSeconds(timeToEndChase);
        StopChasingPlayer();
        Exclamation.GetComponent<Animator>().SetBool("fading", false);
    }

    void StopChasingPlayer()
    {
        isChasing = false;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        hasPlayedAnim = false;

        if (!isWandering)
        {
            StartWandering();
        }
    }

    bool CanSeePlayer(float distance)
    {
        bool val = false;
        float angleStep = coneAngle / (rayCount - 1);
        float startAngle = -coneAngle / 2;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = startAngle + (angleStep * i);
            Vector2 castDir = Quaternion.Euler(0, 0, angle) * direction;
            Vector2 endPos = (Vector2)transform.position + castDir * distance;

            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, castDir, distance);

            foreach (RaycastHit2D hit in hits)
            {
                // Ignore self by checking the tag
                if (hit.collider.CompareTag("Enemy")) continue;

                if (hit.collider.CompareTag("Barrel"))
                {
                    Debug.DrawLine(transform.position, hit.point, Color.blue);
                    val = false;
                    break;
                }
                else if (hit.collider.CompareTag("Player"))
                {
                    Debug.DrawLine(transform.position, hit.point, Color.yellow);
                    val = true;
                }
                else
                {
                    Debug.DrawLine(transform.position, endPos, Color.red);
                }
            }
        }

        return val;
    }

    void SetDirection(Vector2 direction)
    {
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            animDirection = (direction.x > 0) ? 3 : 1;
        }
        else
        {
            animDirection = (direction.y > 0) ? 0 : 2;
        }

        animator.SetInteger("Direction", animDirection);
    }

    void StartWandering()
    {
        isWandering = true;
        isChasing = false;
        wanderCoroutine = StartCoroutine(Wander());
    }

    void StopWandering()
    {
        if (wanderCoroutine != null)
        {
            StopCoroutine(wanderCoroutine);
            wanderCoroutine = null;
        }
    }

    IEnumerator Wander()
    {
        while (true)
        {
            Vector2 randomOffset = new Vector2(
                Random.Range(-wanderRange, wanderRange),
                Random.Range(-wanderRange, wanderRange)
            );
            target.position = (Vector2)transform.position + randomOffset;

            if (targetResetCoroutine != null)
            {
                StopCoroutine(targetResetCoroutine);
            }
            targetResetCoroutine = StartCoroutine(ResetTargetPosition());

            while (Vector2.Distance(transform.position, target.position) > 0.1f)
            {
                transform.position += (Vector3)direction * speed * Time.deltaTime;
                SetDirection(direction);
                animator.SetBool("isMoving", true);
                yield return null;
            }

            animator.SetBool("isMoving", false);
            yield return new WaitForSeconds(Random.Range(1f, 2f));
        }
    }

    IEnumerator ResetTargetPosition()
    {
        yield return new WaitForSeconds(5f);
        target.position = (Vector2)transform.position + new Vector2(
            Random.Range(-wanderRange, wanderRange),
            Random.Range(-wanderRange, wanderRange)
        );
        StopWandering();
        StartWandering();
    }







}
