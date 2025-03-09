using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public float speed = 8f;
    public float firstJumpForce = 9f;
    public float doubleJumpForce = 12f;
    public float fallMultiplier = 3f; 
    public float lowJumpMultiplier = 2f;
    public Animator animator;
    public int maxJumps = 2;
    private int jumpCount = 0;
    private Rigidbody2D rb;
    private bool isGrounded;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        move();
        jump();
    }

    void move()
    {
        float move = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
        if (rb.linearVelocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (rb.linearVelocity.x > 0)
        {
            transform.localScale = Vector3.one;
        }
            animator.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
    }
    void jump()
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

    void fall()
    {
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
}
