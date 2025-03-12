using UnityEngine;
using System.Collections;

public class ShooterEnemyAI : MonoBehaviour
{
    [Header("VALUES")]
    [SerializeField] public float speed = 2f;
    [SerializeField] public float agroRange = 5f;
    [SerializeField] public float wanderRange = 3f;
    [SerializeField] public float chaseSpeedMultiplier = 2f;
    [SerializeField] public float timeToEndChase = 2f;   //DEÐÝÞÝRSE !!! FADE DEÐÝÞÝR.
    [SerializeField] public int rayCount = 15;
    [SerializeField] public float coneAngle = 75f;


    [Header("SHOOTING")]
    [SerializeField] public float attackDistance = 3f;
    [SerializeField] public GameObject arrowPrefab;
    [SerializeField] public Transform shootPoint;
    [SerializeField] public float fireRate = 1f;
    [SerializeField] public float firstShotDelay = 1f;


    private float nextShotTime = 0f;

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
    private bool hasDelayedFirstShot = false;

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
        direction = (target.position - transform.position).normalized;
        SetDirection(direction);
        float distance = Vector2.Distance(transform.position, target.position);
        float stopDistance = 0.3f; // The deadzone range

        // For chasing, stop moving if within attackDistance.
        if (isChasing)
        {
            if (distance > attackDistance)
            {
                direction = (target.position - transform.position).normalized;
                rb.linearVelocity = direction * speed;
                animator.SetBool("isMoving", true);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                animator.SetBool("isMoving", false);
            }
        }
        else if (isWandering)
        {
            if (distance > stopDistance)
            {
                direction = (target.position - transform.position).normalized;
                rb.linearVelocity = direction * speed;
                animator.SetBool("isMoving", true);
            }
            else
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
            ChasePlayerr();
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


    public void ChasePlayerr()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > attackDistance)
            {
                target.position = player.position;
                transform.position += (Vector3)direction * speed * Time.deltaTime;
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);

                // Delay first shot ONLY if this is the first time chasing
                if (!hasDelayedFirstShot)
                {
                    hasDelayedFirstShot = true;
                    StartCoroutine(DelayedFirstShot());
                }
                else if (Time.time > nextShotTime)
                {
                    ShootArrow();
                    nextShotTime = Time.time + fireRate;
                }
            }

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

            Exclamation.GetComponent<Animator>().SetBool("fading", false);
        }
    }

    // Coroutine to add delay before the first shot
    private IEnumerator DelayedFirstShot()
    {
        yield return new WaitForSeconds(firstShotDelay); // Adjust delay as needed
        if (isChasing) // Ensure enemy is still chasing before firing
        {
            ShootArrow();
            nextShotTime = Time.time + fireRate; // Start normal shooting cooldown
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
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > attackDistance)
            {
                target.position = player.position;
                transform.position += (Vector3)direction * speed * Time.deltaTime;
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
                if (Time.time > nextShotTime)
                {
                    ShootArrow();
                    nextShotTime = Time.time + fireRate;
                }
            }
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
        hasDelayedFirstShot = false;

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

    // NEW: Function to shoot an arrow toward the player.
    void ShootArrow()
    {
        if (arrowPrefab != null && shootPoint != null && player != null)
        {
            Vector2 shootDirection = (player.position - shootPoint.position).normalized;
            SetDirection(shootDirection);
            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
            var go = Instantiate(arrowPrefab, shootPoint.position, Quaternion.AngleAxis(angle, Vector3.forward));
            go.GetComponent<EnemyArrow>().parent = this.gameObject;
            entitySFX.BowSFX();
        }
    }

    public void NoticePlayer()
    {
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        StopWandering();
        target.position = player.position;
        SetDirection(direction);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.1f);
        ChasePlayerr();
    }
}
