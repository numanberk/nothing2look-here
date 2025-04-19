using DG.Tweening;
using System.Collections;
using System.Xml.Linq;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class Sword : MonoBehaviour
{


    [Header("Values")]
    [SerializeField] public int attackDamage;
    [SerializeField] public float attackRange;
    [SerializeField] public float attackAngle;
    [SerializeField] public float attackCooldown;
    [SerializeField] public float critChance;
    [SerializeField] public float critDamageMultiplier;
    [SerializeField] public float animationLength; //BUNU DEĞİŞTİRİRSEN 1) FAKE SWORD YOK OLUP GERÇEĞİ GELEN ANİMASYONUN SÜRESİNİ    2) TRAIL EFFECT ANİMASYONULARININ SÜRESİNİ BUNU EŞİTLE ANİMATÖRDE.
    [SerializeField] public float comboResetTime; //BUNU DEĞİŞTİRİRSEN SWING 1 VE 2 YAPILIRKEN PLAYERDA GÖZÜKEN ANİMASYONUN NORMALE DÖNME SÜRESİNİ DE AYARLA. EĞER COMBO DEVAM EDEBİLECEK DURUMDAYSA PLAYER DAHA GERGİN? DURSUN Kİ COMBO NE ZAMAN BİTTİ ANLAŞILSIN. + FAKE SWORD IDLE'A GİDERKEN OLAN EXIT TIMELARI DEĞİŞTİR.
    [SerializeField] public float hitStopTime;

    [Header("3rd Hit Values")]
    [SerializeField] public float thirdHitDamageMultiplier;
    [SerializeField] public float thirdHitRangeMultiplier;
    [SerializeField] public float thirdHitAngle;
    [SerializeField] public float maxChargeInSeconds;  //BUNU DEĞİŞTİRİRSEN PLAYER CHARGE ANİMASYONU KAÇ SANİYE ONU DA DEĞİŞTİRMEN GEREKİR. + PLAYER ATTACK'TA BU DEĞERİN ETKİLEDİĞİ HASARIN DEĞİŞİM HIZINI DEĞİŞTİRMEN GEREKİR.
    [SerializeField] public float maxChargeDamageMultiplier;
    [Space]
    [SerializeField] public float delayBeforeThirdHit; // BUNU DEĞİŞTİRİRSEN PLAYER ANİMATÖRÜNDEKİ!!! (sword değil) SWORDATTACK3 ANİMASYONUNUN DELAY GÖSTERMELİK İLK KISMINI DA OYNATMAN GEREKİR!
    [SerializeField] public float delayAfterThirdHit; // YENİ KOMBO BAŞLATABİLMEK İÇİN GEREKLİ SÜRE. bi üstteki animasyonunun aynısının son kısmını değiştirmen gerekir.
    [SerializeField] public float thirdHitAnimLength;
    [SerializeField] public float thirdHitStopTime;

    [Header("Objects")]
    [SerializeField] public Transform swordParent;
    [SerializeField] public Transform sword;
    [SerializeField] public GameObject swordObject;
    [SerializeField] public Animator swordAnimator;
    [SerializeField] public GameObject impactEffect;
    [SerializeField] public GameObject impactEffectRED;

    [Header("Sound Effects")]
    [SerializeField] public AudioClip[] swordSlash;
    [SerializeField] public AudioClip[] powerSlash;
    [SerializeField] public AudioClip chargingPowerSlash;
    [SerializeField] public AudioClip powerSlashMAX;
    [SerializeField] public AudioClip critSFX;

    [Header("DONT TOUCH")]
    public int attackInt;
    public bool changedValues;
    private AudioSource audioSource;
    private int lastSlashIndex = -1;
    private int lastPowerSlashIndex = -1;
    public GameObject impactEffectToShow;
    public bool isCharging;
    private bool hasPlayedSound;
    public Vector2 attackDir;
    public Vector2 chargeDir;
    private int lastQuadrant = -1;
    private const float quadrantChangeThreshold = 15f;
    public bool chargeInterrupt;



    private void Update()
    {
        sword.position = swordParent.position;
        attackInt = PlayerAttack.Instance.attackInt;
        swordAnimator.SetInteger("attackInt", attackInt);

        if (attackInt == 3)
        {
            if (!changedValues)
            {
                StartCoroutine(WaitToChangeValues());
            }
        }

        else if (attackInt != 3)
        {
            changedValues = false;
            GetBaseValues();
        }

        PlayerAttack.Instance.anim.SetBool("isCharging", isCharging);

        if (Input.GetMouseButtonDown(0) && PlayerAttack.Instance.canAttack && PlayerAttack.Instance.attackInt != 3)
        {
            SwordAttack();
        }


        if (Input.GetMouseButton(0) && PlayerAttack.Instance.attackInt == 3 && PlayerAttack.Instance.canAttack)
        {

            PlayerAttack.Instance.movementScript.speed = PlayerAttack.Instance.movementScript.baseSpeed / 2;
            if (!PlayerAttack.Instance.coroutineStarted)
            {
                isCharging = true;
                PlayerAttack.Instance.chargeSlider.SetActive(true);
                StartCoroutine(ChargeUp());
                PlayerAttack.Instance.coroutineStarted = true;
            }
        }

        if (isCharging && Input.GetMouseButtonUp(0) || isCharging && chargeInterrupt)
        {
            StartCoroutine(ThirdAttack());
            PlayerAttack.Instance.coroutineStarted = false;
            PlayerAttack.Instance.chargeSlider.SetActive(false);
            StopPowerCharge();
        }

        if (PlayerAttack.Instance.elapsedChargeTime >= PlayerAttack.Instance.maxCharge)
        {
            PlayerAttack.Instance.attackObjectAnimator.SetBool("chargeFull", true);
            PlayerAttack.Instance.maxText.GetComponent<Animator>().SetBool("chargeFull", true);
            PlayerAttack.Instance.chargeSlider.SetActive(false);
            if (!hasPlayedSound)
            {
                hasPlayedSound = true;
                PowerChargeMAX();
            }
        }
        else
        {
            PlayerAttack.Instance.attackObjectAnimator.SetBool("chargeFull", false);
            PlayerAttack.Instance.maxText.GetComponent<Animator>().SetBool("chargeFull", false);
            hasPlayedSound = false;
        }


        if (isCharging)
        {
            Vector2 playerPos = (Vector2)PlayerAttack.Instance.Player.transform.position;
            Vector2 mousePos = (Vector2)Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));

            chargeDir = (mousePos - playerPos).normalized;


            PlayerAttack.Instance.anim.SetInteger("Direction", PlayerAttack.Instance.GetMouseQuadrant(chargeDir));
        }

        PlayerAttack.Instance.chargeSlider.GetComponent<Slider>().value = PlayerAttack.Instance.elapsedChargeTime / PlayerAttack.Instance.maxCharge;

    }
    private void Start()
    {
        impactEffectToShow = impactEffect;
        audioSource = GetComponentInParent<AudioSource>();

        PlayerAttack.Instance.sword = true;
        PlayerAttack.Instance.attackObjectParent = swordParent;
        PlayerAttack.Instance.attackObjectAnimator = swordAnimator;
    }

    private void ChangeValues()
    {
        changedValues = true;
        PlayerAttack.Instance.damageMultiplierWeapon = thirdHitDamageMultiplier;
        PlayerAttack.Instance.attackAngle = thirdHitAngle;
        PlayerAttack.Instance.attackRange = Mathf.RoundToInt(attackRange * thirdHitRangeMultiplier);
        PlayerAttack.Instance.attackCooldown = thirdHitAnimLength + delayAfterThirdHit;
        PlayerAttack.Instance.hitStopTime = thirdHitStopTime;
        PlayerAttack.Instance.powerfulShakeMultiplier = 1.8f;
    }

    public void GetBaseValues()
    {
        PlayerAttack.Instance.attackDamage = attackDamage;
        PlayerAttack.Instance.attackRange = attackRange;
        PlayerAttack.Instance.attackAngle = attackAngle;
        PlayerAttack.Instance.damageMultiplierWeapon = 1;
        PlayerAttack.Instance.attackCooldown = attackCooldown;
        PlayerAttack.Instance.animationLength = animationLength;
        PlayerAttack.Instance.comboResetTime = comboResetTime;
        PlayerAttack.Instance.hitStopTime = hitStopTime;
        PlayerAttack.Instance.powerfulShakeMultiplier = 1f;
        PlayerAttack.Instance.critChance = critChance;
        PlayerAttack.Instance.critDamageMultiplier = critDamageMultiplier;
        PlayerAttack.Instance.maxCharge = maxChargeInSeconds;
        PlayerAttack.Instance.maxChargeDamageMultiplier = maxChargeDamageMultiplier;
    }


    public void SwordAttack()
    {
        PlayerAttack.OnPlayerAttacked?.Invoke(attackInt);

        PlayerAttack.Instance.canAttack = false;
        PlayerAttack.Instance.isAttacking = true;
        PlayerAttack.Instance.isIdle = false;

        // Ensure AttackReset always runs to completion
        if (PlayerAttack.Instance.attackTimer != null)
            StopCoroutine(PlayerAttack.Instance.attackTimer);
        PlayerAttack.Instance.attackTimer = StartCoroutine(PlayerAttack.Instance.AttackReset());

        if (PlayerAttack.Instance.comboTimer != null)
            StopCoroutine(PlayerAttack.Instance.comboTimer);
        PlayerAttack.Instance.comboTimer = StartCoroutine(PlayerAttack.Instance.ComboReset());

        Vector2 playerPos = (Vector2)PlayerAttack.Instance.Player.transform.position;
        Vector2 mousePos = (Vector2)Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
        attackDir = (mousePos - playerPos).normalized;
        float centerAngle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        float minAngle = centerAngle - (attackAngle / 2);
        float maxAngle = centerAngle + (attackAngle / 2);



        switch (PlayerAttack.Instance.attackInt)
        {
            case 1:
                PlayerAttack.Instance.attackObjectParent.rotation = Quaternion.Euler(0, 0, minAngle);
                PlayerAttack.Instance.attackObjectParent.DORotate(new Vector3(0, 0, maxAngle), animationLength).SetEase(Ease.OutExpo);
                PlayerAttack.Instance.attackObjectAnimator.SetTrigger("Swing1");
                SlashSFX();
                NormalSwing();
                PlayerAttack.Instance.anim.SetInteger("Direction", PlayerAttack.Instance.GetMouseQuadrant(attackDir));
                PlayerAttack.Instance.anim.SetTrigger("Attack1+2");
                if (PlayerAttack.Instance.canCombo)
                {
                    PlayerAttack.Instance.attackInt = 2;
                }
                break;

            case 2:
                PlayerAttack.Instance.attackObjectParent.rotation = Quaternion.Euler(0, 0, maxAngle);
                PlayerAttack.Instance.attackObjectParent.DORotate(new Vector3(0, 0, minAngle), animationLength).SetEase(Ease.OutExpo);
                PlayerAttack.Instance.attackObjectAnimator.SetTrigger("Swing2");
                SlashSFX();
                PlayerAttack.Instance.anim.SetInteger("Direction", PlayerAttack.Instance.GetMouseQuadrant(attackDir));
                NormalSwing();
                PlayerAttack.Instance.anim.SetTrigger("Attack1+2");
                if (PlayerAttack.Instance.canCombo)
                {
                    PlayerAttack.Instance.attackInt = 3;
                }
                break;
        }
    }

    public void NormalSwing()
    {


        StartCoroutine(SwingDamageDelayer());

        // If not third attack, start cooldown normally
        if (PlayerAttack.Instance.attackTimer != null)
            StopCoroutine(PlayerAttack.Instance.attackTimer);
        PlayerAttack.Instance.attackTimer = StartCoroutine(PlayerAttack.Instance.AttackReset());


    }

    private IEnumerator SwingDamageDelayer()
    {
        yield return new WaitForSeconds(0.08f);
        DamageSword();
    }

    private void DamageSword()
    {

        float centerAngle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        float minAngle = centerAngle - (attackAngle / 2);
        float maxAngle = centerAngle + (attackAngle / 2);



        //DAMAGE AYARLAMALARI
        float damageMultiplierPain = 1 + (PainMeter.Instance.painMeter.value / PainMeter.Instance.painMeter.maxValue);
        float critMultiply;

        if (Random.value < critChance)
        {
            critMultiply = critDamageMultiplier;
            impactEffectToShow = impactEffectRED;
        }
        else
        {
            critMultiply = 1;
            impactEffectToShow = impactEffect;
        }

        float chargeTimeMultiplier = Mathf.Lerp(1f, maxChargeDamageMultiplier, PlayerAttack.Instance.elapsedChargeTime / PlayerAttack.Instance.maxCharge); //LINEAR BİR ARTIŞ VAR, SQRT KULLANILMADI.


        //rengi etkileyen değişkenler : - crit attık mı atmadık mı?    - üçüncü vuruşta mıyız değil miyiz? 
        //rengi etkilemeyen değişkenler : - pain ile gelen damage boost ne kadar fazla?
        int damageThatEffectsTheColors = Mathf.RoundToInt(attackDamage * critMultiply * PlayerAttack.Instance.damageMultiplierWeapon * chargeTimeMultiplier);
        int damageToDeal = Mathf.RoundToInt(damageThatEffectsTheColors * damageMultiplierPain);




        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(PlayerAttack.Instance.attackPoint.position, PlayerAttack.Instance.attackRange);


        foreach (Collider2D objects in hitObjects)
        {
            Vector2 objectsDir = (objects.transform.position - PlayerAttack.Instance.attackPoint.position).normalized;
            float angleBetween = Vector2.SignedAngle(attackDir, objectsDir);
            
            bool inAttackCone = Mathf.Abs(angleBetween) <= PlayerAttack.Instance.attackAngle / 2f;


            if (inAttackCone)
            {
                if (objects.CompareTag("Enemy"))
                {
                    objects.GetComponent<Health>().Hit(damageToDeal, 0);
                    GameObject effect = Instantiate(impactEffectToShow, objects.transform.position,
                        Quaternion.Euler(0, 0, 0));
                    Destroy(effect, 1f);
                    HitStop.Instance.Stop(hitStopTime);
                    CameraFollow.Instance.TriggerShake(0.24f * PlayerAttack.Instance.powerfulShakeMultiplier, 0.5f * PlayerAttack.Instance.powerfulShakeMultiplier);
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


                }


                else if (objects.CompareTag("Barrel"))
                {
                    objects.GetComponent<Health>().Hit(damageToDeal, 0);
                    objects.GetComponent<EntitySFX>().BarrelHitSFX();
                    GameObject effect = Instantiate(impactEffectToShow, objects.transform.position,
                        Quaternion.Euler(0, 0, 0));
                    Destroy(effect, 1f);
                    HitStop.Instance.Stop(hitStopTime);
                    CameraFollow.Instance.TriggerShake(0.24f * PlayerAttack.Instance.powerfulShakeMultiplier, 0.5f * PlayerAttack.Instance.powerfulShakeMultiplier);
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
                }

            }
        }

        changedValues = true;
        PlayerAttack.Instance.elapsedChargeTime = 0f;
    }

    private IEnumerator ThirdAttack()
    {
        PlayerAttack.OnPlayerAttacked?.Invoke(attackInt);
        PlayerAttack.Instance.canAttack = false;
        PlayerAttack.Instance.isAttacking = true;
        isCharging = false;
        PlayerAttack.Instance.movementScript.speed = PlayerAttack.Instance.movementScript.baseSpeed;

        PlayerAttack.Instance.anim.SetTrigger("Attack3");
        if (PlayerAttack.Instance.attackTimer != null)
            StopCoroutine(PlayerAttack.Instance.attackTimer);
        PlayerAttack.Instance.attackTimer = StartCoroutine(PlayerAttack.Instance.AttackReset());


        Vector2 playerPos = (Vector2)PlayerAttack.Instance.Player.transform.position;
        Vector2 mousePos = (Vector2)Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
        attackDir = (mousePos - playerPos).normalized;
        float centerAngle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        float arc = PlayerAttack.Instance.attackAngle;
        float minAngle = centerAngle - arc / 2f;
        float maxAngle = centerAngle + arc / 2f;
        PlayerAttack.Instance.lockedAttackDir = attackDir;


        StartCoroutine(PowerSlashSFXX());

        //yield return new WaitForSeconds(swordScript.delayBeforeThirdHit);

        PlayerAttack.Instance.attackObjectParent.rotation = Quaternion.Euler(0, 0, minAngle);
        PlayerAttack.Instance.attackObjectParent
            .DORotate(new Vector3(0, 0, maxAngle), thirdHitAnimLength)
            .SetEase(Ease.OutExpo);
        PlayerAttack.Instance.attackObjectAnimator.SetTrigger("Swing3");

        StartCoroutine(SwordTurn());

        NormalSwing();
        PlayerAttack.Instance.movementScript.speed = PlayerAttack.Instance.movementScript.baseSpeed;
        StartCoroutine(AttackResetForThree());

        yield return new WaitForSeconds(thirdHitAnimLength);
        PlayerAttack.Instance.ComboResetFunc();


    }

    private IEnumerator AttackResetForThree()
    {
        yield return new WaitForSeconds(thirdHitAnimLength); //attack cooldown yerine bunu kullandık, animasyon uzunluğunda attack cooldown olmuş oldu. attack cooldown çok hızlı değiştiğinde algılanamıyordu? ANİMASYON İPTALİ HATALARI BURADAN KAYNAKLANIYOR DİKKAT! 1-2-3-1-2-3 değil 1-2-3-1-1-2 gibi
        PlayerAttack.Instance.attackInt = 1;
        PlayerAttack.Instance.attackObjectAnimator.SetTrigger("idle");
        PlayerAttack.Instance.isIdle = true;
        PlayerAttack.Instance.canAttack = true;
    }

    private IEnumerator ChargeUp()
    {
        StartPowerChargeSFX();
        if (PlayerAttack.Instance.comboTimer != null)
            StopCoroutine(PlayerAttack.Instance.comboTimer);

        PlayerAttack.Instance.elapsedChargeTime = 0f;
        while (PlayerAttack.Instance.elapsedChargeTime < PlayerAttack.Instance.maxCharge && isCharging) // Run for 5 seconds
        {
            PlayerAttack.Instance.elapsedChargeTime += Time.deltaTime;
            yield return null; // Wait until next frame
        }
    }


    private IEnumerator SwordTurn()
    {
        PlayerAttack.Instance.anim.SetInteger("Direction", PlayerAttack.Instance.GetMouseQuadrantTurnRight(PlayerAttack.Instance.lockedAttackDir));
        yield return new WaitForSeconds(thirdHitAnimLength / 4);
        PlayerAttack.Instance.anim.SetInteger("Direction", PlayerAttack.Instance.GetMouseQuadrant(PlayerAttack.Instance.lockedAttackDir));
        yield return new WaitForSeconds(thirdHitAnimLength / 3);
        PlayerAttack.Instance.anim.SetInteger("Direction", PlayerAttack.Instance.GetMouseQuadrantTurnLeft(PlayerAttack.Instance.lockedAttackDir));
    }

    private IEnumerator PowerSlashSFXX()
    {
        yield return new WaitForSeconds(0.02f);
        PowerSlashSFX();
    }

    private IEnumerator WaitToChangeValues()
    {
        yield return new WaitForSeconds(animationLength / 2);
        ChangeValues();
    }

    public void SlashSFX()
    {
        if (swordSlash.Length > 0)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, swordSlash.Length);
            } while (randomIndex == lastSlashIndex);

            lastSlashIndex = randomIndex;
            audioSource.PlayOneShot(swordSlash[randomIndex]);
        }
    }

    public void PowerSlashSFX()
    {
        if (powerSlash.Length > 0)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, powerSlash.Length);
            } while (randomIndex == lastPowerSlashIndex);

            lastPowerSlashIndex = randomIndex;
            audioSource.PlayOneShot(powerSlash[randomIndex]);
        }
    }


    private void StartPowerChargeSFX()
    {
        audioSource.clip = chargingPowerSlash;
        audioSource.Play();
    }

    private void StopPowerCharge()
    {
        audioSource.Stop();
    }

    public void PowerChargeMAX()
    {
        if (powerSlashMAX != null)
            audioSource.PlayOneShot(powerSlashMAX);
    }

    public void CritSFX()
    {
        if (critSFX != null)
            audioSource.PlayOneShot(critSFX);
    }



}