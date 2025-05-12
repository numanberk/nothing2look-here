using UnityEngine;

public class ExplosionShatterdash : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var baseDamage = PlayerAttack.Instance.attackDamage * PlayerAttack.Instance.weaponSkillDamageMultiplier;

        if (other.CompareTag("Enemy"))
        {

            Health enemyHealth = other.GetComponent<Health>();
            enemyHealth.Hit(Mathf.RoundToInt(baseDamage * ShatterdashSkill.Instance.explosionDamageMultiplier), 45); // 45 = shatterdash explosion code

        }


        if (other.CompareTag("Barrel"))
        {
            Health barrelHealth = other.GetComponent<Health>();
            barrelHealth.Hit(Mathf.RoundToInt(baseDamage * ShatterdashSkill.Instance.explosionDamageMultiplier), 45); // 45 = shatterdash explosion code
            other.GetComponent<EntitySFX>().BarrelHitSFX();

        }
    }
}
