using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 2f; // Enemy speed
    private Transform player;
    private Animator animator;
    public int animDirection; // Enemy's own movement direction

    void Start()
    {
        animator = GetComponent<Animator>();

        // Find the player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Calculate movement direction
            Vector2 direction = (player.position - transform.position);
            float distance = direction.magnitude; // Distance to player
            direction.Normalize(); // Get unit vector for movement

            // Move the enemy if it's not already at the player position
            if (distance > 0.1f) // Small threshold to avoid jitter
            {
                transform.position += (Vector3)direction * speed * Time.deltaTime;
                SetDirection(direction);
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
    }

    void SetDirection(Vector2 direction)
    {
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);

        // Determine the primary movement direction
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) // Moving more horizontally
        {
            animDirection = (direction.x > 0) ? 3 : 1; // Right = 3, Left = 1
        }
        else // Moving more vertically
        {
            animDirection = (direction.y > 0) ? 0 : 2; // Up = 0, Down = 2
        }

        animator.SetInteger("Direction", animDirection);
    }

}
