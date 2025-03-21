using System;
using System.Collections;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public static PlayerMovementController instance;
    bool isDead;
    public GameObject normalPlayer, swordPlayer, spearPlayer, bowPlayer;
    public WeaponType currentWeapon = WeaponType.None;
    // Hareket ayarlar�
    public float speed = 8f;
    public float firstJumpForce = 9f;
    public float doubleJumpForce = 12f;
    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 2f;

    // Geri itilme (Back Leash) de�i�kenleri
    public float backLeashTime, backLeashForce, backLeashCounter;

    // Animator ve Sprite Renderer bile�enleri
    public Animator normalAnim, swordAnim, spearAnim, bowAnim;
    public SpriteRenderer normalSpriteRenderer, swordSprite, spearSprite, bowSprite;

    // Z�plama de�i�kenleri
    public int maxJumps = 2;
    private int jumpCount = 0;
    // T�rmanma de�i�kenleri
    public float climbspeed = 3f;
    private bool isOnLadder, isClimbing = false;
    private float qCooldownTime = 0.35f;
    private float qTime = 0f;
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
    int swordCounter = 0;
    // Silah de�i�tirme sonras� bekleme s�resi
    private float weaponSwitchCooldown = 0.3f;
    private float weaponSwitchTime = 0f;
    // �l�m efekti
    public GameObject deathEffect;

    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        isDead = false;
        // Sprite Renderer bile�eni atanm�� m� kontrol et, e�er yoksa al
        if (normalSpriteRenderer == null && normalPlayer != null)
        {
            normalSpriteRenderer = normalPlayer.GetComponent<SpriteRenderer>();
        }
        if (swordSprite == null && swordPlayer != null)
        {
            swordSprite = swordPlayer.GetComponent<SpriteRenderer>();
        }
        if (spearSprite == null && spearPlayer != null)
        {
            spearSprite = spearPlayer.GetComponent<SpriteRenderer>();
        }
        if (bowSprite == null && bowPlayer != null)
        {
            bowSprite = bowPlayer.GetComponent<SpriteRenderer>();
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
            Climb();
            HandleWeaponAttack();
            HandleWeaponSwitch();
            // Geri itilme (back leash) sonras� saydaml�k s�f�rlan�yor
            if (normalPlayer.activeSelf)
            {
                normalSpriteRenderer.color = new Color(normalSpriteRenderer.color.r, normalSpriteRenderer.color.g, normalSpriteRenderer.color.b, 1f);
            }
            else if (swordPlayer.activeSelf)
            {
                swordSprite.color = new Color(swordSprite.color.r, swordSprite.color.g, swordSprite.color.b, 1f);
                swordCounter++;
            }
            else if (spearPlayer.activeSelf)
            {
                spearSprite.color = new Color(spearSprite.color.r, spearSprite.color.g, spearSprite.color.b, 1f);
            }
            else if (bowPlayer.activeSelf)
            {
                bowSprite.color = new Color(bowSprite.color.r, bowSprite.color.g, bowSprite.color.b, 1f);
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

        if (qTime > 0)
        {
            qTime -= Time.deltaTime;
        }

        // Silah de�i�tirme cooldown kontrol�
        if (weaponSwitchTime > 0)
        {
            weaponSwitchTime -= Time.deltaTime;
        }
    }


    void HandleWeaponAttack()
    {
        if (SwordController.instance != null && currentWeapon == WeaponType.Sword)
        {
            SwordController.instance.SwordAttack();
        }
        if (SpearController.instance != null && currentWeapon == WeaponType.Spear)
        {
            SpearController.instance.SpearAttack();
        }
        // BowController.instance.ShootArrow() �a�r�s�n� kald�rd�k
    }

    void HandleWeaponSwitch()
{   if (GameManager.instance != null && !isClimbing && !isDashing && weaponSwitchTime <= 0) // Merdiven ��karken ve dash sonras� bekleme s�resinde silah de�i�tirmeyi engelle
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && qTime <= 0 )
        {
            SetActiveWeapon(WeaponType.None);
            qTime = qCooldownTime;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && swordCounter > 1)
        {
            SetActiveWeapon(WeaponType.Sword);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && GameManager.instance.spearCount > 0)
        {
            SetActiveWeapon(WeaponType.Spear);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && GameManager.instance.arrowCount > 0)
        {
            SetActiveWeapon(WeaponType.Bow);
        }
    }
}


    void SetActiveWeapon(WeaponType weaponType)
    {
        currentWeapon = weaponType;
        normalPlayer.SetActive(weaponType == WeaponType.None);
        swordPlayer.SetActive(weaponType == WeaponType.Sword);
        spearPlayer.SetActive(weaponType == WeaponType.Spear);
        bowPlayer.SetActive(weaponType == WeaponType.Bow);
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
            else if (swordPlayer.activeSelf)
            {
                swordAnim.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
            }
            else if (spearPlayer.activeSelf)
            {
                spearAnim.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
            }
            else if (bowPlayer.activeSelf)
            {
                bowAnim.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
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
        else if (swordPlayer.activeSelf)
        {
            swordAnim.SetBool("isGrounded", isGrounded);
            swordAnim.SetFloat("firstJumpForce", rb.linearVelocity.y);
        }
        else if (spearPlayer.activeSelf)
        {
            spearAnim.SetBool("isGrounded", isGrounded);
            spearAnim.SetFloat("firstJumpForce", rb.linearVelocity.y);
        }
        else if (bowPlayer.activeSelf)
        {
            bowAnim.SetBool("isGrounded", isGrounded);
            bowAnim.SetFloat("firstJumpForce", rb.linearVelocity.y);
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
                weaponSwitchTime = weaponSwitchCooldown;
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
        else if (swordPlayer.activeSelf)
        {
            swordSprite.color = new Color(swordSprite.color.r, swordSprite.color.g, swordSprite.color.b, 0.5f); // Karakteri yar� saydam yap
        }
        else if (spearPlayer.activeSelf)
        {
            spearSprite.color = new Color(spearSprite.color.r, spearSprite.color.g, spearSprite.color.b, 0.5f); // Karakteri yar� saydam yap
        }
        else if (bowPlayer.activeSelf)
        {
            bowSprite.color = new Color(bowSprite.color.r, bowSprite.color.g, bowSprite.color.b, 0.5f); // Karakteri yar� saydam yap
        }
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Karakterin h�z�n� s�f�rla
    }

    // Merdiven ��kma fonksiyonu
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder")) isOnLadder = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder")) isOnLadder = false;
    }

    void Climb()
    {
        if (normalPlayer.activeSelf)
        {
            if (isOnLadder && Input.GetKey(KeyCode.Q) && qTime <= 0)
            {
                isClimbing = true;
                float h = Input.GetAxis("Vertical");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, h * climbspeed);
                rb.gravityScale = 0;
                normalAnim.SetBool("isClimbing", true);
                normalAnim.SetFloat("climbSpeed", Mathf.Abs(rb.linearVelocity.y));
            }
            else
            {
                isClimbing = false;
                normalAnim.SetBool("isClimbing", false);
                rb.gravityScale = 3; // Yer�ekimini hemen devreye sok
                if (Input.GetKeyUp(KeyCode.Q))
                {
                    qTime = qCooldownTime; // Q tu�u i�in bekleme s�resi
                }
            }
        }
        else if (swordPlayer.activeSelf)
        {
            if (isOnLadder && Input.GetKey(KeyCode.Q) && qTime <= 0)
            {
                isClimbing = true;
                float h = Input.GetAxis("Vertical");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, h * climbspeed);
                rb.gravityScale = 0;
                swordAnim.SetBool("isClimbing", true);
                swordAnim.SetFloat("climbSpeed", Mathf.Abs(rb.linearVelocity.y));
            }
            else
            {
                isClimbing = false;
                swordAnim.SetBool("isClimbing", false);
                rb.gravityScale = 3; // Yer�ekimini hemen devreye sok
                if (Input.GetKeyUp(KeyCode.Q))
                {
                    qTime = qCooldownTime; // Q tu�u i�in bekleme s�resi
                }
            }
        }
        else if (spearPlayer.activeSelf)
        {
            if (isOnLadder && Input.GetKey(KeyCode.Q) && qTime <= 0)
            {
                isClimbing = true;
                float h = Input.GetAxis("Vertical");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, h * climbspeed);
                rb.gravityScale = 0;
                spearAnim.SetBool("isClimbing", true);
                spearAnim.SetFloat("climbSpeed", Mathf.Abs(rb.linearVelocity.y));
            }
            else
            {
                isClimbing = false;
                spearAnim.SetBool("isClimbing", false);
                rb.gravityScale = 3; // Yer�ekimini hemen devreye sok
                if (Input.GetKeyUp(KeyCode.Q))
                {
                    qTime = qCooldownTime; // Q tu�u i�in bekleme s�resi
                }
            }
        }
        else if (bowPlayer.activeSelf)
        {
            if (isOnLadder && Input.GetKey(KeyCode.Q) && qTime <= 0)
            {
                isClimbing = true;
                float h = Input.GetAxis("Vertical");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, h * climbspeed);
                rb.gravityScale = 0;
                bowAnim.SetBool("isClimbing", true);
                bowAnim.SetFloat("climbSpeed", Mathf.Abs(rb.linearVelocity.y));
            }
            else
            {
                isClimbing = false;
                bowAnim.SetBool("isClimbing", false);
                rb.gravityScale = 3; // Yer�ekimini hemen devreye sok
                if (Input.GetKeyUp(KeyCode.Q))
                {
                    qTime = qCooldownTime; // Q tu�u i�in bekleme s�resi
                }
            }
        }
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
        else if (swordPlayer.activeSelf)
        {
            swordAnim.SetTrigger("dash");
        }
        else if (spearPlayer.activeSelf)
        {
            spearAnim.SetTrigger("dash");
        }
        else if (bowPlayer.activeSelf)
        {
            bowAnim.SetTrigger("dash");
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
        else if (swordPlayer.activeSelf)
        {
            swordAnim.SetTrigger("isDead");
        }
        else if (spearPlayer.activeSelf)
        {
            spearAnim.SetTrigger("isDead");
        }
        else if (bowPlayer.activeSelf)
        {
            bowAnim.SetTrigger("isDead");
        }

        // �l�m efektini 1 saniye sonra olu�tur
        StartCoroutine(CreateDeathEffect());
    }

    IEnumerator CreateDeathEffect()
    {
        yield return new WaitForSeconds(0.5f);
        Instantiate(deathEffect, transform.position, Quaternion.identity);
    }

    public void StopPlayer()
    {
        rb.linearVelocity = Vector2.zero;
        if (normalPlayer.activeSelf)
        {
            normalAnim.SetFloat("speed", 0);
        }
        else if (swordPlayer.activeSelf)
        {
            swordAnim.SetFloat("speed", 0);
        }
        else if (spearPlayer.activeSelf)
        {
            spearAnim.SetFloat("speed", 0);
        }
        else if (bowPlayer.activeSelf)
        {
            bowAnim.SetFloat("speed", 0);
        }
    }

    public enum WeaponType
    {
        None,
        Sword,
        Spear,
        Bow
    }
}

