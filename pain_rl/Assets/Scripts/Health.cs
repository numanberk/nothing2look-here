using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] TextMeshProUGUI healthText; //sadece �nemi varsa
    [SerializeField] Slider healthSlider; //sadece �nemi varsa

    [Header("Values")]
    [SerializeField] public int maxHealth;
    [SerializeField] int currentHealth;

    [Header("Events")]
    public UnityEvent OnDeath;

    private bool dead = false;
    private Animator anim;
    private PlayerPain playerPain;

    private void Start()
    {
        anim = GetComponent<Animator>();
        playerPain = GetComponent<PlayerPain>();
        currentHealth = maxHealth;
        

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth; 
        }
        if(healthText != null)
        {
            healthText.text = (currentHealth).ToString();
        }
    }

    private void Update()
    {
        if (currentHealth <= 0 && !dead)
        {
            Die();
        }


                 //GE��C� KOD//
        if(Input.GetKeyDown(KeyCode.Space) && playerPain != null)
        {
            Hit(10, 1);
        }

        if (Input.GetKeyDown(KeyCode.K) && playerPain != null)
        {
            Hit(10, 2);
        }

        if (Input.GetKeyDown(KeyCode.L) && playerPain != null)
        {
            Hit(10, 3);
        }
        //GE��C� KOD//

    }

    public void Hit(int damage, int source)
    {

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (anim != null)
        {
            anim.SetTrigger("Hit");
        }
        else
        {
            Debug.Log("anim null");
        }

        if(healthText != null)
        {
            healthText.text = (currentHealth).ToString();
        }

        if(healthSlider != null)
        {
            healthSlider.value = currentHealth;
            //start metodunda max value'yu max health yapmam�� olsak e�itli�in sa� taraf� = currentHealth/maxHealth olurdu.
        }




        //SADECE PLAYER VARSA GE�ERL� - playerPain assignlanm�� olmal�!!!
        if(playerPain != null && currentHealth > 0 && PainMeter.Instance.painMeter.value != PainMeter.Instance.painMeter.maxValue)
        {

            if(source == 1)
            {
                playerPain.sourceDamage1 += damage;
            }

            if (source == 2)
            {
                playerPain.sourceDamage2 += damage;
            }

            if (source == 3)
            {
                playerPain.sourceDamage3 += damage;
            }

            PainMeter.Instance.Container_INT.GetComponent<Animator>().SetTrigger("Filling");
            
        }


    }

    void Die()
    {
        dead = true;
        OnDeath?.Invoke();
    }
}
