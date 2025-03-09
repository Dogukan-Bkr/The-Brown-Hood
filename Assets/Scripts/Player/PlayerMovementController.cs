using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public static PlayerMovementController instance;
    public float speed = 8f;
    public float firstJumpForce = 9f;
    public float doubleJumpForce = 12f;
    public float fallMultiplier = 3f; 
    public float lowJumpMultiplier = 2f;
    public float backLeashTime, backLeashForce,backLeashCounter;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public int maxJumps = 2;
    private int jumpCount = 0;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool direction;
    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
       if(backLeashCounter <= 0)
        {
            Move();
            Jump();
            CheckMoveDirection();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);

        }
        else
       {
            backLeashCounter -= Time.deltaTime;
            if (direction)
            {rb.linearVelocity = new Vector2(-backLeashForce, rb.linearVelocity.y);}else 
            { rb.linearVelocity = new Vector2(backLeashForce, rb.linearVelocity.y); }
       }
    }

    void Move()
    {
        float move = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
        
            animator.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
    }

    void CheckMoveDirection()
    {
        if (rb.linearVelocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            direction = false;
        }
        else if (rb.linearVelocity.x > 0)
        {
            transform.localScale = Vector3.one;
            direction = true;
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount<maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

            float jumpPower = (jumpCount == 0) ? firstJumpForce : doubleJumpForce;
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

            jumpCount++;
        }
        if (rb.linearVelocity.y < 0) 
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump")) 
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("firstJumpForce", (rb.linearVelocity.y));
    }

   
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
    public void BackLeash()
    {
        backLeashCounter = backLeashTime;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f); 
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }


}
