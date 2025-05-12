using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPain : MonoBehaviour
{
    public static PlayerPain Instance;

    [SerializeField] private TextMeshProUGUI maxHealthPlayer;


    [Space]

    [Header("DONT TOUCH")]
    public int sourceDamage1;
    public int sourceDamage2;
    public int sourceDamage3;
    public float totalPain;
    public bool canFillPain;
    public int latestSource;
    private GameObject Player;

    private Health healthScript;

    void Awake()
    {
        if (Instance == null && CompareTag("Player"))
        {
            Instance = this;
            Debug.Log("PlayerPain.Instance assigned to: " + gameObject.name);
        }
        else if (Instance != null && this != Instance)
        {
            Debug.Log("Prevented clone from overriding PlayerPain.Instance: " + gameObject.name);
        }
    }

    private void Start()
    {
        Player = this.gameObject;
        healthScript = GetComponent<Health>();
        canFillPain = true;
        totalPain = 0;
        sourceDamage1 = 0;
        sourceDamage2 = 0;
        sourceDamage3 = 0;


        if (healthScript != null)
        {
            maxHealthPlayer.text = healthScript.maxHealth.ToString(); //eðer oyun içinde max health deðiþebilecekse bunu update'e atmak gerekebilir. þu anda optimizasyon için start'ta.
        }
    }

    private void Update()
    {
        if (!EventManager.Instance.isPlayerDead)
        {
            // Clamp to prevent negatives
            sourceDamage1 = Mathf.Max(sourceDamage1, 0);
            sourceDamage2 = Mathf.Max(sourceDamage2, 0);
            sourceDamage3 = Mathf.Max(sourceDamage3, 0);

            totalPain = Mathf.Clamp(sourceDamage1 + sourceDamage2 + sourceDamage3, 0, PainMeter.Instance.painMeter.maxValue);
            PainMeter.Instance.targetValue = Mathf.Clamp(totalPain, 0, PainMeter.Instance.painMeter.maxValue);

            if(sourceDamage1 + sourceDamage2 + sourceDamage3 > PainMeter.Instance.painMeter.maxValue)
            {
                int over100percent = Mathf.RoundToInt(sourceDamage1 + sourceDamage2 + sourceDamage3 - PainMeter.Instance.painMeter.maxValue);
                if(latestSource == 1)
                {
                    sourceDamage1 -= over100percent;
                }

                if(latestSource == 2)
                {
                    sourceDamage2 -= over100percent;
                }

                if(latestSource == 3)
                {
                    sourceDamage3 -= over100percent;
                }
            }
        }
    }

    public void ShatterdashOnHit()
    {
        if(ShatterdashSkill.Instance != null && !ShatterdashSkill.Instance.skillManager.isInCooldown && !ShatterdashSkill.Instance.isDashing)
        {
            InstantiateCrystal();
        }
    }

    public void InstantiateCrystal()
    {
        if (ShatterdashSkill.Instance.currentCrystal != null)
        {
            Destroy(ShatterdashSkill.Instance.currentCrystal);

            if (ShatterdashSkill.Instance.timer != null)
            {
                StopCoroutine(ShatterdashSkill.Instance.timer);
                ShatterdashSkill.Instance.timer = null;
            }
        }

        ShatterdashSkill.Instance.currentCrystal = Instantiate(ShatterdashSkill.Instance.CrystalPrefab, Player.transform.position, Quaternion.identity);
        ShatterdashSkill.Instance.currentCrystal.GetComponent<Lifetime>().Setup(ShatterdashSkill.Instance.crystalLifetime);

        ShatterdashSkill.Instance.timer = StartCoroutine(ShatterdashSkill.Instance.Timer());
    }



}
