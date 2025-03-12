using UnityEngine;

public class Barrel : MonoBehaviour
{

    private Health health;
    private bool firstHit = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = GetComponent<Health>();
        health.healthSlider.enabled = false;
        health.healthSlider.gameObject.SetActive(false);
    }

    public void EnableSlider()
    {
        if(health.healthSlider.enabled == false && !firstHit)
        {
            health.healthSlider.enabled = true;
            health.healthSlider.gameObject.SetActive(true);
            firstHit = true;
        }

    }
}
