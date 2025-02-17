using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] TextMeshProUGUI healthText; //sadece önemi varsa
    [SerializeField] Slider healthSlider; //sadece önemi varsa
    [SerializeField] public GameObject FloatingHitTextPrefab;

    [Header("Values")]
    [SerializeField] public int maxHealth;
    [SerializeField] int currentHealth;

    [Header("Events")]
    public UnityEvent OnHit;
    public UnityEvent OnDeath;

    private int lastHitTaken;
    private bool dead = false;
    private Animator anim;
    private PlayerPain playerPain;
    public Canvas canvas;
    public RectTransform canvasRT;

    private void Start()
    {
        anim = GetComponent<Animator>();
        playerPain = GetComponent<PlayerPain>();
        currentHealth = maxHealth;
        canvas = GetComponentInChildren<Canvas>();
        if(canvas != null )
        {
            canvasRT = canvas.gameObject.GetComponent<RectTransform>();
        }

        

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


                 //GEÇÝCÝ KOD//
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
        //GEÇÝCÝ KOD//

    }

    public void Hit(int damage, int source)
    {
        lastHitTaken = damage;
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
            //start metodunda max value'yu max health yapmamýþ olsak eþitliðin sað tarafý = currentHealth/maxHealth olurdu.
        }






        //SADECE PLAYER VARSA GEÇERLÝ - playerPain assignlanmýþ olmalý!!!
        if (playerPain != null && currentHealth > 0 && PainMeter.Instance.painMeter.value != PainMeter.Instance.painMeter.maxValue)
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

        OnHit?.Invoke();


    }

    void Die()
    {
        dead = true;
        OnDeath?.Invoke();
    }

    public void FloatingHitText()
    {
        if(FloatingHitTextPrefab != null)
        {
            Vector2 newPos;
            newPos.x = canvasRT.transform.position.x;
            newPos.y = canvasRT.transform.position.y;


            var go = Instantiate(FloatingHitTextPrefab, newPos, Quaternion.identity, canvas.transform);


            //SCALE MULTIPLIER
            float damageMultiplierPain = 1 + (PainMeter.Instance.painMeter.value / PainMeter.Instance.painMeter.maxValue);
            float finalToCalculate = PlayerAttack.Instance.attackDamage * damageMultiplierPain; //FINAL TO CALCULATE, PLAYER ATTACK'TAKÝ RENGÝ ETKÝLEYEN DEÐÝÞKENLER SONRASINDA NE ÝLE ÇARPILIYORSA BASE ATK DAMAGE'Ý ONUNLA ÇARPMALI.

            go.GetComponent<FloatingHitText>().scaleMultiplier = (float)lastHitTaken / finalToCalculate;
            go.GetComponent<FloatingHitText>().scaleMultiplier = Mathf.Clamp(go.GetComponent<FloatingHitText>().scaleMultiplier, 1f, 1.8f);

            //COLOR
            float colorFloat = (float)lastHitTaken / finalToCalculate;
            colorFloat = Mathf.Clamp(colorFloat, 1f, 2f);
            go.GetComponent<TextMeshProUGUI>().color = new Color(1f,2 - colorFloat ,0f);

            //TEXT
            if(colorFloat >= 2f)
            {
                go.GetComponent<TextMeshProUGUI>().text = lastHitTaken.ToString() + "!";
                Sword.instance.impactEffectToShow = Sword.instance.impactEffectRED;
            }
            else
            {
                go.GetComponent<TextMeshProUGUI>().text = lastHitTaken.ToString();
                Sword.instance.impactEffectToShow = Sword.instance.impactEffect;
            }

        }

    }


    public void PlayerDie(GameObject _gameObject)
    {
        Debug.Log("öldü.");
        EventManager.Instance.isPlayerDead = true;
        Destroy(_gameObject);
    }

    public void BarrelDie(GameObject gameObject1)
    {
        Destroy(gameObject1);
    }

}
