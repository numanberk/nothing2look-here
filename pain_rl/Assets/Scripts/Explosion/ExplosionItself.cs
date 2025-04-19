using System.Collections;
using UnityEngine;

public class ExplosionItself : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private Transform maxPoint;

    private float minDamageAmplifier;
    private float maxDamageAmplifier;

    private float explosionDamage;
    private float explosionRange;
    private float hitStopp;

    private void Start()
    {
        StartCoroutine(Destroyer());
    }

    public void Setup(float damage, float range, float amp, float amp2, float hitStop)
    {
        explosionDamage = damage;
        minDamageAmplifier = amp;
        maxDamageAmplifier = amp2;
        explosionRange = range;
        hitStopp = hitStop;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        float distance = Vector2.Distance(other.transform.position, this.gameObject.transform.position);
        float explosionRangeDistance = Vector2.Distance(this.gameObject.transform.position, maxPoint.position);

        float normalizedDistance = Mathf.Clamp01(distance / explosionRangeDistance);

        float distanceDmgMultiplier = Mathf.Lerp(maxDamageAmplifier, minDamageAmplifier, normalizedDistance);

        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(explosionDamage * distanceDmgMultiplier));



            if (other.CompareTag("Enemy"))
            {
                
                Health enemyHealth = other.GetComponent<Health>();
                enemyHealth.Hit(finalDamage, 42); // 42 = explosion code


            //Debug.Log($"[Explosion] Target: {collision.name}, Range: {explosionRange}, MaxDistance: {explosionRangeDistance}, HitDistance: {distance}, Damage: {explosionDamage}, Multiplier: {distanceDmgMultiplier}, Final: {finalDamage}");
            }


            if (other.CompareTag("Barrel"))
            {
                Health barrelHealth = other.GetComponent<Health>();
                barrelHealth.Hit(finalDamage, 42); // 42 = explosion code
                other.GetComponent<EntitySFX>().BarrelHitSFX();

            }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (GetComponent<CircleCollider2D>() != null)
        {
            float radius = GetComponent<CircleCollider2D>().radius * transform.localScale.x;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }

    IEnumerator Destroyer()
    {
        yield return new WaitForSeconds(lifeTime / 2);
        HitStop.Instance.Stop(hitStopp);
        yield return new WaitForSeconds(lifeTime / 2);
        Destroy(gameObject);
    }

}
