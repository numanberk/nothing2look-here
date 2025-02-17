using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    public int damage = 1;
    public int source = 1;
    public float damageCooldown = 0.5f; // Time between damage instances
    private float lastDamageTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time >= lastDamageTime + damageCooldown)
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Hit(damage, source);
                lastDamageTime = Time.time; // Update the last damage time
            }

            // Optional: Knock the player back
            //Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            //if (playerRb != null)
            //{
            //Vector2 knockback = (other.transform.position - transform.position).normalized * 5f;
            //playerRb.AddForce(knockback, ForceMode2D.Impulse);
            //}

            Debug.Log("Player Damaged");
        }
    }
}
