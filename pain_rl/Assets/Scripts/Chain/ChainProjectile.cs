using System.Collections;
using UnityEngine;
using static Pathfinding.Util.RetainedGizmos;
using static Unity.VisualScripting.Member;

public class ChainProjectile : MonoBehaviour
{

    [Header("DONT TOUCH")]
    public int damage;
    public float lifetime;
    private ChainSkill skill;
    private EntitySFX entitySFX;

    public static T FindObjectByName<T>(string objectName) where T : MonoBehaviour
    {
        GameObject obj = GameObject.Find(objectName);
        return obj ? obj.GetComponent<T>() : null;
    }
    private void Start()
    {
        skill = FindObjectByName<ChainSkill>("Chain Skill");
        entitySFX = GetComponent<EntitySFX>();
        entitySFX.ChainProjectile();
        StartCoroutine(Lifetime());
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Health enemyHealth = other.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.Hit(damage, 0);
                SpawnChainOnEnemy(other.transform, other.gameObject);
            }
        }
        if (other.CompareTag("Barrel"))
        {
            Health barrelHealth = other.GetComponent<Health>();
            if (barrelHealth != null)
            {
                barrelHealth.Hit(damage, 0);
                other.GetComponent<EntitySFX>().BarrelHitSFX();
                Destroy(this.gameObject);
                skill.skillManager.BackToCooldown();
            }
        }
    }

    private void SpawnChainOnEnemy(Transform parent, GameObject parentObject)
    {
        var go = Instantiate(skill.ChainOnEnemyPrefab, parent.transform.position, Quaternion.identity, parent.transform);
        go.GetComponent<ChainOnEnemy>().parentObject = parentObject;
        skill.oldestChain = go;
        skill.latestChain = go;
        StartCoroutine(Destroyer());
    }

    IEnumerator Destroyer()
    {
        var sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        var rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector3.zero;
        var collider = GetComponent<CircleCollider2D>();
        collider.enabled = false;    
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(this.gameObject);
        skill.skillManager.BackToCooldown();
    }
}
