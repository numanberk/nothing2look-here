using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ClonePunch : MonoBehaviour
{
    public Transform attackPoint;
    public GameObject impactEffect;
    public Animator animPunch;
    public Animator animClone;
    private CloneAttack cloneAttack;
    public Transform punch;
    public Transform punchParent;
    public float delay;
    public float animationLength;
    public int currentNumberOfAttacks;
    public Vector2 directionAll;
    private Coroutine animationCoroutine;
    //public float hitStopTime;

    void Start()
    {
        cloneAttack = GetComponentInParent<CloneAttack>();
        animPunch = GetComponentInParent<Animator>();
        punch = this.gameObject.transform;
        punchParent = transform.parent;
        animPunch.SetTrigger("idle");
    }
    public void SetupFromPunch(Punch original)
    {
        impactEffect = original.impactEffect;
        //animationLength = original.animationLength;
        //currentNumberOfAttacks = original.currentNumberOfAttacks;
        //hitStopTime = original.hitStopTime;
        //attackPoint = original.transform.Find("AttackPoint"); // or assign from outside
    }


    public void PerformPunch(Vector2 direction, bool isCharged)
    {

        direction.Normalize();
        Vector2 pivot = attackPoint.position;
        Vector2 boxSize = new Vector2(PlayerAttack.Instance.attackRange, PlayerAttack.Instance.attackRange2);
        Vector2 boxCenter = pivot + direction * (boxSize.x * 4 / 5);
        Vector2 boxCenterCharged = pivot + direction * (boxSize.x * 4/5);


        punchParent.position = attackPoint.position;

        animClone?.SetTrigger("Punch");


        if (!isCharged)
        {
            animPunch.SetTrigger("Punch1");
            punchParent.DOMove(boxCenter, animationLength / (currentNumberOfAttacks * 2)).SetEase(Ease.OutExpo); // Move quickly to attack end point, PlayerAttack.Instance.animationLength).SetEase(Ease.OutExpo); break;
            animPunch.speed = currentNumberOfAttacks / animationLength;
        }

        else if (isCharged)
        {
            animPunch.SetTrigger("ChargedPunch");
            punchParent.DOMove(boxCenterCharged, animationLength / (currentNumberOfAttacks * 2)).SetEase(Ease.OutExpo); // Move quickly to attack end point, PlayerAttack.Instance.animationLength).SetEase(Ease.OutExpo); break;
            animPunch.speed = currentNumberOfAttacks / animationLength;
        }


        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(Animationn());

        StartCoroutine(SwingDamageAfterDelay(0.08f, direction));


    }



    IEnumerator Animationn()
    {
        
        yield return new WaitForSeconds(animationLength / currentNumberOfAttacks);

        punchParent.position = PlayerAttack.Instance.attackPoint.transform.position;
        animPunch.speed = 1;


    }




    private void DealDamage(Vector2 direction)
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("Attack point not assigned!");
            return;
        }

        Vector2 boxSize = new Vector2(PlayerAttack.Instance.attackRange, PlayerAttack.Instance.attackRange2);
        Vector2 pivot = attackPoint.position;

        // Calculate box center in the direction of attackDir
        Vector2 boxCenter = pivot + direction * (boxSize.x / 2f); // Push box in front of attack point

        // Get angle in degrees from attackDir
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Detect objects inside the rotated box
        Collider2D[] hitObjects = Physics2D.OverlapBoxAll(boxCenter, boxSize, angle);


        foreach (Collider2D obj in hitObjects)
        {

            if (obj.CompareTag("Enemy"))
            {
                obj.GetComponent<Health>()?.Hit(Mathf.RoundToInt(PlayerAttack.Instance.attackDamage * PlayerAttack.Instance.damageMultiplierWeapon), 0);
                HitStop.Instance.Stop(PlayerAttack.Instance.hitStopTime);

                if (impactEffect != null)
                {
                    GameObject fx = Instantiate(impactEffect, obj.transform.position, Quaternion.identity);
                    Destroy(fx, 1f);
                }
            }

            else if (obj.CompareTag("Barrel"))
            {
                obj.GetComponent<Health>()?.Hit(Mathf.RoundToInt(PlayerAttack.Instance.attackDamage * PlayerAttack.Instance.damageMultiplierWeapon), 0);
                obj.GetComponent<EntitySFX>().BarrelHitSFX();
                HitStop.Instance.Stop(PlayerAttack.Instance.hitStopTime);

                if (impactEffect != null)
                {
                    GameObject fx = Instantiate(impactEffect, obj.transform.position, Quaternion.identity);
                    Destroy(fx, 1f);
                }
            }

        }

    }


    private IEnumerator SwingDamageAfterDelay(float delay, Vector2 dir)
    {
        yield return new WaitForSeconds(delay);
        DealDamage(dir);
    }

    private void Update()
    {
        if (animClone == null)
        {
            animClone = cloneAttack.cloneAnim;
        }

        punch.position = punchParent.position;
        //delay = CloneSkill.instance.delay;


        float angle = Mathf.Atan2(directionAll.y, directionAll.x) * Mathf.Rad2Deg;
        punchParent.rotation = Quaternion.Euler(0, 0, angle);

        currentNumberOfAttacks = PlayerAttack.Instance.numberOfAttacks;
        animationLength = PlayerAttack.Instance.animationLength;

    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null)
            return;

        // Ensure directionAll is normalized
        Vector2 direction = directionAll.normalized;

        // Box settings
        Vector2 boxSize = new Vector2(PlayerAttack.Instance.attackRange, PlayerAttack.Instance.attackRange2);
        Vector2 pivot = attackPoint.position;
        Vector2 boxCenter = pivot + direction * (boxSize.x / 2f); // push in attack direction

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply rotation matrix manually to get corner positions (needed for accurate rotated box)
        Gizmos.color = Color.red;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(boxCenter, Quaternion.Euler(0, 0, angle), Vector3.one);
        Gizmos.matrix = rotationMatrix;

        // Draw box
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }



}