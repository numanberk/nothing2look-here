using DG.Tweening;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack Instance;

    [Header("SET THESE UP!")]
    [SerializeField] public Transform attackPoint;

    [Header("DONT TOUCH!")]
    [SerializeField] public int attackDamage;
    [SerializeField] public float attackRange;
    [SerializeField] public float attackAngle;
    [SerializeField] public float attackCooldown;
    [SerializeField] public float animationLength;
    [SerializeField] public float comboResetTime;
    [SerializeField] public float hitStopTime;
    [SerializeField] public int attackInt;
    [SerializeField] public Transform attackObjectParent;
    [SerializeField] public Animator attackObjectAnimator;
    public float powerfulShakeMultiplier;
    public bool isAttacking = false;
    public bool sword = false;
    public bool canAttack;
    public bool canCombo;
    private Animator anim;
    private PlayerMovement movementScript;
    private Sword swordScript;
    private Coroutine comboTimer;
    private Coroutine attackTimer;
    private Vector2 lockedAttackDir;
    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        movementScript = GetComponent<PlayerMovement>();
        swordScript = GetComponentInChildren<Sword>();
        canAttack = true;
        attackInt = 1;
    }

    private void Update()
    {
        attackObjectAnimator.SetBool("canCombo", canCombo);

        if (sword && Input.GetMouseButtonDown(0) && canAttack)
        {
            SwordAttack();
        }
    }

    #region SWORD
    private void SwordAttack()
    {
        canAttack = false;
        isAttacking = true;

        // Ensure AttackReset always runs to completion
        if (attackTimer != null)
            StopCoroutine(attackTimer);
        attackTimer = StartCoroutine(AttackReset());

        if (comboTimer != null)
            StopCoroutine(comboTimer);
        comboTimer = StartCoroutine(ComboReset());

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)attackPoint.position).normalized;
        Vector2 attackDir = (mousePos - (Vector2)attackPoint.position).normalized;
        float centerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float minAngle = centerAngle - (attackAngle / 2);
        float maxAngle = centerAngle + (attackAngle / 2);
        


        switch (attackInt)
        {
            case 1:
                if (canCombo)
                {
                    attackInt = 2;
                }
                attackObjectParent.rotation = Quaternion.Euler(0, 0, minAngle);
                attackObjectParent.DORotate(new Vector3(0, 0, maxAngle), animationLength).SetEase(Ease.OutExpo);
                attackObjectAnimator.SetTrigger("Swing1");
                swordScript.SlashSFX();
                NormalSwing();
                anim.SetInteger("Direction", GetMouseQuadrant(attackDir));
                anim.SetTrigger("Attack1+2");
                break;

            case 2:
                if (canCombo)
                {
                    attackInt = 3;
                }
                attackObjectParent.rotation = Quaternion.Euler(0, 0, maxAngle);
                attackObjectParent.DORotate(new Vector3(0, 0, minAngle), animationLength).SetEase(Ease.OutExpo);
                attackObjectAnimator.SetTrigger("Swing2");
                swordScript.SlashSFX();
                anim.SetInteger("Direction", GetMouseQuadrant(attackDir));
                NormalSwing();
                anim.SetTrigger("Attack1+2");
                break;

            case 3:
                StartCoroutine(DelayedAttack());
                anim.SetTrigger("Attack3"); // BU ANİMASYON DELAYİ DE GÖSTERME AMAÇLI OLDUĞUNDAN ÖNCEDEN BAŞLATTIM. 
                return; // EXIT HERE so AttackReset doesn't start twice!
        }
    }



    private IEnumerator DelayedAttack()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)attackPoint.position).normalized;
        Vector2 attackDir = (mousePos - (Vector2)attackPoint.position).normalized;
        float centerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float minAngle = centerAngle - (attackAngle / 2);
        float maxAngle = centerAngle + (attackAngle / 2);
        float angleDiff = maxAngle - minAngle;
        movementScript.speed *= 0.5f;
        lockedAttackDir = attackDir;

        swordScript.PowerChargeSFX();
        StartCoroutine(PowerSlashSFX());

        yield return new WaitForSeconds(swordScript.delayBeforeThirdHit);
        
        attackObjectParent.rotation = Quaternion.Euler(0, 0, minAngle);
        attackObjectParent.DORotate(new Vector3(0, 0, minAngle + angleDiff), swordScript.thirdHitAnimLength).SetEase(Ease.OutExpo);
        attackObjectAnimator.SetTrigger("Swing3");


        StartCoroutine(SwordTurn());

        NormalSwing();
        movementScript.speed = movementScript.baseSpeed;

        yield return new WaitForSeconds(attackCooldown);
        attackInt = 1;
        attackObjectAnimator.SetTrigger("idle");
        canAttack = true;
    }



    private void NormalSwing()
    {


        StartCoroutine(SwingDamageDelayer());

        // If not third attack, start cooldown normally
        if (attackTimer != null)
            StopCoroutine(attackTimer);
        attackTimer = StartCoroutine(AttackReset());


    }

    private void Damage()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 attackDir = (mousePos - (Vector2)attackPoint.position).normalized;

        float centerAngle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        float minAngle = centerAngle - (attackAngle / 2);
        float maxAngle = centerAngle + (attackAngle / 2);

        float damageMultiplierPain = 1 + (PainMeter.Instance.painMeter.value / PainMeter.Instance.painMeter.maxValue);
        int damageToDeal = Mathf.RoundToInt(attackDamage * damageMultiplierPain);

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

        foreach (Collider2D objects in hitObjects)
        {
            Vector2 objectsDir = (objects.transform.position - attackPoint.position).normalized;
            float objectAngle = Mathf.Atan2(objectsDir.y, objectsDir.x) * Mathf.Rad2Deg;

            // Fix for large angles (handles full 360°)
            float adjustedMin = (minAngle + 360) % 360;
            float adjustedMax = (maxAngle + 360) % 360;
            float adjustedObjAngle = (objectAngle + 360) % 360;

            bool inAttackCone = (adjustedMin < adjustedMax)
                ? (adjustedObjAngle >= adjustedMin && adjustedObjAngle <= adjustedMax)
                : (adjustedObjAngle >= adjustedMin || adjustedObjAngle <= adjustedMax);

            if (inAttackCone)
            {
                if (objects.CompareTag("Enemy"))
                {
                    objects.GetComponent<Health>().Hit(damageToDeal, 0);
                    GameObject effect = Instantiate(swordScript.impactEffect, objects.transform.position,
                        Quaternion.Euler(0, 0, objectAngle));
                    Destroy(effect, 1f);
                    HitStop.Instance.Stop(hitStopTime);
                    CameraFollow.Instance.TriggerShake(0.24f * powerfulShakeMultiplier, 0.5f * powerfulShakeMultiplier);
                }


                else if (objects.CompareTag("Barrel"))
                {
                    objects.GetComponent<Health>().Hit(damageToDeal, 0);
                    objects.GetComponent<EntitySFX>().BarrelHitSFX();
                    GameObject effect = Instantiate(swordScript.impactEffect, objects.transform.position,
                        Quaternion.Euler(0, 0, objectAngle));
                    Destroy(effect, 1f);
                    HitStop.Instance.Stop(hitStopTime);
                    CameraFollow.Instance.TriggerShake(0.24f * powerfulShakeMultiplier, 0.5f * powerfulShakeMultiplier);
                }

            }
        }

        swordScript.changedValues = true;
    }

    private IEnumerator SwordTurn()
    {
        anim.SetInteger("Direction", GetMouseQuadrantTurnRight(lockedAttackDir));
        yield return new WaitForSeconds(swordScript.thirdHitAnimLength / 4);
        anim.SetInteger("Direction", GetMouseQuadrant(lockedAttackDir));
        yield return new WaitForSeconds(swordScript.thirdHitAnimLength / 3);
        anim.SetInteger("Direction", GetMouseQuadrantTurnLeft(lockedAttackDir));
    }

    private IEnumerator SwingDamageDelayer()
    {
        yield return new WaitForSeconds(0.08f);
        Damage();   
    }

    private IEnumerator PowerSlashSFX()
    {
        yield return new WaitForSeconds(swordScript.delayBeforeThirdHit - 0.15f);
        swordScript.PowerSlashSFX();
    }

    #endregion

    #region MOUSE QUADRANTS
    private int GetMouseQuadrant(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Get angle in degrees

        if (angle >= -45 && angle < 45) return 3;  // Right side
        if (angle >= 45 && angle < 135) return 0;  // Top side
        if (angle >= -135 && angle < -45) return 2; // Bottom side
        return 1; // Left side
    }

    private int GetMouseQuadrantTurnLeft(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Get angle in degrees

        if (angle >= -45 && angle < 45) return 0;  // Right side
        if (angle >= 45 && angle < 135) return 1;  // Top side
        if (angle >= -135 && angle < -45) return 3; // Bottom side
        return 2; // Left side
    }

    private int GetMouseQuadrantTurnRight(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Get angle in degrees

        if (angle >= -45 && angle < 45) return 2;  // Right side
        if (angle >= 45 && angle < 135) return 3;  // Top side
        if (angle >= -135 && angle < -45) return 1; // Bottom side
        return 0; // Left side
    }

    #endregion


    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        if (sword)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);

            // Get mouse position in world space
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 attackDir = (mousePos - (Vector2)attackPoint.position).normalized;

            Vector2 leftLimit = Quaternion.Euler(0, 0, -attackAngle / 2) * attackDir;
            Vector2 rightLimit = Quaternion.Euler(0, 0, attackAngle / 2) * attackDir;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(attackPoint.position, attackPoint.position + (Vector3)leftLimit * attackRange);
            Gizmos.DrawLine(attackPoint.position, attackPoint.position + (Vector3)rightLimit * attackRange);
        }


    }


    private IEnumerator AttackReset()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        isAttacking = false;
        attackTimer = null; // Reset coroutine reference
    }


    private void ComboResetFunc()
    {
        if(sword)
        {
            if (attackInt == 2 || attackInt == 3)
            {
                attackInt = 1;
                canCombo = false;
                attackObjectAnimator.SetTrigger("idle");
            }
        }

    }

    private IEnumerator ComboReset()
    {
        canCombo = true;
        yield return new WaitForSeconds(comboResetTime);
        ComboResetFunc();
    }

}
