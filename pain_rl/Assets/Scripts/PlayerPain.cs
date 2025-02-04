using UnityEngine;
using UnityEngine.Events;

public class PlayerPain : MonoBehaviour
{
    public static PlayerPain Instance;
    public int sourceDamage1;
    public int sourceDamage2;
    public int sourceDamage3;
    public float totalPain;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        totalPain = 0;
        sourceDamage1 = 0;
        sourceDamage2 = 0;
        sourceDamage3 = 0;
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
