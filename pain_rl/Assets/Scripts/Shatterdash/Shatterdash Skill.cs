using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShatterdashSkill : MonoBehaviour
{
    public static ShatterdashSkill Instance;


    [Header("VALUES")]
    [SerializeField] public float crystalLifetime;
    [SerializeField] float dashSpeed;
    [SerializeField] public float dashRange;
    [SerializeField] public float dashDamageMultiplier;
    [SerializeField] public float explosionRange;
    [SerializeField] public float explosionDamageMultiplier;

    [Header("ASSIGN")]
    [SerializeField] public GameObject CrystalPrefab;
    [SerializeField] public GameObject ExplosionPrefab;
    [SerializeField] GameObject ExplosionParticlePrefab;
    [SerializeField] GameObject ShatterdashSkillUIPrefab;    
    [SerializeField] GameObject ShatterdashBookImagePrefab;
    [SerializeField] GameObject ShatterdashBookInfoPrefab;
    [SerializeField] GameObject ShatterdashF1UIPrefab;

    [Header("DONT TOUCH")]
    public GameObject currentCrystal;
    public SkillManager skillManager;
    private EntitySFX entitySFX;
    private GameObject Player;
    private GameObject skillUI;
    private GameObject skillInfosRow1;
    private GameObject skillInfosRow2;
    private bool firstTime;
    private bool hasDashedYet;
    public bool isDashing;
    private Slider slider;
    private float elapsedTime;
    public Coroutine timer;
    public Vector3 lastCrystalPos;
    private bool hasInstantiatedBook;

    private void Awake()
    {
        entitySFX = GetComponent<EntitySFX>();
        Instance = this;

    }
    private void Start()
    {
        Player = GameObject.Find("Player");
        skillUI = GameObject.Find("SkillsUI");
        skillManager = GetComponent<SkillManager>();
        skillManager.requirementsMetForSkill = true;
        skillManager.endableSkill = false;
        firstTime = true;
        hasDashedYet = false;
        isDashing = false;
    }

    private void Update()
    {
        if(currentCrystal != null)
        {
            if (Vector3.Distance(Player.transform.position, currentCrystal.transform.position) <= dashRange)
            {
                skillManager.canPressButton = true;
            }
            else
            {
                skillManager.canPressButton = false;
            }
            
        }
        else if (currentCrystal == null)
        {
            skillManager.canPressButton = false;
        }

        if (skillManager.secondary != null && firstTime)
        {
            firstTime = false;
            skillManager.secondary.text = (crystalLifetime.ToString() + "s");
        }

        if (!hasInstantiatedBook && Book.Instance != null)
        {
            InstantiateBook();
            hasInstantiatedBook = true;
        }

    }
    public void SpawnUI()
    {
        GameObject newUI = Instantiate(ShatterdashSkillUIPrefab, skillUI.transform);
        newUI.transform.localPosition = Vector3.zero; // Adjust if necessary
        skillManager.SpawnedUI = true;
        skillManager.currentUI = newUI;

        GameObject newF1 = Instantiate(ShatterdashF1UIPrefab, skillManager.row.transform);
        newF1.transform.localPosition = Vector3.zero;
        skillManager.SpawnedF1 = true;
        skillManager.currentF1 = newF1;
    }

    public void StartSkill()
    {
        Player.GetComponent<Health>().isInvulnerable = true;
        isDashing = true;

        hasDashedYet = true;
        StartCoroutine(DashToCrystal());
    }

    private IEnumerator DashToCrystal()
    {
        entitySFX.Dash();

        while (Vector3.Distance(Player.transform.position, currentCrystal.transform.position) > 0.25f)
        {
            Vector3 direction = (currentCrystal.transform.position - Player.transform.position).normalized;
            Player.transform.position += direction * dashSpeed * Time.deltaTime;
            currentCrystal.GetComponent<Lifetime>().destroyable = false;
            lastCrystalPos = currentCrystal.transform.position;
            yield return null;
        }

        Player.transform.position = currentCrystal.transform.position;

        // AFTER DASH ENDS:
        if (hasDashedYet)
        {
            Destroy(currentCrystal);
            skillManager.BackToCooldown();
            Player.GetComponent<Health>().isInvulnerable = false;
            hasDashedYet = false;
            isDashing = false;
            InstantiateExplosion(lastCrystalPos);
        }
    }

    void InstantiateExplosion(Vector3 position)
    {
        var exp = Instantiate(ExplosionPrefab, position, Quaternion.identity);
        exp.GetComponent<Lifetime>().Setup(1f);
        exp.transform.localScale = new Vector3(explosionRange, explosionRange, exp.transform.localScale.z);


        GameObject effect = Instantiate(ExplosionParticlePrefab, position, Quaternion.identity);
        effect.transform.localScale = new Vector3(explosionRange / 2f, explosionRange / 2f, effect.transform.localScale.z);
        Destroy(effect, 1f);

        entitySFX.Shatter();

        StartCoroutine(Shaker());
    }

    public IEnumerator Timer()
    {
        elapsedTime = 0f;

        while (elapsedTime < crystalLifetime)
        {
            float timeLeft = crystalLifetime - elapsedTime;
            elapsedTime += Time.deltaTime;

            if (skillManager.other2 != null)
            {
                slider = skillManager.other2.GetComponent<Slider>();
                slider.value = timeLeft / crystalLifetime;
            }

            if(skillManager.other != null)
            {
                skillManager.other.GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(timeLeft).ToString() + "s";
            }

            if(currentCrystal != null && currentCrystal.GetComponent<Crystal>().Timer != null)
            {
                currentCrystal.GetComponent<Crystal>().Timer.value = timeLeft / crystalLifetime;
            }

            yield return null;
        }
    }

    IEnumerator Shaker()
    {
        yield return new WaitForSeconds(0.12f);
        HitStop.Instance.Stop(0.15f);
        CameraFollow.Instance.TriggerShake(0.5f, 0.8f);
    }

    private void InstantiateBook()
    {
        var bookObj = Instantiate(ShatterdashBookImagePrefab);
        bookObj.GetComponent<SkillsBookButton>().mainSkillScript = this.gameObject;
        bookObj.GetComponent<SkillsBookButton>().infoPrefab = ShatterdashBookInfoPrefab;
        Book.Instance.PlaceNextObject(bookObj);
    }

}
