using UnityEngine;

public class CloneAttack : MonoBehaviour
{
    public CloneSword cloneSword;
    public Animator cloneAnim;

    private Animator playerAnim;
    public int lastAttackInt = -1;
    private bool lastIsCharging = false;
    private int lastDirection = -1;
    private bool lastIdle = false;
    private int lastMirroredAttackInt = -1;
    public Vector2 attackDir;

    void Start()
    {
        playerAnim = PlayerAttack.Instance.anim;
        cloneAnim = GetComponent<Animator>();
        cloneSword = GetComponentInChildren<CloneSword>();

        PlayerAttack.OnPlayerAttacked += MirrorSwing;


    }

    void OnDestroy()
    {
        PlayerAttack.OnPlayerAttacked -= MirrorSwing;
    }

    void Update()
    {
        if (playerAnim == null || cloneAnim == null) return;

        int playerAttackInt = PlayerAttack.Instance.attackInt;
        bool isAttacking = PlayerAttack.Instance.isAttacking;
        bool isCharging = playerAnim.GetBool("isCharging");
        int direction = playerAnim.GetInteger("Direction");
        bool canAttack = PlayerAttack.Instance.canAttack;
        bool isIdle = PlayerAttack.Instance.isIdle;


        Vector2 clonePos = (Vector2)this.gameObject.transform.position;
        Vector2 playerPos = (Vector2)PlayerAttack.Instance.Player.transform.position;
        Vector2 mousePos = (Vector2)Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
        attackDir = (mousePos - playerPos).normalized; // CLONE POZİSYONU MU PLAYER POZİSYONUNA GÖRE ATAK MI????


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

        //CHARGE SYNC
        cloneAnim.SetBool("isCharging", isCharging);

        // DIRECTION SYNC
        if (direction != lastDirection)
        {
            cloneAnim.SetInteger("Direction", direction);
            lastDirection = direction;
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

    void MirrorSwing(int attackInt)
    {
        cloneSword.PerformSwing(attackDir, attackInt);
    }

}