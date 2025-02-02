using UnityEngine;

public class EventManager : MonoBehaviour
{
    public bool isPlayerDead;

    private void Start()
    {
        isPlayerDead = false;
    }
    public void PlayerDie(GameObject _gameObject)
    {
        Debug.Log("öldü.");
        isPlayerDead = true;
        //Destroy(_gameObject);
    }

    public void BarrelDie(GameObject gameObject1)
    {
        Debug.Log("barrell öldü.");
        Destroy(gameObject1);
    }

}
