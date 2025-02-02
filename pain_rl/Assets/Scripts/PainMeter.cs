using UnityEngine;
using UnityEngine.UI;

public class PainMeter : MonoBehaviour
{
    [SerializeField] private Slider painMeter;
    [SerializeField] private float lerpSpeed = 5f;
    private EventManager eventManager;
    private Color targetColor;
    private Image PM_Fill;
    private float color1, color2, color3;

    private void Start()
    {
        PM_Fill = painMeter.fillRect.GetComponent<Image>();
        eventManager = Object.FindFirstObjectByType<EventManager>();
        painMeter.maxValue = 50;
        painMeter.value = 0;
        targetColor = PM_Fill.color; // Initialize target color
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
            PM_Fill.color = Color.Lerp(PM_Fill.color, targetColor, Time.deltaTime * lerpSpeed);
        }
        else if(!eventManager.isPlayerDead && PlayerPain.Instance.totalPain <= 0)
        {
            PM_Fill.color = new Color(color1, color2, color3, 1);
        }

            ColorCalculate();
        
    }

    public void MeterFill()
    {
        if (!eventManager.isPlayerDead)
        {
            painMeter.value += PlayerPain.Instance.damage;
        }
    }

    private void ColorCalculate()
    {
        if(PlayerPain.Instance.totalPain > 0)
        {
            color1 = PlayerPain.Instance.sourceDamage1 / PlayerPain.Instance.totalPain;
            color2 = PlayerPain.Instance.sourceDamage2 / PlayerPain.Instance.totalPain;
            color3 = PlayerPain.Instance.sourceDamage3 / PlayerPain.Instance.totalPain;
        }
        else
        {
            color1 = 1;
            color2 = 1;
            color3 = 1;
        }


        // Set the new target color
        targetColor = new Color(color1, color2, color3, 1);
    }
}
