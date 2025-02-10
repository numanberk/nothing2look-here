using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack Instance;

    [Header("SET THESE UP!")]
    [SerializeField] public Transform attackPoint;
    

    [Header("DONT TOUCH!")]
    [SerializeField] public int attackDamage;
    [SerializeField] public float attackRange;
    [SerializeField] public float attackAngle; // Angle of attack cone
    [SerializeField] public float attackCooldown;
    [SerializeField] public float animationLength;
    [SerializeField] public Transform attackObjectParent;
    [SerializeField] public Animator attackObjectAnimator;
    public bool isAttacking = false;
    public bool sword = false;




    private bool canAttack;
    private Animator anim;
    private PlayerMovement movementScript;
    private Sword swordScript;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        movementScript = GetComponent<PlayerMovement>();
        swordScript = GetComponentInChildren<Sword>();
        canAttack = true;
    }

    private void Update()
    {

        if(sword)
        {
            attackObjectParent.position = attackPoint.position;
            if (Input.GetMouseButtonDown(0))
            {
                if (canAttack)
                {
                    SwordAttack();
                }

            }
        }


    }


    private void SwordAttack()
    {
        anim.SetTrigger("Attack");

        canAttack = false;
        isAttacking = true;
        StartCoroutine(AttackReset());

        // Get mouse position relative to the player
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)attackPoint.position).normalized;

        float centerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float minAngle = centerAngle - (attackAngle / 2);
        float maxAngle = centerAngle + (attackAngle / 2);

        // Ensure attackInt stays in a cycle of 1 → 2 → 3 → 1...
        switch (swordScript.attackInt)
        {
            case 1:
                swordScript.attackInt = 2; // Move to the next attack
                attackObjectParent.rotation = Quaternion.Euler(0, 0, minAngle);
                attackObjectParent.DORotate(new Vector3(0, 0, maxAngle), animationLength)
                    .SetEase(Ease.OutExpo);
                if (attackObjectAnimator != null)
                {
                    attackObjectAnimator.SetTrigger("Swing1");
                }
                break;

            case 2:
                swordScript.attackInt = 1; // Move to the next attack
                attackObjectParent.rotation = Quaternion.Euler(0, 0, maxAngle);
                attackObjectParent.DORotate(new Vector3(0, 0, minAngle), animationLength)
                    .SetEase(Ease.OutExpo);
                if (attackObjectAnimator != null)
                {
                    attackObjectAnimator.SetTrigger("Swing2");
                }
                break;
        }


        // Play sword animation (if separate)


        // Determine the quadrant (X shape)
        int attackQuadrant = GetMouseQuadrant(direction);

            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

            foreach (Collider2D objects in hitObjects)
            {

                Vector2 objectsDir = (objects.transform.position - attackPoint.position).normalized;
                float angleToObject = Vector2.Angle(direction, objectsDir);
                float generousAttackAngle = attackAngle * 1.13f; // 13% more generous

                if (angleToObject <= generousAttackAngle / 2) // Check if inside attack cone
                {

                    if (objects.CompareTag("Enemy"))
                    {
                        objects.GetComponent<Health>().Hit(attackDamage, 0);
                        GameObject effect = Instantiate(swordScript.impactEffect, objects.transform.position, Quaternion.Euler(0, 0, Mathf.Atan2(objectsDir.y, objectsDir.x) * Mathf.Rad2Deg));
                        Destroy(effect, 1f);
                    }

                    else if (objects.CompareTag("Destructible"))
                    {
                        objects.GetComponent<Health>().Hit(attackDamage, 0);
                        GameObject effect = Instantiate(swordScript.impactEffect, objects.transform.position, Quaternion.Euler(0, 0, Mathf.Atan2(objectsDir.y, objectsDir.x) * Mathf.Rad2Deg));
                        Destroy(effect, 1f);
                    }
                }
            }

        anim.SetInteger("Direction", GetMouseQuadrant(direction));



    }

    private int GetMouseQuadrant(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Get angle in degrees

        if (angle >= -45 && angle < 45) return 3;  // Right side
        if (angle >= 45 && angle < 135) return 0;  // Top side
        if (angle >= -135 && angle < -45) return 2; // Bottom side
        return 1; // Left side
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        if (sword)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);

            // Get mouse position in world space
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 attackDir = (mousePos - (Vector2)attackPoint.position).normalized;

            Vector2 leftLimit = Quaternion.Euler(0, 0, -attackAngle / 2) * attackDir;
            Vector2 rightLimit = Quaternion.Euler(0, 0, attackAngle / 2) * attackDir;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(attackPoint.position, attackPoint.position + (Vector3)leftLimit * attackRange);
            Gizmos.DrawLine(attackPoint.position, attackPoint.position + (Vector3)rightLimit * attackRange);
        }


    }

    private IEnumerator AttackReset()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        isAttacking = false;
    }

}
