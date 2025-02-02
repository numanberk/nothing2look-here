using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] TextMeshProUGUI healthText; //sadece önemi varsa
    [SerializeField] Slider healthSlider; //sadece önemi varsa

    [Header("Values")]
    [SerializeField] int maxHealth;
    [SerializeField] int currentHealth;

    [Header("Events")]
    public UnityEvent OnDeath;

    private bool dead = false;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth; 
        }
        if(healthText != null)
        {
            healthText.text = (currentHealth + "/" + maxHealth).ToString();
        }
    }

    private void Update()
    {
        if (currentHealth <= 0 && !dead)
        {
            Die();
        }


                 //GEÇÝCÝ KOD//
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Hit(10);
        }
                 //GEÇÝCÝ KOD//

    }

    void Hit(int damage)
    {

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (anim != null)
        {
            anim.SetTrigger("hit");
        }

        if(healthText != null)
        {
            healthText.text = (currentHealth + "/" + maxHealth).ToString();
        }

        if(healthSlider != null)
        {
            healthSlider.value = currentHealth;
            //start metodunda max value'yu max health yapmamýþ olsak eþitliðin sað tarafý = currentHealth/maxHealth olurdu.
        }

    }

    void Die()
    {
        dead = true;
        OnDeath?.Invoke();
    }
}
