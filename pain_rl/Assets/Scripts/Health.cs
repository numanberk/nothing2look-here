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
    [SerializeField] TextMeshProUGUI healthText; //sadece �nemi varsa
    [SerializeField] public Slider healthSlider; //sadece �nemi varsa
    [SerializeField] public GameObject FloatingHitTextPrefab;

    [Header("Values")]
    [SerializeField] public int maxHealth;
    [SerializeField] int currentHealth;

    [Header("Events")]
    public UnityEvent OnHit;
    public UnityEvent OnDeath;

    [Header("DONT TOUCH")]
    private ChainOnEnemy chain;
    private int lastHitTaken;
    public bool lastHitTakenWasCrit;
    private bool dead = false;
    private Animator anim;
    private PlayerPain playerPain;
    public Canvas canvas;
    public RectTransform canvasRT;
    private bool originalHit;
    private GameObject sprites;

    private void Start()
    {
        anim = GetComponent<Animator>();
        playerPain = GetComponent<PlayerPain>();
        currentHealth = maxHealth;

        Transform spritesTransform = this.gameObject.transform.Find("Sprites");
        if (spritesTransform != null)
        {
            sprites = spritesTransform.gameObject;
        }
        canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvasRT = canvas.gameObject.GetComponent<RectTransform>();
        }



        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        if (healthText != null)
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

        if (chain == null)
        {
            chain = GetComponentInChildren<ChainOnEnemy>();
        }


        //GE��C� KOD//
        if (Input.GetKeyDown(KeyCode.Space) && playerPain != null)
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

        if (healthText != null)
        {
            healthText.text = (currentHealth).ToString();
        }

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
            //start metodunda max value'yu max health yapmam�� olsak e�itli�in sa� taraf� = currentHealth/maxHealth olurdu.
        }



        if (source != -1) //-1, d��manlar�n kendine sektirmesi i�in olan source kodu.
        {
            StartCoroutine(CheckChain(damage));
        }





        //SADECE PLAYER VARSA GE�ERL� - playerPain assignlanm�� olmal�!!!
        if (playerPain != null && currentHealth > 0 && PainMeter.Instance.painMeter.value != PainMeter.Instance.painMeter.maxValue)
        {

            if (source == 1)
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

        StartCoroutine(OnHitDelayer());


    }

    private IEnumerator OnHitDelayer()
    {
        yield return new WaitForSeconds(0.02f);
        OnHit?.Invoke();
    }
    void Die()
    {
        dead = true;
        OnDeath?.Invoke();
    }

    public void FloatingHitText()
    {
        if (FloatingHitTextPrefab != null && canvasRT != null)
        {
            Vector2 newPos;
            newPos.x = canvasRT.transform.position.x;
            newPos.y = canvasRT.transform.position.y;


            var go = Instantiate(FloatingHitTextPrefab, newPos, Quaternion.identity, canvas.transform);


            //SCALE MULTIPLIER
            float damageMultiplierPain = 1 + (PainMeter.Instance.painMeter.value / PainMeter.Instance.painMeter.maxValue);
            float finalToCalculate = PlayerAttack.Instance.attackDamage * damageMultiplierPain; //FINAL TO CALCULATE, PLAYER ATTACK'TAK� RENG� ETK�LEYEN DE���KENLER SONRASINDA NE �LE �ARPILIYORSA BASE ATK DAMAGE'� ONUNLA �ARPMALI.

            go.GetComponent<FloatingHitText>().scaleMultiplier = (float)lastHitTaken / finalToCalculate;
            go.GetComponent<FloatingHitText>().scaleMultiplier = Mathf.Clamp(go.GetComponent<FloatingHitText>().scaleMultiplier, 1f, 1.8f);

            //COLOR
            float colorFloat = (float)lastHitTaken / finalToCalculate;
            colorFloat = Mathf.Clamp(colorFloat, 1f, 2f);
            go.GetComponent<TextMeshProUGUI>().color = new Color(1f, 2 - colorFloat, 0f);
            if (!lastHitTakenWasCrit)
            {
                go.GetComponent<TextMeshProUGUI>().text = lastHitTaken.ToString();
            }
            else
            {
                go.GetComponent<TextMeshProUGUI>().text = lastHitTaken.ToString() + "!";
            }


        }

    }


    public void PlayerDie(GameObject _gameObject)
    {
        Debug.Log("�ld�.");
        EventManager.Instance.isPlayerDead = true;
        Destroy(_gameObject);
    }

    public void ObjectDie(GameObject gameObject1)
    {
        StartCoroutine(Destroyer(gameObject1));
    }

    private IEnumerator CheckChain(int damage)
    {
        yield return new WaitForSeconds(0.018f);
        if (chain != null)
        {
            chain.skill.DamageDistribution(damage, this.gameObject);
        }
    }

    IEnumerator Destroyer(GameObject go)
    {


        SpriteRenderer[] sr = GetComponentsInChildren<SpriteRenderer>();
        foreach (var c in sr)
        {
            c.enabled = false;
        }

        if (sprites != null)
        {
            sprites.SetActive(false);
        }


        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }

        var collider = GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        var collider2 = GetComponent<CapsuleCollider2D>();
        if (collider2 != null)
        {
            collider2.enabled = false;
        }

        if (canvas != null)
        {
            canvas.enabled = false;
        }

        yield return new WaitForSeconds(0.018f);

        Destroy(go);
    }
}








    
