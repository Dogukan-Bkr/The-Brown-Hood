using System;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public static PlayerMovementController instance;
    bool isDead;
    public GameObject normalPlayer, swordPlayer;
    // Hareket ayarlarý
    public float speed = 8f;
    public float firstJumpForce = 9f;
    public float doubleJumpForce = 12f;
    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 2f;

    // Geri itilme (Back Leash) deðiþkenleri
    public float backLeashTime, backLeashForce, backLeashCounter;

    // Animator ve Sprite Renderer bileþenleri
    public Animator normalAnim , swordAnim;
    public SpriteRenderer normalSpriteRenderer , swordSpirte;

    // Zýplama deðiþkenleri
    public int maxJumps = 2;
    private int jumpCount = 0;

    // Fizik bileþeni
    private Rigidbody2D rb;

    // Zemin kontrolü
    private bool isGrounded;
    private bool direction; // Oyuncunun baktýðý yön (true: sað, false: sol)

    // Dash (Atýlma) deðiþkenleri
    public float dashSpeed = 15f; // Dash hýzý
    public float dashDuration = 0.3f; // Dash süresi
    private bool isDashing = false; // Þu an dash yapýlýyor mu?
    private float dashTime = 0f; // Dash zamanlayýcýsý
    


    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        isDead = false;
        // Sprite Renderer bileþeni atanmýþ mý kontrol et, eðer yoksa al
        if (normalSpriteRenderer == null)
        {
            normalSpriteRenderer = GetComponent<SpriteRenderer>();
        }
        else if (swordSpirte == null)
        {
            swordSpirte = GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (backLeashCounter <= 0) // Geri itilme süresi dolduysa normal hareketi çalýþtýr
        {
            Move();
            Jump();
            CheckMoveDirection();
            Dash();
            if(SwordController.instance != null)
                SwordController.instance.SwordAttack();
            // Geri itilme (back leash) sonrasý saydamlýk sýfýrlanýyor
            if (normalPlayer.activeSelf)
            {
                normalSpriteRenderer.color = new Color(normalSpriteRenderer.color.r, normalSpriteRenderer.color.g, normalSpriteRenderer.color.b, 1f);
            }
            else
            {
                swordSpirte.color = new Color(swordSpirte.color.r, swordSpirte.color.g, swordSpirte.color.b, 1f);
            }
        }

        else
        {
            // Geri itilme süresi devam ederken karakterin geriye gitmesini saðla
            backLeashCounter -= Time.deltaTime;
            if (direction)
                rb.linearVelocity = new Vector2(-backLeashForce, rb.linearVelocity.y);
            else
                rb.linearVelocity = new Vector2(backLeashForce, rb.linearVelocity.y);
        }
    }

    // Oyuncunun yatay hareketini yönetir
    void Move()
    {
        if (!isDashing) // Dash sýrasýnda hareket etme
        {
            float move = Input.GetAxisRaw("Horizontal"); // Klavyeden sað/sol tuþlarý alýnýr
            rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y); // Karakter hareket ettirilir

            // Animator’a hýz bilgisini göndererek animasyonlarý tetikle
            if (normalPlayer.activeSelf)
            {
                normalAnim.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
            }
            else
            {
                swordAnim.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
            }
        }
    }

    // Karakterin yönünü deðiþtirir
    void CheckMoveDirection()
    {
        if (rb.linearVelocity.x < 0) // Sol tarafa bakýyor
        {
            transform.localScale = new Vector3(-1, 1, 1);
            direction = false;
        }
        else if (rb.linearVelocity.x > 0) // Sað tarafa bakýyor
        {
            transform.localScale = Vector3.one;
            direction = true;
        }
    }

    // Zýplama fonksiyonu
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps) // Eðer zýplama tuþuna basýldýysa ve zýplama hakký varsa
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Önceki düþüþ hýzýný sýfýrla

            // Ýlk zýplama mý yoksa çift zýplama mý olduðunu kontrol et
            float jumpPower = (jumpCount == 0) ? firstJumpForce : doubleJumpForce;
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse); // Zýplama kuvveti uygula

            jumpCount++; // Zýplama sayýsýný artýr
        }

        // Daha gerçekçi bir zýplama eðrisi için ekstra fiziksel kuvvet uygula
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // Animator’a zemin durumu ve zýplama kuvveti bilgisini gönder
        if (normalPlayer.activeSelf)
        {
            normalAnim.SetBool("isGrounded", isGrounded);
            normalAnim.SetFloat("firstJumpForce", rb.linearVelocity.y);
        }
        else
        {
            swordAnim.SetBool("isGrounded", isGrounded);
            swordAnim.SetFloat("firstJumpForce", rb.linearVelocity.y);
        }
        
    }

    // Dash (Atýlma) fonksiyonu
    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isDashing) // Eðer E tuþuna basýldýysa ve þu an Dash yapýlmýyorsa
        {
            DashCheck(); // Dash fonksiyonunu çaðýr
        }

        if (isDashing) // Eðer þu an Dash yapýlýyorsa
        {
            dashTime -= Time.deltaTime; // Dash süresini azalt
            if (dashTime <= 0) // Dash süresi dolduðunda
            {
                isDashing = false; // Dash’i bitir
            }
        }
    }

    // Yere deðdiðinde zýplama hakkýný sýfýrla
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // Eðer yere temas ettiyse
        {
            isGrounded = true;
            jumpCount = 0; // Zýplama hakkýný sýfýrla
        }
    }

    // Zeminden ayrýldýðýnda isGrounded deðiþkenini false yap
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // Karakter geri itilme durumuna geçtiðinde çalýþýr
    public void BackLeash()
    {
        backLeashCounter = backLeashTime; // Geri itilme zamanýný baþlat
        if (normalPlayer.activeSelf)
        {
            normalSpriteRenderer.color = new Color(normalSpriteRenderer.color.r, normalSpriteRenderer.color.g, normalSpriteRenderer.color.b, 0.5f); // Karakteri yarý saydam yap
        }
        else
        {
            swordSpirte.color = new Color(swordSpirte.color.r, swordSpirte.color.g, swordSpirte.color.b, 0.5f); // Karakteri yarý saydam yap
        }
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Karakterin hýzýný sýfýrla
    }

    // Dash kontrolü ve uygulanmasý
    void DashCheck()
    {
        isDashing = true; // Dash aktif hale getirildi
        dashTime = dashDuration; // Dash süresi baþlatýldý

        // Dash animasyonunu tetikle
        if (normalPlayer.activeSelf)
        {
            normalAnim.SetTrigger("dash");
        }
        else
        {
            swordAnim.SetTrigger("dash");
        }
        

        // Dash yönünü belirle (karakterin baktýðý yöne göre)
        if (direction)
        {
            rb.linearVelocity = new Vector2(dashSpeed, rb.linearVelocity.y); // Saða Dash
        }
        else
        {
            rb.linearVelocity = new Vector2(-dashSpeed, rb.linearVelocity.y); // Sola Dash
        }
    }

    public void PlayerDead()
    {
        isDead = true;
        // Oyuncu öldüðünde animasyonlarý tetikle
        if (normalPlayer.activeSelf)
        {
            normalAnim.SetTrigger("isDead");
        }
        else
        {
            swordAnim.SetTrigger("isDead");
        }
        
    }

}
