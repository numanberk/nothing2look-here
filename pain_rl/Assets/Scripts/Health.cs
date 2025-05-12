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
    [SerializeField] public Slider healthSlider; //sadece önemi varsa
    [SerializeField] public GameObject FloatingHitTextPrefab;

    [Header("Values")]
    [SerializeField] public int maxHealth;
    [SerializeField] int currentHealth;

    [Header("Events")]
    public UnityEvent OnHit;
    public UnityEvent OnDeath;
    public static System.Action OnEnemyDied;

    [Header("DONT TOUCH")]
    private ChainOnEnemy chain;
    private int lastHitTaken;
    public bool lastHitTakenWasCrit;
    public bool dead = false;
    private Animator anim;
    private PlayerPain playerPain;
    public Canvas canvas;
    public RectTransform canvasRT;
    private bool originalHit;
    private GameObject sprites;
    private GameObject canvasHP;
    private GameObject canvasExclamation;
    private ChaserEnemyAI enemyAI1;
    private ShooterEnemyAI enemyAI2;
    private DasherEnemyAI enemyAI3;
    public bool isInvulnerable;
    private float lastDamageTime;
    private float damageCooldown = 0.15f;
    private bool hasGivenCharge = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        playerPain = GetComponent<PlayerPain>();
        currentHealth = maxHealth;
        isInvulnerable = false;

        Transform spritesTransform = this.gameObject.transform.Find("Sprites");
        if (spritesTransform != null)
        {
            sprites = spritesTransform.gameObject;
        }
        canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvasRT = canvas.gameObject.GetComponent<RectTransform>();
            canvasHP = canvas.transform.Find("HealthSlider")?.gameObject;
            canvasExclamation = canvas.transform.Find("!!!")?.gameObject;
        }

        enemyAI1 = GetComponent<ChaserEnemyAI>();
        enemyAI2 = GetComponent<ShooterEnemyAI>();
        enemyAI3 = GetComponent<DasherEnemyAI>();



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


        //GEÇÝCÝ KOD//
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
        //GEÇÝCÝ KOD//

        if(dead)
        {
            var rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
            }

            if(chain != null)
            {
                ChainSkill.instance.RemoveObject(this.gameObject);
                Destroy(chain.gameObject);
            }

        }

        if (!hasGivenCharge)
        {
            // Check if both conditions are met
            if (PlayerAttack.Instance.punch && Punch.Instance.isCharged)
            {
                // Optional safety: prevent double subscription just in case
                OnEnemyDied -= Punch.Instance.AddChargeWithKill;
                OnEnemyDied += Punch.Instance.AddChargeWithKill;

                hasGivenCharge = true;
                Debug.Log("Successfully subscribed to OnEnemyDied");
            }
        }

    }

    public void Hit(int damage, int source)
    {

        if(!isInvulnerable)
        {
            if (ShatterdashSkill.Instance != null && ShatterdashSkill.Instance.isDashing)
            {
                 if(Time.time < lastDamageTime + damageCooldown)
                {
                    return;
                }
            }
            

            lastDamageTime = Time.time;
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
                //start metodunda max value'yu max health yapmamýþ olsak eþitliðin sað tarafý = currentHealth/maxHealth olurdu.
            }



            if (source != -1 && chain != null) //-1, düþmanlarýn kendine sektirmesi için olan source kodu.
            {
                chain.skill.DamageDistribution(damage, this.gameObject);
            }





            //SADECE PLAYER VARSA GEÇERLÝ - playerPain assignlanmýþ olmalý!!!
            if (playerPain != null && currentHealth > 0 && !PainMeter.Instance.isFull && playerPain.canFillPain)
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
                playerPain.latestSource = source;



            }

            StartCoroutine(OnHitDelayer());

        }


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
            int count = canvasRT.GetComponentsInChildren<FHT_Check>().Length;

            FHT_Check[] checks = canvasRT.GetComponentsInChildren<FHT_Check>();
            if (checks.Length >= 4)
            {
                foreach (FHT_Check check in checks)
                {
                    Destroy(check.gameObject.GetComponent<FHT_Check>());
                }
            }


            go.GetComponent<FloatingHitText>().newY = go.GetComponent<FloatingHitText>().originalY + count * 3; //original = 5, count = 1 iken 8 ........ count = 2 iken 11;


            //SCALE MULTIPLIER
            float damageMultiplierPain = 1 + (PainMeter.Instance.painMeter.value / PainMeter.Instance.painMeter.maxValue);
            float finalToCalculate = PlayerAttack.Instance.attackDamage * damageMultiplierPain; //FINAL TO CALCULATE, PLAYER ATTACK'TAKÝ RENGÝ ETKÝLEYEN DEÐÝÞKENLER SONRASINDA NE ÝLE ÇARPILIYORSA BASE ATK DAMAGE'Ý ONUNLA ÇARPMALI.

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
        EventManager.Instance.isPlayerDead = true;
        Destroy(_gameObject);
    }

    public void ObjectDie(GameObject gameObject1)
    {
        StartCoroutine(Destroyer(gameObject1));

        if(gameObject1.CompareTag("Enemy"))
        {
            OnEnemyDied?.Invoke();
        }

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

        var audiosource = GetComponent<AudioSource>();
        if(audiosource != null)
        {
            audiosource.enabled = false;
        }

        if(healthSlider != null)
        {
            healthSlider.enabled = false;
        }

        if(canvas != null)
        {
            Destroy(canvasHP);
            Destroy(canvasExclamation);

            if (enemyAI3 != null)
            {
                Destroy(enemyAI3.Stun);
            }
        }



        yield return new WaitForSeconds(0.15f);

        if(chain != null)
        {
            ChainSkill.instance.RemoveObject(this.gameObject);
            Destroy(chain);
        }


        yield return new WaitForSeconds(1f);

        Destroy(go);
    }
}








    
