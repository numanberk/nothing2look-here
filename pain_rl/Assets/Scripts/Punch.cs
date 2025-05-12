using DG.Tweening;
using System.Collections;
using System.Xml.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class Punch : MonoBehaviour
{
    public static Punch Instance;

    [Header("Values")]
    [SerializeField] public int attackDamage;
    [SerializeField] public float attackRangeHorizontal;
    [SerializeField] public float attackRangeVertical;
    [SerializeField] public float attackCooldown;
    [SerializeField] public float delayBetweenAttacks;
    [SerializeField] public float animationLength; //BUNU DE���T�R�RSEN 1) FAKE SWORD YOK OLUP GER�E�� GELEN AN�MASYONUN S�RES�N�    2) TRAIL EFFECT AN�MASYONULARININ S�RES�N� BUNU E��TLE AN�MAT�RDE. (sword içindi)
    [SerializeField] public int numberOfAttacks;
    [SerializeField] public float hitStopTime;
    [Space]
    [Header("Charged Values")]
    [SerializeField] public float chargedDamageMultiplier;
    [SerializeField] public int chargedNumberOfAttacks;
    [SerializeField] public float attackRangeHorizontalMultiplier;
    [SerializeField] public float addChargeEachPunch;
    [SerializeField] public float substractChargeEachPunch;
    [SerializeField] public float dashSpeed;
    [SerializeField] public float chargedHitStopTime;
    [Space]
    [Space]
    [SerializeField] public float critChance;
    [SerializeField] public float critChanceMultiplier;
    [SerializeField] public float critDamageMultiplier;
    //[SerializeField] public float comboResetTime; //BUNU DE���T�R�RSEN SWING 1 VE 2 YAPILIRKEN PLAYERDA G�Z�KEN AN�MASYONUN NORMALE D�NME S�RES�N� DE AYARLA. E�ER COMBO DEVAM EDEB�LECEK DURUMDAYSA PLAYER DAHA GERG�N? DURSUN K� COMBO NE ZAMAN B�TT� ANLA�ILSIN. + FAKE SWORD IDLE'A G�DERKEN OLAN EXIT TIMELARI DE���T�R.



    [Header("Objects")]
    [SerializeField] public Transform punchParent;
    [SerializeField] public Transform punch;
    [SerializeField] public GameObject punchObject;
    [SerializeField] public Animator punchAnimator;
    [SerializeField] public GameObject punchBookPrefab;
    [SerializeField] public GameObject punchBookPrefab2;
    [SerializeField] public GameObject impactEffect;
    [SerializeField] public GameObject impactEffectRED;
    [SerializeField] private Color punchMaxFillColor;


    [Header("Sound Effects")]
    [SerializeField] public AudioClip[] punchSFX;
    [SerializeField] public AudioClip critSFX;
    [SerializeField] public AudioClip powerChargeMAX;

    [Header("DONT TOUCH")]
    public AudioSource audioSource;
    public AudioSource audioSourceParent;
    private int lastSlashIndex = -1;
    private int lastPowerSlashIndex = -1;
    public GameObject impactEffectToShow;
    private bool hasPlayedSound;
    public Vector2 attackDir;
    private int lastQuadrant = -1;
    private const float quadrantChangeThreshold = 15f;
    [SerializeField] public float attackInterval;
    [SerializeField] public float dashDuration;
    public bool isCharged;
    public int currentNumberOfAttacks;
    private float baseCooldown;
    private float baseAnimLength;
    private Coroutine animationCoroutine;
    private bool hasInstantiatedBook;
    public GameObject instantiatedBookImage;


    private void Awake()
    {
        if (Instance == null && CompareTag("WeaponPunch"))
        {
            Instance = this;
        }
        else if (Instance != null && this != Instance)
        {
            Debug.Log("Prevented clone from overriding Punch.Instance: " + gameObject.name);
        }
    }


    private void Update()
    {
        PlayerAttack.Instance.numberOfAttacks = currentNumberOfAttacks;

        punch.position = punchParent.position;

        if (Input.GetMouseButton(0) && PlayerAttack.Instance.canAttack && !PlayerAttack.Instance.isAttacking)
        {
            PunchAttack();
        }


        if (PlayerAttack.Instance.chargeSlider.GetComponent<Slider>().value >= 1 && PlayerAttack.Instance.attackWaitingCheck)
        {
            isCharged = true;
            PlayerAttack.Instance.isCharged = true;
            if (!hasPlayedSound)
            {
                hasPlayedSound = true;
                PowerChargeMAX();
                PlayerAttack.Instance.maxText.GetComponent<Animator>().SetBool("chargeFull", true);
                PlayerAttack.Instance.chargeSliderFill.GetComponent<Image>().color = punchMaxFillColor;
            }
        }

        if (PlayerAttack.Instance.chargeSlider.GetComponent<Slider>().value <= 0 && PlayerAttack.Instance.attackWaitingCheck)
        {
            isCharged = false;
            PlayerAttack.Instance.isCharged = false;
            hasPlayedSound = false;
            PlayerAttack.Instance.maxText.GetComponent<Animator>().SetBool("chargeFull", false);
            PlayerAttack.Instance.chargeSliderFill.GetComponent<Image>().color = PlayerAttack.Instance.baseFillColor;
        }

        if(!hasInstantiatedBook && Book.Instance != null)
        {
            InstantiateBook();
            hasInstantiatedBook = true;
        }


    }
    private void Start()
    {
        impactEffectToShow = impactEffect;
        audioSourceParent = punchAnimator.gameObject.GetComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();

        PlayerAttack.Instance.punch = true;
        PlayerAttack.Instance.attackObjectParent = punchParent;
        PlayerAttack.Instance.attackObjectAnimator = punchAnimator;
        PlayerAttack.Instance.chargeSlider.GetComponent<Slider>().value = 0;
        PlayerAttack.Instance.chargeSlider.SetActive(true);
        GetBaseValues();
        baseCooldown = attackCooldown;
        baseAnimLength = animationLength;
        hasInstantiatedBook = false;

        

    }

    public void InstantiateBook()
    {
        instantiatedBookImage = Instantiate(punchBookPrefab, Book.Instance.P1LeftContainer.transform);
        Instantiate(punchBookPrefab2, Book.Instance.P1RightContainer.transform);
    }

    public void GetBaseValues()
    {
        PlayerAttack.Instance.attackDamage = attackDamage;
        PlayerAttack.Instance.attackRange = attackRangeHorizontal;
        PlayerAttack.Instance.attackRange2 = attackRangeVertical;
        PlayerAttack.Instance.damageMultiplierWeapon = 1;
        PlayerAttack.Instance.attackCooldown = attackCooldown;
        PlayerAttack.Instance.animationLength = animationLength;
        PlayerAttack.Instance.hitStopTime = hitStopTime;
        PlayerAttack.Instance.powerfulShakeMultiplier = 1f;
        PlayerAttack.Instance.critChance = critChance;
        PlayerAttack.Instance.critDamageMultiplier = critDamageMultiplier;
        currentNumberOfAttacks = numberOfAttacks;
        PlayerAttack.Instance.delayBetweenAttacks = delayBetweenAttacks;
    }

    public void GetChargedValues()
    {
        currentNumberOfAttacks = chargedNumberOfAttacks;
        var divisionCooldown = baseCooldown * chargedNumberOfAttacks / numberOfAttacks;
        var divisionAnimLength = baseAnimLength * chargedNumberOfAttacks / numberOfAttacks;
        PlayerAttack.Instance.damageMultiplierWeapon = chargedDamageMultiplier;
        PlayerAttack.Instance.attackRange = attackRangeHorizontal * attackRangeHorizontalMultiplier;
        PlayerAttack.Instance.hitStopTime = chargedHitStopTime;
        PlayerAttack.Instance.attackCooldown = divisionCooldown;
        PlayerAttack.Instance.animationLength = divisionAnimLength;
        PlayerAttack.Instance.powerfulShakeMultiplier = 1.82f;
        PlayerAttack.Instance.critChance = critChance * critChanceMultiplier;
    }


    public void PunchAttack()
    {
        if(isCharged)
        {
            GetChargedValues();
        }

        if(!isCharged)
        {
            GetBaseValues();
        }
        PlayerAttack.Instance.canAttack = false;
        PlayerAttack.Instance.isAttacking = true;
        PlayerAttack.Instance.isIdle = false;
        StartCoroutine(DamageDelayer());

        PlayerAttack.Instance.attackTimer = PlayerAttack.Instance.StartCoroutine(PlayerAttack.Instance.AttackResetPunch());



        Vector2 playerPos = (Vector2)PlayerAttack.Instance.Player.transform.position;
        Vector2 mousePos = (Vector2)Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
        attackDir = (mousePos - playerPos).normalized;
        PlayerAttack.Instance.attackDirLocked = true;

        PlayerAttack.Instance.anim.SetInteger("Direction", PlayerAttack.Instance.GetMouseQuadrant(attackDir));

        


    }

    IEnumerator Animationn()
    {
        Vector2 pivot = PlayerAttack.Instance.attackPoint.position;
        Vector2 boxSize = new Vector2(PlayerAttack.Instance.attackRange, PlayerAttack.Instance.attackRange2);
        Vector2 boxCenter = pivot + attackDir * (boxSize.x * 4 / 5);
        Vector2 boxCenterCharged = pivot + attackDir * (boxSize.x);

        PlayerAttack.Instance.attackObjectParent.position = PlayerAttack.Instance.attackPoint.transform.position;

        if(!isCharged)
        {
            if (Random.value < 0.5f)
            {
                PlayerAttack.Instance.attackObjectAnimator.SetTrigger("Punch1");
            }
            else
            {
                PlayerAttack.Instance.attackObjectAnimator.SetTrigger("Punch2");
            }

            PlayerAttack.Instance.attackObjectAnimator.speed = currentNumberOfAttacks / PlayerAttack.Instance.animationLength;
            PlayerAttack.Instance.attackObjectParent.DOMove(boxCenter, animationLength / (currentNumberOfAttacks * 2)).SetEase(Ease.OutExpo); // Move quickly to attack end point
            yield return new WaitForSeconds(animationLength / currentNumberOfAttacks);
            PlayerAttack.Instance.attackObjectParent.position = PlayerAttack.Instance.attackPoint.transform.position;
            PlayerAttack.Instance.attackObjectAnimator.speed = 1;
        }
        else
        {

            //TEK ANIM
            PlayerAttack.Instance.attackObjectAnimator.SetTrigger("ChargedPunch");

            PlayerAttack.Instance.attackObjectAnimator.speed = currentNumberOfAttacks / PlayerAttack.Instance.animationLength;
            PlayerAttack.Instance.attackObjectParent.DOMove(boxCenterCharged, animationLength / (currentNumberOfAttacks * 2)).SetEase(Ease.OutExpo); // Move quickly to attack end point
            yield return new WaitForSeconds(animationLength / currentNumberOfAttacks);
            PlayerAttack.Instance.attackObjectParent.position = PlayerAttack.Instance.attackPoint.transform.position;
            PlayerAttack.Instance.attackObjectAnimator.speed = 1;
        }



    }

    private IEnumerator DamageDelayer()
    {
        yield return new WaitForSeconds(0.08f);
        StartCoroutine(MultipleAttacks());

        //BUNU ŞARJLIYKEN SADECE İLK YUMRUKTA DEĞİL DE HER YUMRUKTA DASH YAPMAK İSTİYORSAN AŞAĞIDAKİ FONKSİYONA KES YAPIŞTIR.
        if (isCharged)
        {
            DashTowardsPunch();
        }

    }

    IEnumerator MultipleAttacks()
    {
       attackInterval = PlayerAttack.Instance.attackCooldown / currentNumberOfAttacks;
       for (int i = 0; i < currentNumberOfAttacks; i++)
       {
           PlayerAttack.OnPlayerAttackedPunch?.Invoke(isCharged);

           DamagePunch();
           PlayerAttack.Instance.anim.SetTrigger("Punch");
           PunchSFX();
           if (isCharged)
           {
                ChargeDown();
           }

            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
            animationCoroutine = StartCoroutine(Animationn());

            if (i < currentNumberOfAttacks - 1)
            yield return new WaitForSeconds(attackInterval);
       }
        PlayerAttack.Instance.attackDirLocked = false;

    }

    public void ChargeUp()
    {

            PlayerAttack.Instance.chargeSlider.GetComponent<Slider>().value += addChargeEachPunch;
    }

    public void ChargeDown()
    {

            PlayerAttack.Instance.chargeSlider.GetComponent<Slider>().value -= substractChargeEachPunch;
    }

    public void DashTowardsPunch()
    {
        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        Rigidbody2D rb = PlayerAttack.Instance.Player.GetComponent<Rigidbody2D>();
        if (rb == null) yield break;

        //PlayerMovement.instance.enabled = false; // optional: prevent movement during dash

        dashDuration = attackInterval;
        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            rb.linearVelocity = attackDir * dashSpeed;
            elapsed += Time.deltaTime;
            yield return null;
        }

        //rb.linearVelocity = Vector2.zero;
        //PlayerMovement.instance.enabled = true; // re-enable movement
    }


    private void DamagePunch()
    {



        //DAMAGE AYARLAMALARI
        float critMultiply;

        if (Random.value < PlayerAttack.Instance.critChance)
        {
            critMultiply = critDamageMultiplier;
            impactEffectToShow = impactEffectRED;
        }
        else
        {
            critMultiply = 1;
            impactEffectToShow = impactEffect;
        }


        //rengi etkileyen de�i�kenler : - crit att�k m� atmad�k m�?    - ���nc� vuru�ta m�y�z de�il miyiz? 
        //rengi etkilemeyen de�i�kenler : - pain ile gelen damage boost ne kadar fazla?
        int damageThatEffectsTheColors = Mathf.RoundToInt(attackDamage * critMultiply * PlayerAttack.Instance.damageMultiplierWeapon);
        int damageToDeal = Mathf.RoundToInt(damageThatEffectsTheColors * PlayerAttack.Instance.damageMultiplierPain);


        Vector2 boxSize = new Vector2(PlayerAttack.Instance.attackRange, PlayerAttack.Instance.attackRange2);
        Vector2 pivot = PlayerAttack.Instance.attackPoint.position;

        // Calculate box center in the direction of attackDir
        Vector2 boxCenter = pivot + attackDir * (boxSize.x / 2f); // Push box in front of attack point

        // Get angle in degrees from attackDir
        float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

        // Detect objects inside the rotated box
        Collider2D[] hitObjects = Physics2D.OverlapBoxAll(boxCenter, boxSize, angle);


        foreach (Collider2D objects in hitObjects)
        {

                if (objects.CompareTag("Enemy"))
                {
                    objects.GetComponent<Health>().Hit(damageToDeal, 0);
                    GameObject effect = Instantiate(impactEffectToShow, objects.transform.position,
                        Quaternion.Euler(0, 0, 0));
                    Destroy(effect, 1f);
                    HitStop.Instance.Stop(PlayerAttack.Instance.hitStopTime);
                    CameraFollow.Instance.TriggerShake(0.25f * PlayerAttack.Instance.powerfulShakeMultiplier, 0.6f * PlayerAttack.Instance.powerfulShakeMultiplier);
                    if (critMultiply == critDamageMultiplier)
                    {
                        CritSFX();
                    }

                    if (critMultiply == critDamageMultiplier)
                    {
                        objects.GetComponent<Health>().lastHitTakenWasCrit = true;
                    }
                    else
                    {
                        objects.GetComponent<Health>().lastHitTakenWasCrit = false;
                    }

                if (!isCharged)
                {
                    ChargeUp();
                }


            }


                else if (objects.CompareTag("Barrel"))
                {
                    objects.GetComponent<Health>().Hit(damageToDeal, 0);
                    objects.GetComponent<EntitySFX>().BarrelHitSFX();
                    GameObject effect = Instantiate(impactEffectToShow, objects.transform.position,
                        Quaternion.Euler(0, 0, 0));
                    Destroy(effect, 1f);
                    HitStop.Instance.Stop(PlayerAttack.Instance.hitStopTime);
                    CameraFollow.Instance.TriggerShake(0.24f * PlayerAttack.Instance.powerfulShakeMultiplier, 0.6f * PlayerAttack.Instance.powerfulShakeMultiplier);
                    if (critMultiply == critDamageMultiplier)
                    {
                        CritSFX();
                    }

                    if (critMultiply == critDamageMultiplier)
                    {
                        objects.GetComponent<Health>().lastHitTakenWasCrit = true;
                    }
                    else
                    {
                        objects.GetComponent<Health>().lastHitTakenWasCrit = false;
                    }

                if (!isCharged)
                {
                    ChargeUp();
                }
            }

        }
       
    }


    public void PunchSFX()
    {
        if (punchSFX.Length > 1)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, punchSFX.Length);
            } while (randomIndex == lastSlashIndex);

            lastSlashIndex = randomIndex;

            // Set random pitch
            audioSource.pitch = Random.Range(0.7f, 1.3f);

            audioSource.PlayOneShot(punchSFX[randomIndex]);
        }
    }


    public void CritSFX()
    {
        if (critSFX != null)
            audioSourceParent.PlayOneShot(critSFX);
    }

    public void PowerChargeMAX()
    {
        if (powerChargeMAX != null)
            audioSourceParent.PlayOneShot(powerChargeMAX);
    }



}