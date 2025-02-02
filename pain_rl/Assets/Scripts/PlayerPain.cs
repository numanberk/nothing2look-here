using UnityEngine;
using UnityEngine.Events;

public class PlayerPain : MonoBehaviour
{
    public static PlayerPain Instance;
    private EventManager eventManager;
    public int damage;
    public int sourceDamage1;
    public int sourceDamage2;
    public int sourceDamage3;
    public float totalPain;
    public UnityEvent painGain;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        eventManager = Object.FindFirstObjectByType<EventManager>();
    }

    private void Update()
    {
        if(!eventManager.isPlayerDead)
        {
            totalPain = sourceDamage1 + sourceDamage2 + sourceDamage3;
        }
        
    }

}
