using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pathfinding.Util.RetainedGizmos;

    public class PlayerMovement : MonoBehaviour
    {
        public static PlayerMovement instance;
        [SerializeField] public float speed;

        
        public float baseSpeed;
        public bool canMove = true;
        public bool isDashing;


        private Animator animator;
        private PlayerAttack attackScript;
        private Sword swordScript;
        private Vector2 moveDir = Vector2.zero;
        public int animDirection;
        private GameObject Player;
        private bool wasDashing;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
            animator = GetComponent<Animator>();
            attackScript = GetComponent<PlayerAttack>();
            swordScript = GetComponentInChildren<Sword>();
            if (animator != null )
            {
                animDirection = 2;
            }

            baseSpeed = speed;
            Player = this.gameObject;
            canMove = true;

    }


    private void Update()
    {
        if(canMove)
        {
            HandleMovement();
        }
            
            CheckDashDamage();

            if (moveDir.magnitude > 0 && !attackScript.isAttacking)
            {
                if (attackScript.sword && Player.GetComponentInChildren<Sword>().isCharging)
                {
                    return;
                }

                animator.SetInteger("Direction", animDirection);
            }


            animator.SetFloat("MoveX", moveDir.x);
            animator.SetFloat("MoveY", moveDir.y);


    }


    private void HandleMovement()
    {
        moveDir = Vector2.zero;
       

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveDir.x = -1;
            animDirection = 1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveDir.x = 1;
            animDirection = 3;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            moveDir.y = 1;
            animDirection = 0;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            moveDir.y = -1;
            animDirection = 2;
        }

        moveDir.Normalize();

        // Use attack direction if attacking, otherwise use movement direction
        
        animator.SetBool("isMoving", moveDir.magnitude > 0);

        GetComponent<Rigidbody2D>().linearVelocity = speed * moveDir;
    }

    private void CheckDashDamage()
    {
        if (ShatterdashSkill.Instance != null && ShatterdashSkill.Instance.isDashing)
        {
            // Define size of your hitbox
            Vector2 boxSize = GetComponent<BoxCollider2D>().size * 1.2f; //ayný yap drawgizmo
            Vector2 offset = GetComponent<BoxCollider2D>().offset;
            Collider2D[] hits = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + offset.x, transform.position.y + offset.y), boxSize, 0f);
            var baseDamage = PlayerAttack.Instance.attackDamage * PlayerAttack.Instance.weaponSkillDamageMultiplier;


            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    Health enemyHealth = hit.GetComponent<Health>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.Hit(Mathf.RoundToInt(baseDamage * PlayerAttack.Instance.damageMultiplierPain * ShatterdashSkill.Instance.dashDamageMultiplier), 100);
                    }
                }
                if (hit.CompareTag("Barrel"))
                {
                    Health barrelHealth = hit.GetComponent<Health>();
                    if (barrelHealth != null)
                    {
                        barrelHealth.Hit(Mathf.RoundToInt(baseDamage * PlayerAttack.Instance.damageMultiplierPain * ShatterdashSkill.Instance.dashDamageMultiplier), 100);
                        hit.GetComponent<EntitySFX>().BarrelHitSFX();
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (ShatterdashSkill.Instance != null && ShatterdashSkill.Instance.isDashing)
        {
            Gizmos.color = Color.red;
            Vector2 boxSize = GetComponent<BoxCollider2D>().size * 1.2f; // Same as you use in OverlapBoxAll
            Gizmos.DrawWireCube(transform.position, boxSize);
        }
    }


}

