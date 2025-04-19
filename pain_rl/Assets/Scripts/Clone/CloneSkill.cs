using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CloneSkill : MonoBehaviour
{
    //public static CloneSkill instance;
    [Header("Values")]
    //[SerializeField] public float delay;
    [SerializeField] public float lifetime;
    [Header("ASSIGN")]
    [SerializeField] GameObject CloneSkillUIPrefab;
    [SerializeField] GameObject CloneF1UIPrefab;

    private SkillManager skillManager;
    private EntitySFX entitySFX;
    private GameObject Player;
    private GameObject skillUI;
    private GameObject skillInfos;
    private GameObject currentClone;
    private bool firstTime;
    private Slider slider;
    private float elapsedTime;

    private void Awake()
    {
        entitySFX = GetComponent<EntitySFX>();
        //if(instance == null)
        //{
            //instance = this;
        //}

    }
    private void Start()
    {
        Player = GameObject.Find("Player");
        skillUI = GameObject.Find("SkillsUI");
        skillInfos = GameObject.Find("Skill Infos");
        skillManager = GetComponent<SkillManager>();
        skillManager.requirementsMetForSkill = true;
        skillManager.endableSkill = false;
        firstTime = true;
    }

    public void SpawnUI()
    {
        GameObject newUI = Instantiate(CloneSkillUIPrefab, skillUI.transform);
        newUI.transform.localPosition = Vector3.zero; // Adjust if necessary
        skillManager.SpawnedUI = true;
        skillManager.currentUI = newUI;

        GameObject newF1 = Instantiate(CloneF1UIPrefab, skillInfos.transform);
        newF1.transform.localPosition = Vector3.zero;
        skillManager.SpawnedF1 = true;
        skillManager.currentF1 = newF1;
    }

    public void SkillStart()
    {
        skillManager.isRunning = true;
        skillManager.canGoToCooldown = false;
        skillManager.requirementsMetForSkill = false; //UI iyi gözüksün diye, clone süresi bitince true'la ikisini de.
        

        InstantiateClone();
    }

    private void InstantiateClone()
    {
        GameObject player = Player; // or however you reference them
        GameObject clone = Instantiate(player, player.transform.position, player.transform.rotation);
        clone.tag = "PlayerClone";

        DestroyImmediate(clone.GetComponent<PlayerMovement>());

        DestroyImmediate(clone.GetComponent<PlayerPain>());
        DestroyImmediate(clone.GetComponent<Health>());


        Sword originalSword = player.GetComponentInChildren<Sword>();
        GameObject swordHolder = clone.GetComponentInChildren<Sword>().gameObject; // or whatever it's called

        DestroyImmediate(clone.GetComponentInChildren<Sword>());
        DestroyImmediate(clone.GetComponent<PlayerAttack>());

        
        CloneSword cloneSword = swordHolder.gameObject.AddComponent<CloneSword>();
        cloneSword.SetupFromSword(originalSword);
        Transform cloneAttackPoint = clone.transform.Find("AttackPoint"); // or use any accurate path
        cloneSword.attackPoint = cloneAttackPoint;


        clone.AddComponent<CloneAttack>();
        clone.GetComponent<CloneAttack>().cloneSword = cloneSword;




        DestroyImmediate(clone.GetComponent<Rigidbody2D>());
        DestroyImmediate(clone.GetComponent<BoxCollider2D>());

        SpriteRenderer[] spriteRenderers = clone.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in spriteRenderers)
        {
            Color c = sr.color;
            c.a = 0.5f;
            sr.color = c;
        }

        currentClone = clone;
        StartCoroutine(DestroyClone());


    }

    IEnumerator DestroyClone()
    {
        elapsedTime = 0f;

        while (elapsedTime < lifetime)
        {
            elapsedTime += Time.deltaTime;

            if (skillManager.other2 != null)
            {
                slider = skillManager.other2.GetComponent<Slider>();
                float timeLeft = lifetime - elapsedTime;
                slider.value = timeLeft / lifetime;
            }

            yield return null;
        }

        Destroy(currentClone);

        skillManager.canGoToCooldown = true;
        skillManager.requirementsMetForSkill = true;
        skillManager.isRunning = false;
    }


    private void Update()
    {
        if (skillManager.secondary != null && firstTime)
        {
            firstTime = false;
            skillManager.secondary.text = (lifetime.ToString() + "s");
        }
    }
}
