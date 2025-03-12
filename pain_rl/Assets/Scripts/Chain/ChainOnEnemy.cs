using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ChainOnEnemy : MonoBehaviour
{
    public float activateTime;
    public float searchForJumpTime;
    public float maxJumpDistance;
    public ChainSkill skill;
    public bool isTheLatest = false;
    public bool isTheOldest = false;
    public GameObject parentObject;
    public bool isActive;
    public bool isSearching;
    private EntitySFX entitySFX;
    private bool hasSpawnedAnother = false;
    public GameObject go;
    private Animator anim;
    private bool animPlayed = false;
    public Coroutine Search;
    public Coroutine Activate;

    public static T FindObjectByName<T>(string objectName) where T : MonoBehaviour
    {
        GameObject obj = GameObject.Find(objectName);
        return obj ? obj.GetComponent<T>() : null;
    }

    private void Awake()
    {
        entitySFX = GetComponent<EntitySFX>();
    }
    private void Start()
    {
        skill = FindObjectByName<ChainSkill>("Chain Skill");
        anim = GetComponent<Animator>();
        activateTime = skill.chainActivateTime;
        searchForJumpTime = skill.chainJumpSearchTime;
        maxJumpDistance = skill.maxChainJumpDistance;
        go = this.gameObject;
        skill.AddChain(go);
        isActive = false;
        entitySFX.ChainHit();
        Activate = StartCoroutine(Activator());
    }

    private void Update()
    {
        if(skill.latestChain == this.gameObject)
        {
            isTheLatest = true;
        }
        else
        {
            isTheLatest = false;
        }

        if(skill.oldestChain == this.gameObject)
        {
            isTheOldest = true;
        }
        else
        {
            isTheOldest = false;
        }

        this.gameObject.transform.position = parentObject.transform.position;

        if(isActive && isTheLatest && isSearching)
        {
            Jump();        
        }

        if(parentObject == null)
        {
            skill.RemoveObject(parentObject);
            skill.RemoveChain(go);
        }
    }

    IEnumerator Activator()
    {
        anim.speed = 1 / activateTime; //speed 1 iken aktivasyon animasyonu 1 saniyelik ise...

        yield return new WaitForSeconds(activateTime);

        isActive = true;
        skill.AddObject(parentObject);
        anim.speed = 1;
        entitySFX.ChainActive();
        Search = StartCoroutine(SearchForJump());

        GameObject effect = Instantiate(skill.ChainParticlePurple, go.transform.position, Quaternion.identity);
        Destroy(effect, 1f);

    }

    IEnumerator SearchForJump()
    {
        isSearching = true;
        anim.SetTrigger("searching");
        yield return new WaitForSeconds(0.5f);
        if(isSearching)
        {
            entitySFX.ChainSearchLoop();
        }
        yield return new WaitForSeconds(searchForJumpTime - 0.5f);
        isSearching = false;
        entitySFX.audioSource.clip = null;
        StopAnim();
        
    }

    private void Jump()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, maxJumpDistance);
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Enemy") && col.gameObject != parentObject && !hasSpawnedAnother)
            {
                bool alreadyHasChain = col.GetComponentInChildren<ChainOnEnemy>() != null;

                if (!alreadyHasChain)
                {
                    hasSpawnedAnother = true;
                    var go = Instantiate(skill.ChainOnEnemyPrefab, col.transform.position, Quaternion.identity, col.transform);
                    entitySFX.audioSource.clip = null;
                    skill.latestChain = go;
                    go.GetComponent<EntitySFX>().ChainHit();
                    go.GetComponent<ChainOnEnemy>().parentObject = col.gameObject;
                    Deactivate();
                    return; // Stop searching after finding one valid target
                }
            }
        }
    }



    void Deactivate()
    {
        isSearching = false;
        StopAnim();
    }

    private void OnDrawGizmosSelected()
    {
            Gizmos.color = Color.cyan; // Set the gizmo color
            Gizmos.DrawWireSphere(transform.position, maxJumpDistance); // Draw a wireframe sphere

    }

    public void StopAnim()
    {
        if (!animPlayed)
        {
            anim.SetTrigger("done");
            animPlayed = true;
            entitySFX.audioSource.clip = null;
            entitySFX.ChainLocked();
            GameObject effect = Instantiate(skill.ChainParticleGreen, go.transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }
    }

}
