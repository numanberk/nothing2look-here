using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private EventManager eventManager;


    private Rigidbody2D rb;
    private Animator anim;
    private float scaleX;
    private float scaleY;
    private float moveHorizontal;
    private float moveVertical;
    private Vector2 movement;
    private int facingDirection = 1; // (1) -> sa� . (-1) -> sol

    [SerializeField] float moveSpeed; 



    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        eventManager = Object.FindFirstObjectByType<EventManager>();
        scaleY = gameObject.transform.localScale.y;
        scaleX = gameObject.transform.localScale.x;
    }

    private void Update()
    {
        if (eventManager!= null)
        {
            if (eventManager.isPlayerDead)
            {
                movement = Vector2.zero;
                if (anim != null)
                    anim.SetFloat("Velocity", 0);
                return;
            }

            moveHorizontal = Input.GetAxisRaw("Horizontal");
            moveVertical = Input.GetAxisRaw("Vertical");
            movement = new Vector2(moveHorizontal, moveVertical);


            if (movement.magnitude > 1)
                movement.Normalize();


            if (anim != null)
                anim.SetFloat("Velocity", movement.magnitude);
            
            if(movement.x != 0)
                facingDirection = movement.x > 0 ? 1 : -1;


            transform.localScale = new Vector2(scaleX * facingDirection, scaleY);
        }


        else if (eventManager == null)
            Debug.Log("Event Manager can not be found.");
    }


    private void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }
}
