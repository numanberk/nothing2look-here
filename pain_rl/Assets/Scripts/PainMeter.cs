using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PainMeter : MonoBehaviour
{
    public static PainMeter Instance;

    [Header("Changable Values")]
    [SerializeField] public float meterMax;
    [SerializeField] public Slider painMeter;
    [SerializeField] public GameObject Container;
    [SerializeField] public GameObject FillArea;
    [SerializeField] public TextMeshProUGUI PercentValue;
    [SerializeField] private float lerpSpeed = 5f; //color için
    [SerializeField] private float lerpStopThreshold;

    [Header("DONT TOUCH")]
    public float targetValue;


    private EventManager eventManager;
    private Color targetColor;
    private Image PM_Fill;
    private float color1, color2, color3;


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
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
            PM_Fill.color = new Color(color1, color2, color3, 1);
        }
            
        if(painMeter.value <= painMeter.maxValue)
        {
            ColorCalculate();
        }

        if(painMeter.value < painMeter.maxValue)
        {
            painMeter.value = Mathf.Lerp(painMeter.value, targetValue, Time.deltaTime * lerpSpeed); //LERP gerektirenler UPDATE'de olmak zorunda!!!
        }
            
        if(painMeter.value > painMeter.maxValue - lerpStopThreshold)
        {
            painMeter.value = painMeter.maxValue;
        }    

        if(painMeter.value == painMeter.maxValue)
        {
            Container.GetComponent<Animator>().SetBool("isFull", true);
        }
        else
        {
            Container.GetComponent<Animator>().SetBool("isFull",false);
        }

            float scaleFactor = Mathf.Lerp(0.02f, 0.7f, painMeter.value / painMeter.maxValue); //Fill'in gözüktüðü dalgalarýn boyutu için 0.02 ve 0.7 deðiþtirilebilir.
            FillArea.transform.localScale = new Vector3(scaleFactor, 1f, 1f);
            PercentValue.text = Mathf.RoundToInt(painMeter.value/painMeter.maxValue*100).ToString();

    }

    private void ColorCalculate()
    {
        if(PlayerPain.Instance.totalPain > 0 && PainMeter.Instance.painMeter.value <= PainMeter.Instance.painMeter.maxValue)  //ÖNEMLÝ!!! PAINMETER dolduktan sonra alýnan hasarlar renk karýþýmýna katýlsýn mý?!!!
        {
            color1 = PlayerPain.Instance.sourceDamage1 / PlayerPain.Instance.totalPain;
            color2 = PlayerPain.Instance.sourceDamage2 / PlayerPain.Instance.totalPain;
            color3 = PlayerPain.Instance.sourceDamage3 / PlayerPain.Instance.totalPain;
        }
        else if (PlayerPain.Instance.totalPain <= 0)
        {
            color1 = 1;
            color2 = 1;
            color3 = 1;
        }


        // Set the new target color
        targetColor = new Color(color1, color2, color3, 1);
    }
}
