using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("SET THESE UP!")]
    [SerializeField] public Transform attackPoint;
    [SerializeField] public float animationLength;

    [Header("Adjustable Variables")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackAngle = 60f; // Angle of attack cone
    [SerializeField] private float attackCooldown = 0.6f;

    private bool canAttack;
    public bool isAttacking;
    private Animator anim;
    private PlayerMovement movementScript;

    private void Start()
    {
        anim = GetComponent<Animator>();
        movementScript = GetComponent<PlayerMovement>();
        canAttack = true;
        isAttacking = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(canAttack)
            {
                Attack();
            }
            
        }

    }


    private void Attack()
    {
        anim.SetTrigger("Attack");
        canAttack = false;
        isAttacking = true;
        StartCoroutine(AttackReset());

        // Get mouse position relative to the player
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)attackPoint.position).normalized;

        // Determine the quadrant (X shape)
        int attackQuadrant = GetMouseQuadrant(direction);
        Debug.Log("Attacking in Quadrant: " + attackQuadrant);

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

        foreach (Collider2D objects in hitObjects)
        {

            Vector2 objectsDir = (objects.transform.position - attackPoint.position).normalized;
            float angleToObject = Vector2.Angle(direction, objectsDir);

            if (angleToObject <= attackAngle / 2) // Check if inside attack cone
            {
                objects.GetComponent<Health>().Hit(attackDamage, 0);

                if (objects.CompareTag("Enemy"))
                {
                    return;
                }
                else if (objects.CompareTag("Destructible"))
                {
                    Debug.Log("destructible");
                }
            }



            




            
        }

        anim.SetInteger("Direction", GetMouseQuadrant(direction));
        StartCoroutine(ResetDirectionAfterAttack());



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

    private IEnumerator ResetDirectionAfterAttack()
    {
        yield return new WaitForSeconds(animationLength); // Adjust based on attack animation length
        isAttacking = false;
    }

    private IEnumerator AttackReset()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
