using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    public bool isPlayerDead;
    public UnityEvent PressedTAB;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        isPlayerDead = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PressedTAB?.Invoke();
        }
    }





}
