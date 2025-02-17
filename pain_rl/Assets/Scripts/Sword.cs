using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour
{

    public static Sword instance;

    [Header("Values")]
    [SerializeField] public int attackDamage;
    [SerializeField] public float attackRange;
    [SerializeField] public float attackAngle;
    [SerializeField] public float attackCooldown;
    [SerializeField] public float critChance;
    [SerializeField] public float critDamageMultiplier;
    [SerializeField] public float animationLength; //BUNU DE���T�R�RSEN 1) FAKE SWORD YOK OLUP GER�E�� GELEN AN�MASYONUN S�RES�N�    2) TRAIL EFFECT AN�MASYONULARININ S�RES�N� BUNU E��TLE AN�MAT�RDE.
    [SerializeField] public float comboResetTime; //BUNU DE���T�R�RSEN SWING 1 VE 2 YAPILIRKEN PLAYERDA G�Z�KEN AN�MASYONUN NORMALE D�NME S�RES�N� DE AYARLA. E�ER COMBO DEVAM EDEB�LECEK DURUMDAYSA PLAYER DAHA GERG�N? DURSUN K� COMBO NE ZAMAN B�TT� ANLA�ILSIN. + FAKE SWORD IDLE'A G�DERKEN OLAN EXIT TIMELARI DE���T�R.
    [SerializeField] public float hitStopTime;

    [Header("3rd Hit Values")]
    [SerializeField] public float thirdHitDamageMultiplier;
    [SerializeField] public float thirdHitRangeMultiplier;
    [SerializeField] public float thirdHitAngle;
    [SerializeField] public float maxChargeInSeconds;  //BUNU DE���T�R�RSEN PLAYER CHARGE AN�MASYONU KA� SAN�YE ONU DA DE���T�RMEN GEREK�R. + PLAYER ATTACK'TA BU DE�ER�N ETK�LED��� HASARIN DE����M HIZINI DE���T�RMEN GEREK�R.
    [SerializeField] public float maxChargeDamageMultiplier;
    [Space]
    [SerializeField] public float delayBeforeThirdHit; // BUNU DE���T�R�RSEN PLAYER AN�MAT�R�NDEK�!!! (sword de�il) SWORDATTACK3 AN�MASYONUNUN DELAY G�STERMEL�K �LK KISMINI DA OYNATMAN GEREK�R!
    [SerializeField] public float delayAfterThirdHit; // YEN� KOMBO BA�LATAB�LMEK ���N GEREKL� S�RE. bi �stteki animasyonunun ayn�s�n�n son k�sm�n� de�i�tirmen gerekir.
    [SerializeField] public float thirdHitAnimLength;
    [SerializeField] public float thirdHitStopTime;

    [Header("Objects")]
    [SerializeField] public Transform swordParent;
    [SerializeField] public Transform sword;
    [SerializeField] public Animator swordAnimator;
    [SerializeField] public GameObject impactEffect;
    [SerializeField] public GameObject impactEffectRED;

    [Header("Sound Effects")]
    [SerializeField] public AudioClip[] swordSlash;
    [SerializeField] public AudioClip[] powerSlash;
    [SerializeField] public AudioClip chargePowerSlash;
    [SerializeField] public AudioClip critSFX;

    [Header("DONT TOUCH")]
    public int attackInt;
    public bool changedValues;
    private AudioSource audioSource;
    private int lastSlashIndex = -1;
    private int lastPowerSlashIndex = -1;
    public GameObject impactEffectToShow;


    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        sword.position = swordParent.position;
        attackInt = PlayerAttack.Instance.attackInt;
        swordAnimator.SetInteger("attackInt", attackInt);

        if(attackInt == 3)
        {
            if(!changedValues)
            {
                StartCoroutine(WaitToChangeValues());
            }
        }

        else if (attackInt != 3)
        {
            changedValues = false;
            GetBaseValues();
        }
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


    public void PowerChargeSFX()
    {
        if(chargePowerSlash != null)
            audioSource.PlayOneShot(chargePowerSlash);
    }

    public void CritSFX()
    {
        if (critSFX != null)
            audioSource.PlayOneShot(critSFX);
    }



}
