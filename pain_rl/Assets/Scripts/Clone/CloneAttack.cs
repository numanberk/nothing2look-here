using System.Drawing;
using UnityEngine;

public class CloneAttack : MonoBehaviour
{
    public CloneSword cloneSword;
    public ClonePunch clonePunch;
    public Animator cloneAnim;

    private Animator playerAnim;
    public int lastAttackInt = -1;
    private bool lastIsCharging = false;
    private int lastDirection = -1;
    private bool lastIdle = false;
    private int lastMirroredAttackInt = -1;
    public Vector2 attackDir;
    public Vector2 lastAttackDir;

    void Start()
    {
        playerAnim = PlayerAttack.Instance.anim;
        cloneAnim = GetComponent<Animator>();
        cloneSword = GetComponentInChildren<CloneSword>();
        clonePunch = GetComponentInChildren<ClonePunch>();

        if(PlayerAttack.Instance.sword)
        {
            PlayerAttack.OnPlayerAttacked += MirrorSwing;
        }

        if(PlayerAttack.Instance.punch)
        {
            PlayerAttack.OnPlayerAttackedPunch += MirrorPunch;
        }
        


    }

    void OnDestroy()
    {
        if (PlayerAttack.Instance.sword)
        {
            PlayerAttack.OnPlayerAttacked -= MirrorSwing;
        }

        if (PlayerAttack.Instance.punch)
        {
            PlayerAttack.OnPlayerAttackedPunch -= MirrorPunch;
        }
    }

    public int GetMouseQuadrant(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Get angle in degrees

        if (angle >= -45 && angle < 45) return 3;  // Right side
        if (angle >= 45 && angle < 135) return 0;  // Top side
        if (angle >= -135 && angle < -45) return 2; // Bottom side
        return 1; // Left side
    }

    void Update()
    {
        if (playerAnim == null || cloneAnim == null) return;

        bool isAttacking = PlayerAttack.Instance.isAttacking;
        bool canAttack = PlayerAttack.Instance.canAttack;
        bool isIdle = PlayerAttack.Instance.isIdle;
        int direction = GetMouseQuadrant(attackDir); //BU UPDATE İÇİ OLDUĞUNDAN ŞU AN ATTACK YAPMIYOR OLSAK BİLE CLONE ÇEVRESİNDE MOUSE DÖNDÜĞÜNDE OTOMATİK DÖNÜYOR :D


        Vector2 clonePos = (Vector2)this.gameObject.transform.position;
        Vector2 playerPos = (Vector2)PlayerAttack.Instance.Player.transform.position;
        Vector2 mousePos = (Vector2)Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
        if (!PlayerAttack.Instance.attackDirLocked)
        {
            attackDir = (mousePos - clonePos).normalized; // CLONE POZİSYONU MU PLAYER POZİSYONUNA GÖRE ATAK MI????
            lastAttackDir = attackDir;
        }
        else
        {
            attackDir = lastAttackDir;
        }

        // DIRECTION SYNC
        if (direction != lastDirection)
        {
            cloneAnim.SetInteger("Direction", direction);
            lastDirection = direction;
        }


        if (PlayerAttack.Instance.sword)
        {
            int playerAttackInt = PlayerAttack.Instance.attackInt;
            bool isCharging = playerAnim.GetBool("isCharging");

            //CHARGE SYNC
            cloneAnim.SetBool("isCharging", isCharging);


            // ATTACK SYNC
            if (isAttacking && playerAttackInt != lastMirroredAttackInt && canAttack)
            {
                cloneSword.PerformSwing(attackDir, playerAttackInt);
                lastMirroredAttackInt = playerAttackInt;
            }

            if (!isAttacking)
            {
                lastMirroredAttackInt = -1;
            }


            // IDLE SYNC
            if (isIdle && !lastIdle)
            {
                cloneSword.animSword.SetTrigger("idle");
                lastIdle = true;
            }
            else if (!isIdle)
            {
                lastIdle = false;
            }

            // Optional: if your animation logic relies on attackInt
            cloneSword.animSword.SetInteger("attackInt", playerAttackInt);
        }

        else if (PlayerAttack.Instance.punch)
        {
            // ATTACK SYNC
            if (isAttacking && canAttack)
            {
                clonePunch.PerformPunch(attackDir, PlayerAttack.Instance.isCharged);
            }


            // IDLE SYNC
            if (isIdle && !lastIdle)
            {
                clonePunch.animPunch.SetTrigger("idle");
                lastIdle = true;
            }
            else if (!isIdle)
            {
                lastIdle = false;
            }
        }
        
    }

    void MirrorSwing(int attackInt)
    {
        cloneSword.PerformSwing(attackDir, attackInt);
    }


    void MirrorPunch(bool isCharged)
    {

        clonePunch.PerformPunch(attackDir, isCharged);
        clonePunch.directionAll = attackDir;
    }

    }
