using System.Collections;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ExplosionSkill : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float thresholdPainToUse;
    [SerializeField] private float baseRange;
    [SerializeField] private float maxRange;
    [SerializeField] private float furthestDistanceDmgAmplifier;
    [SerializeField] private float closestDistanceDmgAmplifier;
    [Header("Assign")]
    [SerializeField] private GameObject ExplosionPrefab;
    [SerializeField] private GameObject ExplosionSkillUIPrefab;
    [SerializeField] private GameObject ExplosionF1UIPrefab;
    [SerializeField] private GameObject ExplosionBookImagePrefab;
    [SerializeField] private GameObject ExplosionBookInfoPrefab;
    [SerializeField] private GameObject ExplosionParticlePrefab;

    [Header("DONT TOUCH")]
    public GameObject skillUI;
    public GameObject skillInfos;
    public SkillManager skillManager;
    private EntitySFX entitySFX;
    public GameObject Player;
    private Coroutine coroutine;
    private PlayerPain pain;
    public int currentSrc1;
    public int currentSrc2;
    public int currentSrc3;
    private bool running;
    public float decreaseRate;
    private bool firstTime = true;
    private float number;
    public float powerGathered;
    public int baseDamage;
    private Slider slider;
    private bool hasInstantiatedBook;
    private void Awake()
    {
        entitySFX = GetComponent<EntitySFX>();
    }
    private void Start()
    {
        Player = GameObject.Find("Player");
        skillUI = GameObject.Find("SkillsUI");
        skillInfos = GameObject.Find("Skill Infos");
        skillManager = GetComponent<SkillManager>();
        pain = Player.GetComponent<PlayerPain>();
        running = false;
        number = thresholdPainToUse * 0.01f - 0.005f;
        skillManager.endableSkill = true;
        hasInstantiatedBook = false;
    }

    private void Update()
    {
        if(PainMeter.Instance.painMeter.value >= PainMeter.Instance.painMeter.maxValue * number)
        {
            if(!skillManager.requirementsMetForSkill)
            {
                skillManager.BackToCooldown();
            }
            skillManager.requirementsMetForSkill = true;
        }
        else
        {
            skillManager.requirementsMetForSkill = false;
        }

        if(PainMeter.Instance.painMeter.value < 0.5f && skillManager.isRunning)
        {
            SkillEnd();
        }

        if(skillManager.secondary != null && firstTime)
        {
            firstTime = false;
            skillManager.secondary.text = (thresholdPainToUse.ToString() + "%");
        }


        if (skillManager.other2 != null && skillManager.other != null)
        {
            slider = skillManager.other2.GetComponent<Slider>();
            float targetValue = powerGathered / PainMeter.Instance.painMeter.maxValue;
            slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * 2f); // 10f is the lerp speed

            skillManager.other.GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(slider.value * 100f).ToString() + "%";
        }

        if (!hasInstantiatedBook && Book.Instance != null)
        {
            InstantiateBook();
            hasInstantiatedBook = true;
        }


    }

    public void SpawnUI()
    {
        GameObject newUI = Instantiate(ExplosionSkillUIPrefab, skillUI.transform);
        newUI.transform.localPosition = Vector3.zero; // Adjust if necessary
        skillManager.SpawnedUI = true;
        skillManager.currentUI = newUI;

        GameObject newF1 = Instantiate(ExplosionF1UIPrefab, skillManager.row.transform);
        newF1.transform.localPosition = Vector3.zero;
        skillManager.SpawnedF1 = true;
        skillManager.currentF1 = newF1;
    }

    public void SkillStart()
    {
        skillManager.canGoToCooldown = false;
        skillManager.isRunning = true;
        running = true;
        slider.value = 0;

        pain.canFillPain = false;
        powerGathered = 0;

        if(coroutine == null)
        {
            coroutine = StartCoroutine(PainDecrease()); 
        }

        entitySFX.AnnihilationCharge();

    }

    public void SkillEnd()
    {
        skillManager.BackToCooldown();
        running = false;
        StopCoroutine(DecreaseRate());


        Player.GetComponent<PlayerPain>().canFillPain = true;

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        InstantiateExplosion(powerGathered);
    }

    IEnumerator PainDecrease()
    {
        currentSrc1 = pain.sourceDamage1;
        currentSrc2 = pain.sourceDamage2;
        currentSrc3 = pain.sourceDamage3;
        StartCoroutine(DecreaseRate());

        while(running)
        {
            int decrease1 = Mathf.Max(1, Mathf.RoundToInt(pain.sourceDamage1 * 0.03f));
            int decrease2 = Mathf.Max(1, Mathf.RoundToInt(pain.sourceDamage2 * 0.03f));
            int decrease3 = Mathf.Max(1, Mathf.RoundToInt(pain.sourceDamage3 * 0.03f));

            if (pain.sourceDamage1 >= decrease1)
            {
                pain.sourceDamage1 -= decrease1;
                powerGathered += decrease1;
            }
            else
            {
                pain.sourceDamage1 = 0;
            }

            if (pain.sourceDamage2 >= decrease2)
            {
                pain.sourceDamage2 -= decrease2;
                powerGathered += decrease2;
            }
            else
            {
                pain.sourceDamage2 = 0;
            }

            if (pain.sourceDamage3 >= decrease3)
            {
                pain.sourceDamage3 -= decrease3;
                powerGathered += decrease3;
            }
            else
            {
                pain.sourceDamage3 = 0;
            }

            
            yield return new WaitForSeconds(decreaseRate);
        }


    }

    IEnumerator DecreaseRate()
    {
        decreaseRate = 1f;
        float minRate = 0.06f;    // Minimum allowed rate
        float rateDecreaseAmount = 0.02f; // How much it decreases by each time
        float rateDecreaseInterval = 0.065f; // How often it decreases (every second)


        while (running)
        {
            if (decreaseRate > minRate)
            {
                decreaseRate -= rateDecreaseAmount;
                decreaseRate = Mathf.Max(decreaseRate, minRate); // Clamp to prevent going below minRate
            }

            yield return new WaitForSeconds(rateDecreaseInterval);
        }
    }

    public void InstantiateExplosion(float powerGathered)
    {
        baseDamage = Mathf.RoundToInt(PlayerAttack.Instance.attackDamage * PlayerAttack.Instance.weaponSkillDamageMultiplier);

        float amplifier = 1 + powerGathered * 0.01f;
        float explosionDMG = baseDamage * Mathf.Pow(amplifier, 4f);

        float t = Mathf.Clamp01(amplifier - 1f);
        float rangeScale = Mathf.Lerp(baseRange, maxRange, t);

        GameObject explosion = Instantiate(ExplosionPrefab, Player.transform.position, Quaternion.identity);
        explosion.transform.localScale = Vector3.one * rangeScale;

        float hitStopTime = 0.3f * amplifier;

        ExplosionItself script = explosion.GetComponent<ExplosionItself>();
        if (script != null)
        {
            script.Setup(explosionDMG, rangeScale, furthestDistanceDmgAmplifier, closestDistanceDmgAmplifier, hitStopTime);
        }

        GameObject effect = Instantiate(ExplosionParticlePrefab, explosion.transform.position, Quaternion.identity);
        effect.transform.localScale *= rangeScale;
        Destroy(effect, 1f);

        entitySFX.audioSource.volume *= rangeScale;
        entitySFX.AnnihilationGo();

        CameraFollow.Instance.TriggerShake(0.3f+0.53f*amplifier, amplifier*1.3f);
    }


    private void InstantiateBook()
    {
        var bookObj = Instantiate(ExplosionBookImagePrefab);
        bookObj.GetComponent<SkillsBookButton>().mainSkillScript = this.gameObject;
        bookObj.GetComponent<SkillsBookButton>().infoPrefab = ExplosionBookInfoPrefab;
        Book.Instance.PlaceNextObject(bookObj);
    }


}
