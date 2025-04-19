using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using static Unity.VisualScripting.Member;

public class DasherEnemyAI : MonoBehaviour
{
    [Header("VALUES")]
    [SerializeField] public float speed = 2f;
    [SerializeField] public float agroRange = 5f;
    [SerializeField] public float wanderRange = 3f;
    [SerializeField] public float timeToEndChase = 2f;   //DEĞİŞİRSE !!! FADE DEĞİŞİR.
    [SerializeField] public int rayCount = 15;
    [SerializeField] public float coneAngle = 75f;
    [Header("DASH")]
    [SerializeField] public float chargeTime = 0.5f;
    [SerializeField] public float dashSpeedMultiplier = 20f;
    [SerializeField] public float stunAfterDash = 1f;
    [SerializeField] public float stunAfterHitMultiplier = 2f;
    [SerializeField] public float dashDistance = 5f;
    [SerializeField] public float dashTime = 2f;
    [SerializeField] public float dashDamageMultiplier = 2f;




    [Header("ASSIGN")]
    [SerializeField] public GameObject Exclamation;
    [SerializeField] public GameObject Stun;
    [SerializeField] public GameObject DashIndicator;

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
    private bool dashStarted;
    private bool chargingDash;
    public bool isDashing;
    private float baseStunAfterDash;
    private bool go;
    public Coroutine dashCoroutine;
    private Vector2 dashDirection;
    private float dashSpeed;
    private bool canSetTarget;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        entitySFX = GetComponent<EntitySFX>();
        DashIndicator.SetActive(false);

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
        baseStunAfterDash = stunAfterDash;
        chaseTimer = null;


        health.healthSlider.enabled = false;
        health.healthSlider.gameObject.SetActive(false);
        Stun.SetActive(false);
    }

    void Update()
    {
        if (GetComponent<Health>().dead == false)
        {
            direction = (target.position - transform.position).normalized;
            SetDirection(direction);
            float distance = Vector2.Distance(transform.position, target.position);
            float stopDistance = 0.3f; // The deadzone range

            if (isWandering)
            {
                if (distance > stopDistance) // Only update movement if outside the deadzone
                {
                    rb.linearVelocity = direction * speed;
                    animator.SetBool("isMoving", true);
                }
            }
            if (rb.linearVelocity == Vector2.zero)
            {
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

            if (isChasing)
            {

                if (!health.healthSlider.enabled)
                {
                    health.healthSlider.enabled = true;
                    health.healthSlider.gameObject.SetActive(true);
                }

                Exclamation.SetActive(true);
            }
            else
            {
                Exclamation.SetActive(false);
            }


            if (canSetTarget)
            {
                target.position = player.position; // Lock target position
            }

            Vector2 directionToTarget = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            DashIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);


        }

        else
        {
            StopAllCoroutines();
        }

    }

    void FixedUpdate()
    {
        if (go)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
        }
    }



    public void ChasePlayer()
    {
        if (player != null && !dashStarted)
        {
            isChasing = true;
            isWandering = false;
            StopWandering();


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

             dashCoroutine = StartCoroutine(DashTowardsPlayer());
             dashStarted = true;
            
        }
    }

    IEnumerator DashTowardsPlayer()
    {
        chargingDash = true;
        stunAfterDash = baseStunAfterDash;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        StartCoroutine(SetTarget());


        yield return new WaitForSeconds(chargeTime); // Charging delay
        DashIndicator.GetComponent<Animator>().SetTrigger("stop");
        DashIndicator.SetActive(false);

        isDashing = true;
        go = false;
        dashSpeed = baseSpeed * dashSpeedMultiplier;
        float elapsedTime = 0;
        dashDirection = (target.position - transform.position).normalized;

        if (dashDirection == Vector2.zero)
        {
            dashDirection = Vector2.right; // Fallback direction
        }

        Vector2 startPos = rb.position;

        while (elapsedTime < dashTime)
        {
            float distanceTraveled = Vector2.Distance(startPos, rb.position);
            if (distanceTraveled >= dashDistance) break;

            go = true;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StopDash();
        StartCoroutine(StunAfterDash());
    }

    private IEnumerator SetTarget()
    {
        DashIndicator.SetActive(true);
        DashIndicator.GetComponent<Animator>().SetTrigger("grow");
        canSetTarget = true;

        var time = chargeTime * 2 / 3;
        DashIndicator.GetComponent<Animator>().speed = 1/time;//ANİMASYON 1 SANİYE İKEN HIZI 1, BUNU AŞAĞIDA HESAPLANAN var time'A EŞİTLE.                                                    

        yield return new WaitForSeconds(time); //TIME KAÇA EŞİT İSE DASH INDİCATOR ANİMASYONU O KADAR OLMALI.

        DashIndicator.GetComponent<Animator>().speed = 1;
        DashIndicator.GetComponent<Animator>().SetTrigger("glow");
        canSetTarget = false;

    }

    public void StopDash()
    {
        StopCoroutine(dashCoroutine);
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        isDashing = false;
        isChasing = false;
        go = false;
    }
    public IEnumerator StunAfterDash()
    {
        yield return new WaitForSeconds(stunAfterDash);
        Stun.SetActive(false);
        StopChasingPlayer();
        dashStarted = false;
    }


    IEnumerator ExclamationDelay()
    {
        yield return new WaitForSeconds(0.018f);
        Exclamation.GetComponent<Animator>().SetTrigger("trigger");
        entitySFX.EnemyConfusedSFX();
    }

    void StopChasingPlayer()
    {
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
