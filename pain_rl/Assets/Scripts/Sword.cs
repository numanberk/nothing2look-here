using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] public int attackDamage;
    [SerializeField] public float attackRange;
    [SerializeField] public float attackAngle;
    [SerializeField] public float attackCooldown;
    [SerializeField] public float animationLength; //BUNU DEÐÝÞTÝRÝRSEN 1) FAKE SWORD YOK OLUP GERÇEÐÝ GELEN ANÝMASYONUN SÜRESÝNÝ    2) TRAIL EFFECT ANÝMASYONULARININ SÜRESÝNÝ BUNU EÞÝTLE ANÝMATÖRDE.

    [Header("Objects")]
    [SerializeField] public Transform swordParent;
    [SerializeField] public Transform sword;
    [SerializeField] public Animator swordAnimator;
    [SerializeField] public GameObject impactEffect;


    public int attackInt;


    private void Update()
    {
        sword.position = swordParent.position;
    }
    private void Start()
    {
        attackInt = 1;
        PlayerAttack.Instance.sword = true;
        PlayerAttack.Instance.attackDamage = attackDamage;
        PlayerAttack.Instance.attackRange = attackRange;
        PlayerAttack.Instance.attackAngle = attackAngle;
        PlayerAttack.Instance.attackCooldown = attackCooldown;
        PlayerAttack.Instance.animationLength = animationLength;
        PlayerAttack.Instance.attackObjectParent = swordParent;
        PlayerAttack.Instance.attackObjectAnimator = swordAnimator;
    }
}
