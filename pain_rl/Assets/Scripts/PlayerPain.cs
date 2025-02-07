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

    private Health healthScript;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        healthScript = GetComponent<Health>();
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
        if(!EventManager.Instance.isPlayerDead)
        {
            totalPain = sourceDamage1 + sourceDamage2 + sourceDamage3;
            PainMeter.Instance.targetValue = Mathf.Clamp(totalPain, 0, PainMeter.Instance.painMeter.maxValue);
        }
        

        
    }

}
