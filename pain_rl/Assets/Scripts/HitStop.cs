using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    public static HitStop Instance;

    bool waiting;
    private void Awake()
    {
        Instance = this;
    }

    public void Stop(float duration)
    {
        if(waiting)
        {
            return;
        }

            Time.timeScale = 0f;
            StartCoroutine(Wait(duration));

    }

    IEnumerator Wait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        waiting = false;

    }

    public void Slow(float duration)
    {
        if (waiting)
        {
            return;
        }
            Time.timeScale = 0.1f;
            StartCoroutine(Wait(duration));


    }


}
