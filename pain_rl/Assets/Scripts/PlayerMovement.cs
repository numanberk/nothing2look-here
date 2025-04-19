using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] public float speed;

        
        public float baseSpeed;


        private Animator animator;
        private PlayerAttack attackScript;
        private Sword swordScript;
        private Vector2 moveDir = Vector2.zero;
        public int animDirection;
        private GameObject Player;

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
    }


    private void Update()
    {
        HandleMovement();

        if(moveDir.magnitude > 0 && !attackScript.isAttacking)
        {
            if(attackScript.sword && !Player.GetComponentInChildren<Sword>().isCharging)
            {
                animator.SetInteger("Direction", animDirection);
            }
            
        }
        else
        {
            return;
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
}

