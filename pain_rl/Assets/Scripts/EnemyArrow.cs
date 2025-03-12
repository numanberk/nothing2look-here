using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    [Header("Arrow Properties")]
    public float speed = 10f;
    public int damage = 10;
    public int source = 1;
    public float lifetime = 5f;

    private Vector2 direction;
    private Rigidbody2D rb;
    [Header("DONT TOUCH")]
    public GameObject parent;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime); // Destroy after a set time
        if (direction == Vector2.zero) // Ensure direction is set
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                SetDirection((player.transform.position - transform.position).normalized);
            }
        }
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        transform.right = direction; // Rotate the arrow to face its movement direction
    }

    void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Hit(damage, source);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Barrel"))
        {
            Health barrelHealth = other.GetComponent<Health>();
            if (barrelHealth != null)
            {
                barrelHealth.Hit(damage, 0);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            if(other.gameObject != this.parent)
            {
                Health enemyHealth = other.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.Hit(damage, 0);
                }
                Destroy(gameObject);
            }

        }
    }
}