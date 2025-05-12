using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CloneSword : MonoBehaviour
{
    public Transform attackPoint;
    public GameObject impactEffect;
    public Animator animSword;
    public Animator animClone;
    private CloneAttack cloneAttack;
    public Transform sword;
    public Transform swordParent;
    public float delay;
    //public float hitStopTime;

    void Start()
    {
        cloneAttack = GetComponentInParent<CloneAttack>();
        animSword = GetComponentInParent<Animator>();
        sword = this.gameObject.transform;
        swordParent = transform.parent;
        animSword.SetTrigger("idle");
    }
    public void SetupFromSword(Sword original)
    {
        impactEffect = original.impactEffect;
        //hitStopTime = original.hitStopTime;
        //attackPoint = original.transform.Find("AttackPoint"); // or assign from outside
    }


    public void PerformSwing(Vector2 direction, int attackInt)
    {

            direction.Normalize();
            float centerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.Euler(0, 0, centerAngle);
            float minAngle = centerAngle - (PlayerAttack.Instance.attackAngle / 2);
            float maxAngle = centerAngle + (PlayerAttack.Instance.attackAngle / 2);
            float arc = PlayerAttack.Instance.attackAngle;
            float minAngle3 = centerAngle - arc / 2f;
            float maxAngle3 = centerAngle + arc / 2f;







            switch (attackInt)
            {
                case 1: animSword.SetTrigger("Swing1"); animClone?.SetTrigger("Attack1+2"); Debug.Log("1"); swordParent.rotation = Quaternion.Euler(0, 0, minAngle); swordParent.DORotate(new Vector3(0, 0, maxAngle), PlayerAttack.Instance.animationLength).SetEase(Ease.OutExpo); break;

                case 2: animSword.SetTrigger("Swing2"); animClone?.SetTrigger("Attack1+2"); Debug.Log("2"); swordParent.rotation = Quaternion.Euler(0, 0, maxAngle); swordParent.DORotate(new Vector3(0, 0, minAngle), PlayerAttack.Instance.animationLength).SetEase(Ease.OutExpo); break;

                case 3: animSword.SetTrigger("Swing3"); animClone?.SetTrigger("Attack3"); Debug.Log("3"); swordParent.rotation = Quaternion.Euler(0, 0, minAngle3); swordParent.DORotate(new Vector3(0, 0, maxAngle3 + 360f), PlayerAttack.Instance.attackCooldown, RotateMode.FastBeyond360).SetEase(Ease.OutExpo); break;
            }

            StartCoroutine(SwingDamageAfterDelay(0.08f, direction));

    }








    private void DealDamage(Vector2 dir)
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("Attack point not assigned!");
            return;
        }

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, PlayerAttack.Instance.attackRange);

        foreach (var obj in hitObjects)
        {
            Vector2 toTarget = (obj.transform.position - attackPoint.position).normalized;
            float angleBetween = Vector2.Angle(dir, toTarget);

            if (angleBetween <= PlayerAttack.Instance.attackAngle / 2f)
            {
                if (obj.CompareTag("Enemy"))
                {
                    obj.GetComponent<Health>()?.Hit(PlayerAttack.Instance.attackDamage, 0);
                    HitStop.Instance.Stop(PlayerAttack.Instance.hitStopTime);

                    if (impactEffect != null)
                    {
                        GameObject fx = Instantiate(impactEffect, obj.transform.position, Quaternion.identity);
                        Destroy(fx, 1f);
                    }
                }

                else if (obj.CompareTag("Barrel"))
                {
                    obj.GetComponent<Health>()?.Hit(PlayerAttack.Instance.attackDamage, 0);
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
    }


    private IEnumerator SwingDamageAfterDelay(float delay, Vector2 dir)
    {
        yield return new WaitForSeconds(delay);
        DealDamage(dir);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackPoint.position, PlayerAttack.Instance.attackRange);

        // Use the last attack direction or fallback
        Vector2 direction = cloneAttack.attackDir != Vector2.zero ? cloneAttack.attackDir : Vector2.right;

        Vector2 leftLimit = Quaternion.Euler(0, 0, -PlayerAttack.Instance.attackAngle / 2) * direction;
        Vector2 rightLimit = Quaternion.Euler(0, 0, PlayerAttack.Instance.attackAngle / 2) * direction;

        Gizmos.DrawLine(attackPoint.position, attackPoint.position + (Vector3)leftLimit * PlayerAttack.Instance.attackRange);
        Gizmos.DrawLine(attackPoint.position, attackPoint.position + (Vector3)rightLimit * PlayerAttack.Instance.attackRange);
    }

    private void Update()
    {
        if (animClone == null)
        {
            animClone = cloneAttack.cloneAnim;
        }

        sword.position = swordParent.position;
        //delay = CloneSkill.instance.delay;

    }


}