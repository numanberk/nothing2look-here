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
        Debug.Log("�ld�.");
        isPlayerDead = true;
        //Destroy(_gameObject);
    }

}
