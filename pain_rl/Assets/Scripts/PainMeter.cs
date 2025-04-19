using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PainMeter : MonoBehaviour
{
    public static PainMeter Instance;

    [Header("Changable Values")]
    [SerializeField] public float meterMax;
    [SerializeField] private float lerpSpeed = 3f; //color için
    [SerializeField] private float lerpStopThreshold; //bir üst birler basamaðýna yuvarlanmadan önceki son eþik (0.5 verirsen 49.5 -> 50 insta olur.)


    [Header("Text Infos")]
    [SerializeField] private TextMeshProUGUI redValue;
    [SerializeField] private TextMeshProUGUI greenValue;
    [SerializeField] private TextMeshProUGUI blueValue;
    [SerializeField] private TextMeshProUGUI damageTakenText;
    [SerializeField] private TextMeshProUGUI maxPainText;

    [Header("DONT TOUCH")]
    public Slider painMeter;
    public float targetValue;
    [SerializeField] public GameObject Container_INT;
    [SerializeField] public GameObject FillArea;
    [SerializeField] public GameObject Fill;
    [SerializeField] public GameObject Lock;
    [SerializeField] public GameObject Percent;
    [SerializeField] public TextMeshProUGUI PercentValue;
    public bool isFull;


    private EventManager eventManager;
    private Color targetColor;
    private Image PM_Fill;
    private float color1, color2, color3;
    private Animator anim;


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        painMeter = GetComponent<Slider>();
        anim = GetComponent<Animator>();
        PM_Fill = painMeter.fillRect.GetComponent<Image>();
        eventManager = Object.FindFirstObjectByType<EventManager>();
        painMeter.maxValue = meterMax;
        painMeter.value = 0;
        targetColor = PM_Fill.color; // Initialize target color
        targetValue = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            painMeter.value = 0;
            PlayerPain.Instance.sourceDamage1 = 0;
            PlayerPain.Instance.sourceDamage2 = 0;
            PlayerPain.Instance.sourceDamage3 = 0;
        }

        if (!eventManager.isPlayerDead && PlayerPain.Instance.totalPain > 0)
        {
            // Gradually change the color over time
            PM_Fill.color = Color.Lerp(PM_Fill.color, targetColor, Time.deltaTime * lerpSpeed); //LERP gerektirenler UPDATE'de olmak zorunda!!!
        }
        else if(!eventManager.isPlayerDead && PlayerPain.Instance.totalPain <= 0)
        {
            PM_Fill.color = new Color(color1, color2, color3);
        }
            
        if(targetValue <= painMeter.maxValue)
        {
            ColorCalculate();
        }

        if(painMeter.value < painMeter.maxValue)
        {
            painMeter.value = Mathf.Lerp(painMeter.value, targetValue, Time.deltaTime * lerpSpeed); //LERP gerektirenler UPDATE'de olmak zorunda!!!
        }
           

        if (painMeter.value > painMeter.maxValue - lerpStopThreshold)
        {
            Container_INT.GetComponent<Animator>().SetBool("isFull", true);
            anim.SetBool("isFull", true);
            Fill.GetComponent<Animator>().SetBool("isFull", true);
            isFull = true;
        }
        else
        {
            Container_INT.GetComponent<Animator>().SetBool("isFull", false);
            anim.SetBool("isFull", false);
            Fill.GetComponent<Animator>().SetBool("isFull", false);
            isFull = false;
        }

            float scaleFactor = Mathf.Lerp(0.03f, 0.25f, painMeter.value / painMeter.maxValue); //Fill'in gözüktüðü dalgalarýn boyutu için 0.02 ve 0.3 deðiþtirilebilir.
            FillArea.transform.localScale = new Vector3(scaleFactor, 1f, 1f);
            PercentValue.text = Mathf.RoundToInt(painMeter.value/painMeter.maxValue*100).ToString();
            
            
            damageTakenText.text = Mathf.RoundToInt(painMeter.value).ToString();
            maxPainText.text = painMeter.maxValue.ToString();

        if(!PlayerPain.Instance.canFillPain)
        {
            Lock.SetActive(true);
            Percent.SetActive(false);
        }
        else
        {
            Lock.SetActive(false);
            Percent.SetActive(true);
        }

    }

    private void ColorCalculate()
    {
        if(PlayerPain.Instance.totalPain > 0 && painMeter.value <= painMeter.maxValue)  //ÖNEMLÝ!!! PAINMETER dolduktan sonra alýnan hasarlar renk karýþýmýna katýlsýn mý?!!!
        {
            color1 = PlayerPain.Instance.sourceDamage1 / PlayerPain.Instance.totalPain;
            color2 = PlayerPain.Instance.sourceDamage2 / PlayerPain.Instance.totalPain;
            color3 = PlayerPain.Instance.sourceDamage3 / PlayerPain.Instance.totalPain;

            redValue.text = Mathf.RoundToInt(PlayerPain.Instance.sourceDamage1 / PlayerPain.Instance.totalPain * 100) + "%".ToString();
            greenValue.text = Mathf.RoundToInt(PlayerPain.Instance.sourceDamage2 / PlayerPain.Instance.totalPain * 100) + "%".ToString();
            blueValue.text = Mathf.RoundToInt(PlayerPain.Instance.sourceDamage3 / PlayerPain.Instance.totalPain * 100) + "%".ToString();
        }
        else if (PlayerPain.Instance.totalPain <= 0)
        {
            color1 = 1;
            color2 = 1;
            color3 = 1;

            redValue.text = "0%";
            greenValue.text = "0%";
            blueValue.text = "0%";
        }


        // Set the new target color
        targetColor = new Color(color1, color2, color3);
    }
}
