using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    public int damage = 1;
    public int source = 1;
    public float damageCooldown = 0.5f; // Time between damage instances
    private float lastDamageTime;

    private DasherEnemyAI dasher;
    private EntitySFX sfx;
    public bool isKnockedBack;

    private void Start()
    {
        dasher = GetComponent<DasherEnemyAI>();
        sfx = GetComponent<EntitySFX>();
        isKnockedBack = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time >= lastDamageTime + damageCooldown)
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                if (dasher != null && dasher.isDashing)
                {
                    playerHealth.Hit(Mathf.RoundToInt(damage * dasher.dashDamageMultiplier), source);


                    DasherHitSomething();

                    lastDamageTime = Time.time;
                }
                else
                {
                    playerHealth.Hit(damage, source);
                    lastDamageTime = Time.time;
                }
            }
            Debug.Log("Player Damaged");
        }

        if(other.CompareTag("Barrel") && dasher != null && dasher.isDashing)
        {

            Health barrelHealth = other.GetComponent<Health>();
            barrelHealth.Hit(Mathf.RoundToInt(damage * dasher.dashDamageMultiplier), 0);
            other.GetComponent<EntitySFX>().BarrelHitSFX();
            DasherHitSomething();
        }

        if (other.CompareTag("Enemy") && dasher != null && dasher.isDashing)
        {

            Health enemyHealth = other.GetComponent<Health>();
            enemyHealth.Hit(Mathf.RoundToInt(damage * dasher.dashDamageMultiplier), 0);
            //other.GetComponent<EntitySFX>().SFX();
            DasherHitSomething();
        }
    }

    void DasherHitSomething()
    {
        dasher.stunAfterDash *= dasher.stunAfterHitMultiplier;
        dasher.StopDash();
        dasher.GetComponent<Health>().Hit(Mathf.RoundToInt(damage * dasher.dashDamageMultiplier), 0);
        dasher.Stun.SetActive(true);
        StartCoroutine(dasher.StunAfterDash());
        StopCoroutine(dasher.dashCoroutine);
        sfx.StunSFX();
    }
}
