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
    [SerializeField] GameObject CloneBookImagePrefab;
    [SerializeField] GameObject CloneBookInfoPrefab;
    [SerializeField] GameObject CloneCanvasSlider;

    private SkillManager skillManager;
    private EntitySFX entitySFX;
    private GameObject Player;
    private GameObject skillUI;
    private GameObject skillInfos;
    private GameObject currentClone;
    private bool firstTime;
    private Slider slider;
    private GameObject sliderCloneCanvas;
    private float elapsedTime;
    private bool hasInstantiatedBook;

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

        GameObject newF1 = Instantiate(CloneF1UIPrefab, skillManager.row.transform);
        newF1.transform.localPosition = Vector3.zero;
        skillManager.SpawnedF1 = true;
        skillManager.currentF1 = newF1;


    }

    public void SkillStart()
    {
        StartCoroutine(WaitUntilNotAttackingAndStart());

        //skillManager.canGoToCooldown = false;
        //skillManager.isRunning = true;
        //skillManager.requirementsMetForSkill = false;
        //InstantiateClone();

    }


    private void InstantiateClone() //SES BUGUNU DÜZELT, CHARGE SFX SESÝ ÇIKIYOR. ONUN YERÝNE AUDÝOSOURCE KALDIR, KENDÝN EKLE, YENÝ SFX VER.
    {
        GameObject player = Player;
        GameObject clone = Instantiate(player, player.transform.position, player.transform.rotation);
        clone.tag = "PlayerClone";

        DestroyImmediate(clone.GetComponent<PlayerMovement>());

        DestroyImmediate(clone.GetComponent<PlayerPain>());
        DestroyImmediate(clone.GetComponent<Health>());
        DestroyImmediate(clone.GetComponent<PlayerAttack>());

        Transform cloneAttackPoint = clone.transform.Find("AttackPoint");
        Transform cloneCanvas = clone.transform.Find("Canvas");
        GameObject chargeSlider = cloneCanvas.transform.Find("ChargeSlider").gameObject;
        GameObject MAX = cloneCanvas.transform.Find("MAX!").gameObject;
        sliderCloneCanvas = Instantiate(CloneCanvasSlider, cloneCanvas.transform);

        if (PlayerAttack.Instance.sword)
        {
            Sword originalSword = player.GetComponentInChildren<Sword>();
            GameObject swordHolder = clone.GetComponentInChildren<Sword>().gameObject;

            DestroyImmediate(clone.GetComponentInChildren<Sword>());


            CloneSword cloneSword = swordHolder.gameObject.AddComponent<CloneSword>();
            cloneSword.SetupFromSword(originalSword);  
            cloneSword.attackPoint = cloneAttackPoint;


            clone.AddComponent<CloneAttack>();
            clone.GetComponent<CloneAttack>().cloneSword = cloneSword;
        }

        if (PlayerAttack.Instance.punch)
        {
            Punch originalPunch = player.GetComponentInChildren<Punch>();
            GameObject punchHolder = clone.GetComponentInChildren<Punch>().gameObject;

            DestroyImmediate(clone.GetComponentInChildren<Punch>());


            ClonePunch clonePunch = punchHolder.gameObject.AddComponent<ClonePunch>();
            clonePunch.SetupFromPunch(originalPunch);
            clonePunch.attackPoint = cloneAttackPoint;


            clone.AddComponent<CloneAttack>();
            clone.GetComponent<CloneAttack>().clonePunch = clonePunch;
        }





        DestroyImmediate(clone.GetComponent<Rigidbody2D>());
        DestroyImmediate(clone.GetComponent<BoxCollider2D>());
        DestroyImmediate(chargeSlider);
        DestroyImmediate(MAX);

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
                sliderCloneCanvas.GetComponent<Slider>().value = slider.value;
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

        if (!hasInstantiatedBook && Book.Instance != null)
        {
            InstantiateBook();
            hasInstantiatedBook = true;
        }
    }

    private IEnumerator WaitUntilNotAttackingAndStart()
    {

        skillManager.canGoToCooldown = false;
        skillManager.isRunning = true;
        skillManager.requirementsMetForSkill = false;

        if (!PlayerAttack.Instance.isAttacking && !PlayerAttack.Instance.isCharging)
        {
            StartSkillLogic();
            yield break;
        }

        yield return new WaitUntil(() => !PlayerAttack.Instance.isAttacking && !PlayerAttack.Instance.isCharging);

        if (!PlayerAttack.Instance.isAttacking && !PlayerAttack.Instance.isCharging)
        {
            StartSkillLogic();
        }
    }

    private void InstantiateBook()
    {
        var bookObj = Instantiate(CloneBookImagePrefab);
        bookObj.GetComponent<SkillsBookButton>().mainSkillScript = this.gameObject;
        bookObj.GetComponent<SkillsBookButton>().infoPrefab = CloneBookInfoPrefab;
        Book.Instance.PlaceNextObject(bookObj);
    }

    private void StartSkillLogic()
    {
        //PlayerAttack.Instance.ComboResetFunc();
        InstantiateClone();
    }
}
