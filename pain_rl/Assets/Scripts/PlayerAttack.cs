using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using Image = UnityEngine.UI.Image;

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack Instance;

    [Header("SET THESE UP!")]
    [SerializeField] public Transform attackPoint;
    [SerializeField] public GameObject maxText;
    [SerializeField] public GameObject chargeSlider;
    [SerializeField] public Color baseFillColor;

    [Header("DONT TOUCH!")]
    [SerializeField] public int attackDamage;
    [SerializeField] public float attackRange;
    [SerializeField] public float attackRange2;
    [SerializeField] public float attackAngle;
    [SerializeField] public float attackCooldown;
    [SerializeField] public float critChance;
    [SerializeField] public float critDamageMultiplier;
    [SerializeField] public float animationLength;
    [SerializeField] public float comboResetTime;
    [SerializeField] public float hitStopTime;
    [SerializeField] public float knockbackForce;
    [SerializeField] public int attackInt;
    [SerializeField] public float damageMultiplierWeapon;
    [SerializeField] public Transform attackObjectParent;
    [SerializeField] public Animator attackObjectAnimator;
    [SerializeField] public float maxCharge;
    [SerializeField] public float maxChargeDamageMultiplier;
    public float delayBetweenAttacks;
    public float damageMultiplierPain;
    public float elapsedChargeTime = 0f;
    public float powerfulShakeMultiplier;
    public bool isAttacking = false;
    public bool canAttack;
    public bool canCombo;
    public Animator anim;
    public PlayerMovement movementScript;
    private Sword swordScript;
    private Punch punchScript;
    public Coroutine comboTimer;
    public Coroutine attackTimer;
    public Vector2 lockedAttackDir;
    public bool coroutineStarted;
    public GameObject Player;
    public bool isIdle;
    public bool isCharging;
    public Vector2 attackDir;
    public static System.Action<int> OnPlayerAttacked;
    public static System.Action<bool> OnPlayerAttackedPunch;
    public GameObject chargeSliderFill;
    public float weaponSkillDamageMultiplier;
    public bool attackDirLocked;
    public bool isCharged;
    public int numberOfAttacks;
    public bool attackWaitingCheck;

    [Space]
    [Header("WEAPON SELECTION")]
    public bool sword = false;
    public bool punch = false;
    [Header("TEST")]
    public float testAngle;


    void Awake()
    {
        if (Instance == null && CompareTag("Player"))
        {
            Instance = this;
            Debug.Log("PlayerAttack.Instance assigned to: " + gameObject.name);
        }
        else if (Instance != null && this != Instance)
        {
            Debug.Log("Prevented clone from overriding PlayerAttack.Instance: " + gameObject.name);
        }
    }




    private void Start()
    {
        anim = GetComponent<Animator>();
        movementScript = GetComponent<PlayerMovement>();
        swordScript = GetComponentInChildren<Sword>();
        punchScript = GetComponentInChildren<Punch>();
        canAttack = true;
        attackInt = 1;
        //chargeSlider.SetActive(false);
        elapsedChargeTime = 0f;
        Player = this.gameObject;
        Transform fill = chargeSlider.transform.Find("Fill Area/Fill");
        chargeSliderFill = fill.gameObject;
        chargeSliderFill.GetComponent<Image>().color = baseFillColor;

    }

    private void Update()
    {

        if(EventManager.Instance.GameFrozen)
        {
            canAttack = false;
        }

        if(attackObjectAnimator != null && attackObjectParent != null)
        {
            if (sword)
            {
                attackObjectAnimator.SetBool("canCombo", canCombo);
                attackObjectParent.position = attackPoint.transform.position;
            }
            
            
        }
        
        

        //eğer sword ise ve chargeDir / attackDir birbirinden farklıysa değişiyorsa ona göre flipletme???
        if(sword)
        {
            attackDir = swordScript.attackDir;
            isCharging = swordScript.isCharging;
            weaponSkillDamageMultiplier = 1;
        }

        if (punch)
        {
            attackDir = punchScript.attackDir;
            weaponSkillDamageMultiplier = 4;
            float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
            attackObjectParent.rotation = Quaternion.Euler(0, 0, angle);
        }

        damageMultiplierPain = 1 + (PainMeter.Instance.painMeter.value / PainMeter.Instance.painMeter.maxValue);
    }







    #region MOUSE QUADRANTS
    public int GetMouseQuadrant(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Get angle in degrees
        testAngle = angle;

        if (angle >= -45 && angle < 45) return 3;  // Right side
        if (angle >= 45 && angle < 135) return 0;  // Top side
        if (angle >= -135 && angle < -45) return 2; // Bottom side
        return 1; // Left side
    }


    public int GetMouseQuadrantTurnLeft(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Get angle in degrees

        if (angle >= -45 && angle < 45) return 0;  // Right side
        if (angle >= 45 && angle < 135) return 1;  // Top side
        if (angle >= -135 && angle < -45) return 3; // Bottom side
        return 2; // Left side
    }

    public int GetMouseQuadrantTurnRight(Vector2 direction)
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

        if(punch)
        {
            Gizmos.color = Color.red;

            // Get mouse position in world space
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 attackDir = (mousePos - (Vector2)attackPoint.position).normalized;

            // Set up the box
            Vector2 boxSize = new Vector2(attackRange, attackRange2);
            Vector2 pivot = attackPoint.position;
            Vector2 boxCenter = pivot + attackDir * (boxSize.x / 2f); // Forward offset from pivot
            float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

            // Apply rotation and draw the box
            Gizmos.matrix = Matrix4x4.TRS(boxCenter, Quaternion.Euler(0, 0, angle), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);

            // Reset matrix just in case
            Gizmos.matrix = Matrix4x4.identity;
        }

    }

    public IEnumerator AttackReset()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        isAttacking = false;
        attackTimer = null; // Reset coroutine reference
    }

    public IEnumerator AttackResetPunch()
    {
        yield return new WaitForSeconds(attackCooldown);

        attackWaitingCheck = true;

        yield return new WaitForSeconds(delayBetweenAttacks);
        canAttack = true;
        isAttacking = false;
        attackTimer = null; // Reset coroutine reference
        attackWaitingCheck = false;
    }
    public void ComboResetFunc()
    {
        if (sword)
        {
            if (attackInt == 2 || attackInt == 3)
            {
                attackInt = 1;
                canCombo = false;
                attackObjectAnimator.SetTrigger("idle");
                isIdle = true;
            }
        }

    }

    public IEnumerator ComboReset()
    {
        canCombo = true;
        yield return new WaitForSeconds(comboResetTime);
        ComboResetFunc();
    }

}