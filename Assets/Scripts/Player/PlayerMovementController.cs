using System;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public static PlayerMovementController instance;
    bool isDead;
    public GameObject normalPlayer, swordPlayer;
    // Hareket ayarlar�
    public float speed = 8f;
    public float firstJumpForce = 9f;
    public float doubleJumpForce = 12f;
    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 2f;

    // Geri itilme (Back Leash) de�i�kenleri
    public float backLeashTime, backLeashForce, backLeashCounter;

    // Animator ve Sprite Renderer bile�enleri
    public Animator normalAnim , swordAnim;
    public SpriteRenderer normalSpriteRenderer , swordSpirte;

    // Z�plama de�i�kenleri
    public int maxJumps = 2;
    private int jumpCount = 0;

    // Fizik bile�eni
    private Rigidbody2D rb;

    // Zemin kontrol�
    private bool isGrounded;
    private bool direction; // Oyuncunun bakt��� y�n (true: sa�, false: sol)

    // Dash (At�lma) de�i�kenleri
    public float dashSpeed = 15f; // Dash h�z�
    public float dashDuration = 0.3f; // Dash s�resi
    private bool isDashing = false; // �u an dash yap�l�yor mu?
    private float dashTime = 0f; // Dash zamanlay�c�s�
    


    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        isDead = false;
        // Sprite Renderer bile�eni atanm�� m� kontrol et, e�er yoksa al
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
        if (backLeashCounter <= 0) // Geri itilme s�resi dolduysa normal hareketi �al��t�r
        {
            Move();
            Jump();
            CheckMoveDirection();
            Dash();
            if(SwordController.instance != null)
                SwordController.instance.SwordAttack();
            // Geri itilme (back leash) sonras� saydaml�k s�f�rlan�yor
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
            // Geri itilme s�resi devam ederken karakterin geriye gitmesini sa�la
            backLeashCounter -= Time.deltaTime;
            if (direction)
                rb.linearVelocity = new Vector2(-backLeashForce, rb.linearVelocity.y);
            else
                rb.linearVelocity = new Vector2(backLeashForce, rb.linearVelocity.y);
        }
    }

    // Oyuncunun yatay hareketini y�netir
    void Move()
    {
        if (!isDashing) // Dash s�ras�nda hareket etme
        {
            float move = Input.GetAxisRaw("Horizontal"); // Klavyeden sa�/sol tu�lar� al�n�r
            rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y); // Karakter hareket ettirilir

            // Animator�a h�z bilgisini g�ndererek animasyonlar� tetikle
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

    // Karakterin y�n�n� de�i�tirir
    void CheckMoveDirection()
    {
        if (rb.linearVelocity.x < 0) // Sol tarafa bak�yor
        {
            transform.localScale = new Vector3(-1, 1, 1);
            direction = false;
        }
        else if (rb.linearVelocity.x > 0) // Sa� tarafa bak�yor
        {
            transform.localScale = Vector3.one;
            direction = true;
        }
    }

    // Z�plama fonksiyonu
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps) // E�er z�plama tu�una bas�ld�ysa ve z�plama hakk� varsa
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // �nceki d���� h�z�n� s�f�rla

            // �lk z�plama m� yoksa �ift z�plama m� oldu�unu kontrol et
            float jumpPower = (jumpCount == 0) ? firstJumpForce : doubleJumpForce;
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse); // Z�plama kuvveti uygula

            jumpCount++; // Z�plama say�s�n� art�r
        }

        // Daha ger�ek�i bir z�plama e�risi i�in ekstra fiziksel kuvvet uygula
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // Animator�a zemin durumu ve z�plama kuvveti bilgisini g�nder
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

    // Dash (At�lma) fonksiyonu
    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isDashing) // E�er E tu�una bas�ld�ysa ve �u an Dash yap�lm�yorsa
        {
            DashCheck(); // Dash fonksiyonunu �a��r
        }

        if (isDashing) // E�er �u an Dash yap�l�yorsa
        {
            dashTime -= Time.deltaTime; // Dash s�resini azalt
            if (dashTime <= 0) // Dash s�resi doldu�unda
            {
                isDashing = false; // Dash�i bitir
            }
        }
    }

    // Yere de�di�inde z�plama hakk�n� s�f�rla
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // E�er yere temas ettiyse
        {
            isGrounded = true;
            jumpCount = 0; // Z�plama hakk�n� s�f�rla
        }
    }

    // Zeminden ayr�ld���nda isGrounded de�i�kenini false yap
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // Karakter geri itilme durumuna ge�ti�inde �al���r
    public void BackLeash()
    {
        backLeashCounter = backLeashTime; // Geri itilme zaman�n� ba�lat
        if (normalPlayer.activeSelf)
        {
            normalSpriteRenderer.color = new Color(normalSpriteRenderer.color.r, normalSpriteRenderer.color.g, normalSpriteRenderer.color.b, 0.5f); // Karakteri yar� saydam yap
        }
        else
        {
            swordSpirte.color = new Color(swordSpirte.color.r, swordSpirte.color.g, swordSpirte.color.b, 0.5f); // Karakteri yar� saydam yap
        }
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Karakterin h�z�n� s�f�rla
    }

    // Dash kontrol� ve uygulanmas�
    void DashCheck()
    {
        isDashing = true; // Dash aktif hale getirildi
        dashTime = dashDuration; // Dash s�resi ba�lat�ld�

        // Dash animasyonunu tetikle
        if (normalPlayer.activeSelf)
        {
            normalAnim.SetTrigger("dash");
        }
        else
        {
            swordAnim.SetTrigger("dash");
        }
        

        // Dash y�n�n� belirle (karakterin bakt��� y�ne g�re)
        if (direction)
        {
            rb.linearVelocity = new Vector2(dashSpeed, rb.linearVelocity.y); // Sa�a Dash
        }
        else
        {
            rb.linearVelocity = new Vector2(-dashSpeed, rb.linearVelocity.y); // Sola Dash
        }
    }

    public void PlayerDead()
    {
        isDead = true;
        // Oyuncu �ld���nde animasyonlar� tetikle
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
