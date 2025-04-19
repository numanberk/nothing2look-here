using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{


    public static EventManager Instance;
    public bool isPlayerDead;
    public UnityEvent PressedTAB;
    public UnityEvent PressedF1;
    public UnityEvent LetGoF1;
    //private bool F1;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //F1 = false;
        isPlayerDead = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PressedTAB?.Invoke();
        }


        if (Input.GetKeyDown(KeyCode.F1))
        {
            PressedF1?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.F1))
        {
            LetGoF1?.Invoke();
        }


    }





}
