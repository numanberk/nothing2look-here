using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] public float speed;


        private Animator animator;
        private PlayerAttack attackScript;

        private Vector2 moveDir = Vector2.zero;
        private int animDirection;

    private void Start()
    {
            animator = GetComponent<Animator>();
            attackScript = GetComponent<PlayerAttack>();

            if (animator != null )
            {
                animDirection = 2;
                animator.SetLayerWeight(1, 0);
            }
    }


    private void Update()
    {
        HandleMovement();

        if(moveDir.magnitude > 0 && !attackScript.isAttacking)
        {
            animator.SetInteger("Direction", animDirection);
        }
        else
        {
            return;
        }
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

