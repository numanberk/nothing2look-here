using System.Collections;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    private float lifetime = 10f;
    private bool destroyScheduled = false;
    public bool destroyable = true;


    private void Start()
    {
        destroyable = true;
    }
    public void Setup(float newLifetime)
    {
        lifetime = newLifetime;
        if (!destroyScheduled)
        {
            StartCoroutine(DestroyQuestion());
            destroyScheduled = true;
        }
    }

    private IEnumerator DestroyQuestion()
    {
        yield return new WaitForSeconds(lifetime);

        if (destroyable)
        {
            Destroy(gameObject);
        }
        else
        {
            // Wait until destroyable becomes true
            yield return new WaitUntil(() => destroyable == true);
            Destroy(gameObject);
        }
    }
}
