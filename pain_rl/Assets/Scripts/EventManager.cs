using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    public bool isPlayerDead;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        isPlayerDead = false;
    }
    public void PlayerDie(GameObject _gameObject)
    {
        Debug.Log("�ld�.");
        isPlayerDead = true;
        Destroy(_gameObject);
    }

    public void BarrelDie(GameObject gameObject1)
    {
        Debug.Log("barrell �ld�.");
        Destroy(gameObject1);
    }

}
