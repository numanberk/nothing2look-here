using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ChainSkill : MonoBehaviour
{
    public static ChainSkill instance;

    [Header("Values Projectile")]
    [SerializeField] private float chainProjectileSpeed;
    [SerializeField] private int chainDamage;
    [SerializeField] private float chainLifetime;

    [Header("Values Chain")]
    [SerializeField] public float chainActivateTime;
    [SerializeField] public float chainJumpSearchTime;
    [SerializeField] public float maxChainJumpDistance;

    [Header("Assign")]
    [SerializeField] private GameObject ChainSkillUIPrefab;
    [SerializeField] private GameObject ChainProjectilePrefab;
    [SerializeField] public GameObject ChainOnEnemyPrefab;
    [SerializeField] public GameObject ChainParticlePurple;
    [SerializeField] public GameObject ChainParticleGreen;
    [SerializeField] public GameObject ChainF1UIPrefab;
    [SerializeField] private float attackDelay;

    [Header("DONT TOUCH")]
    public GameObject latestChain;
    public GameObject oldestChain;
    public SkillManager skillManager;
    public List<GameObject> chainedList = new List<GameObject>(); //LÝSTEDE BULUNAN DÜÞMANLARA NE OLACAK? (VURULAN KENDÝSÝ HASARIN %100'ÜNÜ ALIRKEN DÝÐERLERÝ BÖLÜÞECEK MÝ VS.)??? - ayarlandý. (distribution())
    public List<GameObject> allChainsList = new List<GameObject>();
    public GameObject skillUI;
    public GameObject skillInfos;
    private EntitySFX entitySFX;
    private bool inAttackCooldown = false;
    private GameObject Player;

    private void Awake()
    {
        instance = this;
        entitySFX = GetComponent<EntitySFX>();
    }
    private void Start()                                                                                        
    {
        skillUI = GameObject.Find("SkillsUI");
        skillInfos = GameObject.Find("Skill Infos");
        skillManager = GetComponent<SkillManager>();
        skillManager.requirementsMetForSkill = true;
        skillManager.endableSkill = true;
        Player = GameObject.Find("Player");
    }

    private void Update()
    {
        CleanUpList();
        GreenCheck();

        if (latestChain != null && latestChain.GetComponent<ChainOnEnemy>().isActive && !latestChain.GetComponent<ChainOnEnemy>().isSearching && oldestChain == latestChain || allChainsList.Count == 0)
        {
            ChainEndCheck();
        }

        if(skillManager.isRunning)
        {
            skillManager.other.GetComponentInChildren<TextMeshProUGUI>().text = chainedList.Count.ToString();
            skillManager.other.SetActive(true);
        }
        else
        {
            skillManager.other.SetActive(false);
        }

        if(inAttackCooldown)
        {
            PlayerAttack.Instance.canAttack = false;
        }


    }
    public void SkillGo()
    {
        //anim yap, anim uzunluðunu da attackDelay'e eþitle.
        StartCoroutine(AttackCooldown());

        skillManager.canGoToCooldown = false;
        skillManager.isRunning = true;

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;

        Vector3 spawnPosition = skillManager.Player.transform.position;
        Vector3 direction = (mouseWorldPosition - spawnPosition).normalized;


        GameObject projectile = Instantiate(ChainProjectilePrefab, spawnPosition, Quaternion.identity);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * chainProjectileSpeed;
        }

        projectile.GetComponent<ChainProjectile>().damage = chainDamage;
        projectile.GetComponent<ChainProjectile>().lifetime = chainLifetime;

    }

    public void SkillEnd()
    {
        if (latestChain != null)
        {
            var script = latestChain.GetComponent<ChainOnEnemy>();

            if(script.Search != null)
            {
                StopCoroutine(script.Search);
            }
            if (script.Activate != null)
            {
                StopCoroutine(script.Activate);
            }
            script.isSearching = false;
            script.StopAnim();
            if (!script.isActive)
            {
                Destroy(latestChain);
                //RemoveObject(script.parentObject);
            }
            skillManager.keybind.enabled = false;
        }
    }

    public void GoCooldown()
    {
        skillManager.canGoToCooldown = true;
    }

    public void AddObject(GameObject obj)
    {
        if (obj != null && !chainedList.Contains(obj))
        {
            chainedList.Add(obj);
        }
    }

    public void AddChain(GameObject obj)
    {
        if (obj != null && !chainedList.Contains(obj))
        {
            allChainsList.Add(obj);
        }
    }

    public void RemoveObject(GameObject objectt)
    {
        GameObject obj = chainedList.Find(o => o == objectt);
        if (obj != null)
        {
            chainedList.Remove(obj);
        }
    }

    public void RemoveChain(GameObject objectt)
    {
        GameObject obj = allChainsList.Find(o => o == objectt);
        if (obj != null)
        {
            allChainsList.Remove(obj);
        }
    }

    public void RemoveAllObjects()
    {
        foreach (GameObject obj in chainedList)
        {
            var script = obj.GetComponentInChildren<ChainOnEnemy>();
            if (script != null)
            {
                Destroy(script.go); // Removes chains from the scene
            }

        }

        chainedList.Clear(); // Clears the list
        allChainsList.Clear();
    }

    private void CleanUpList()
    {
        int initialChainedCount = chainedList.Count;
        int initialAllChainsCount = allChainsList.Count;

        chainedList.RemoveAll(obj => obj == null);
        allChainsList.RemoveAll(obj => obj == null);

        if (chainedList.Count < initialChainedCount || allChainsList.Count < initialAllChainsCount)
        {
            ChainEndCheck();
            if (skillManager.isRunning && allChainsList.Count == 0)
            {
                skillManager.BackToCooldown();
                entitySFX.ChainBreak();
            }
        }
    }



    public void DamageDistribution(int damage, GameObject sender) 
    {
        foreach (GameObject obj in chainedList)
        {
            if(obj != sender)
            {
                var damageDistributionCount = chainedList.Count - 1;
                if(damageDistributionCount > 0)
                {
                    obj.GetComponent<Health>().Hit(damage / damageDistributionCount, -1);
                }

            }
            
        }
    }

    public void ChainEndCheck()
    {
        for (int i = allChainsList.Count - 1; i >= 0; i--) // Iterate backwards
        {
            GameObject chain = allChainsList[i];

            if (chain != null && chain.GetComponent<ChainOnEnemy>().isActive && !chain.GetComponent<ChainOnEnemy>().isSearching && allChainsList.Count == 1)
            {
                RemoveAllObjects();
                skillManager.BackToCooldown();
                entitySFX.ChainBreak();
                break; // Exit loop after modifying the list
            }


        }



    }

    public void SpawnUI()
    {
        GameObject newUI = Instantiate(ChainSkillUIPrefab, skillUI.transform);
        newUI.transform.localPosition = Vector3.zero; // Adjust if necessary
        skillManager.SpawnedUI = true;
        skillManager.currentUI = newUI;

        GameObject newF1 = Instantiate(ChainF1UIPrefab, skillInfos.transform);
        newF1.transform.localPosition = Vector3.zero;
        skillManager.SpawnedF1 = true;
        skillManager.currentF1 = newF1;
    }

    private void GreenCheck()
    {
        bool everyoneGreen = true; // Start assuming it's true

        foreach (GameObject obj in allChainsList)
        {
            ChainOnEnemy chain = obj.GetComponent<ChainOnEnemy>();
            if (chain.isSearching || !chain.isActive) // If ANY chain isn't active or is searching
            {
                everyoneGreen = false;
                
                break; // No need to check further, we already know the result
            }
        }

        // Set the animator bool once, after the loop
        skillManager.currentUI.GetComponent<Animator>().SetBool("everyoneGreen", everyoneGreen);
        if(allChainsList.Count > 0)
        {
            skillManager.keybind.enabled = !everyoneGreen;
        }

    }

    IEnumerator AttackCooldown()
    {
        inAttackCooldown = true;
        StartCoroutine(ChargeInterrupt());
        yield return new WaitForSeconds(attackDelay);
        inAttackCooldown = false;
        PlayerAttack.Instance.canAttack = true;
    }

    IEnumerator ChargeInterrupt()
    {
        Player.GetComponentInChildren<Sword>().chargeInterrupt = true;
        yield return new WaitForSeconds(0.018f);
        Player.GetComponentInChildren<Sword>().chargeInterrupt = false;
    }




}
