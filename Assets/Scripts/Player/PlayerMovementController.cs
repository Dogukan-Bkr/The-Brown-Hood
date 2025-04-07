using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovementController : MonoBehaviour
{
    public static PlayerMovementController instance;
    bool isDead;
    public GameObject normalPlayer, swordPlayer, spearPlayer, bowPlayer;
    public WeaponType currentWeapon = WeaponType.None;
    
    
    // Hareket ayarlarý
    public float speed = 8f;
    private float moveInput = 0f;
    public float firstJumpForce = 9f;
    public float doubleJumpForce = 12f;
    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 2f;
    private Vector3 originalScale;
    // Geri itilme (Back Leash) deðiþkenleri
    public float backLeashTime, backLeashForce, backLeashCounter;

    // Animator ve Sprite Renderer bileþenleri
    public Animator normalAnim, swordAnim, spearAnim, bowAnim;
    public SpriteRenderer normalSpriteRenderer, swordSprite, spearSprite, bowSprite;

    // Zýplama deðiþkenleri
    public int maxJumps = 2;
    private int jumpCount = 0;
    public Transform groundCheck; // Inspector'da karakterin altýna ekle
    public LayerMask groundLayer; // Sadece zemin olan nesneler için bir Layer Mask
    public float groundCheckDistance = 0.2f; // Kontrol edilecek alanýn yarýçapý
    private bool jumpButtonPressed = false; // Butona basýldýðýný kontrol eden deðiþken
    public Button jumpButton;
    // Týrmanma deðiþkenleri
    public float climbspeed = 3f;
    private bool isOnLadder, isClimbing = false;
    private float qCooldownTime = 0.35f;
    private float qTime = 0f;
    private float verticalInput = 0f;
    public float ladderMinX;
    public float ladderMaxX;
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
    private float dashCooldown = 1f; // Dash atmak için gereken süre
    private float lastDashTime; // Son dash zamaný
    private bool dashButtonPressed = false;
    public Button dashButton;
    int swordCounter = 0;
    // Silah deðiþtirme sonrasý bekleme süresi
    private float weaponSwitchCooldown = 0.3f;
    private float weaponSwitchTime = 0f;
    public Button swordButton;
    public Button spearButton;
    public Button bowButton;
    // Ölüm efekti
    public GameObject deathEffect;
    //NPC
    private bool isNearBlackSmith = false;
    private bool isNearTent = false;
    public GameObject blacksmithPanel,Tent;
    private bool canOpenPanel = true; // Panel açýlýp açýlamayacaðýný kontrol eder
    private float panelCooldownTime = 3f; // 3 saniye bekleme süresi
    private float lastPanelOpenTime = 0f; // Son panel açma zamaný
    private void Start()
    {
        if (swordButton != null)
        {
            swordButton.onClick.AddListener(OnSwordButtonClick);
        }if (spearButton != null)
        {
            spearButton.onClick.AddListener(OnSpearButtonClick);
        }
        if (bowButton != null)
        {
            bowButton.onClick.AddListener(OnBowButtonClick);
        }
        
    }
    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        isDead = false;
       

        // Sprite Renderer bileþeni atanmýþ mý kontrol et, eðer yoksa al
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
        
        if (backLeashCounter <= 0) // Geri itilme süresi dolduysa normal hareketi çalýþtýr
        {
            isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

            if (isGrounded)
            {
                jumpCount = 0; // Karakter zemine deðdiðinde zýplama hakký sýfýrlanýr
            }
            if (Input.GetKeyDown(KeyCode.Q) && isOnLadder) //Bilgisayar testleri bittiðinde kaldýrýlabilir.
            {
                isClimbing = !isClimbing;  
            }
            // Klavyeden giriþ al ve sadece tuþ basýlýyken hareket et
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                moveInput = -1f;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                moveInput = 1f;
            }
            else
            {
                moveInput = 0f; // Tuþ býrakýldýðýnda hareketi durdur
            }
            // Butonlara olaylarý baðlayalým
            // Kýlýç butonu 
            if (swordButton != null)
            {
                bool hasSword = swordCounter >= 1;
                swordButton.interactable = hasSword;
            }

            // Mýzrak butonu 
            if (spearButton != null)
            {
                bool hasSpear = GameManager.instance.spearCount > 0;
                spearButton.interactable = hasSpear;
            }

            // Yay butonu 
            if (bowButton != null)
            {
                bool hasBow = GameManager.instance.arrowCount > 0;
                bowButton.interactable = hasBow;
            }
            Jump();
            Move();
            CheckMoveDirection();
            Dash();
            Climb();
            HandleWeaponAttack();


            // BlackSmith panelini açma kontrolü
            // Eðer 3 saniyelik süre geçmiþse, panel açýlabilir
            if (Time.time - lastPanelOpenTime >= panelCooldownTime)
            {
                canOpenPanel = true;
            }

            // Blacksmith panelini açma
            if (isNearBlackSmith && Input.GetMouseButtonDown(0) && canOpenPanel)
            {
                OpenBlacksmithPanel();
                lastPanelOpenTime = Time.time; // Panel açýldý, zamaný kaydet
                canOpenPanel = false; // Panel açýlana kadar bekle
            }
            // Tent panelini açma
            else if (isNearTent && Input.GetMouseButtonDown(0) && canOpenPanel)
            {
                OpenTentPanel();
                lastPanelOpenTime = Time.time; // Panel açýldý, zamaný kaydet
                canOpenPanel = false; // Panel açýlana kadar bekle
            }

            // Geri itilme (back leash) sonrasý saydamlýk sýfýrlanýyor
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
            // Geri itilme süresi devam ederken karakterin geriye gitmesini saðla
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

        // Silah deðiþtirme cooldown kontrolü
        if (weaponSwitchTime > 0)
        {
            weaponSwitchTime -= Time.deltaTime;
        }

        // Mýzrak sayýsý sýfýra düþtüðünde silah deðiþtirme iþlemini kontrol et
        if (currentWeapon == WeaponType.Spear && GameManager.instance.spearCount <= 0)
        {
            SetActiveWeapon(WeaponType.None);
        }
    }




    public void OnSwordButtonClick()
    {
        if (GameManager.instance != null && !isClimbing && !isDashing && weaponSwitchTime <= 0 && qTime <= 0)
        {
            // Saldýrý sýrasýnda silah deðiþtirmeyi engelle
            if ((SwordController.instance != null && SwordController.instance.isAttacking) ||
                (SpearController.instance != null && (SpearController.instance.isAttacking || SpearController.instance.isAiming)) ||
                (BowController.instance != null && BowController.instance.isShooting))
            {
                return;
            }

            // Kýlýç geçiþi
            if (swordCounter > 1)
            {
                SetActiveWeapon(WeaponType.Sword);
                qTime = qCooldownTime;
            }
        }
    }

    // Mýzrak butonuna basýldýðýnda
    public void OnSpearButtonClick()
    {
        if (GameManager.instance != null && !isClimbing && !isDashing && weaponSwitchTime <= 0 && qTime <= 0)
        {
            // Saldýrý sýrasýnda silah deðiþtirmeyi engelle
            if ((SwordController.instance != null && SwordController.instance.isAttacking) ||
                (SpearController.instance != null && (SpearController.instance.isAttacking || SpearController.instance.isAiming)) ||
                (BowController.instance != null && BowController.instance.isShooting))
            {
                return;
            }

            // Mýzrak geçiþi
            if (GameManager.instance.spearCount > 0)
            {
                SetActiveWeapon(WeaponType.Spear);
                qTime = qCooldownTime;
            }
        }
    }

    // Yay butonuna basýldýðýnda
    public void OnBowButtonClick()
    {
        if (GameManager.instance != null && !isClimbing && !isDashing && weaponSwitchTime <= 0 && qTime <= 0)
        {
            // Saldýrý sýrasýnda silah deðiþtirmeyi engelle
            if ((SwordController.instance != null && SwordController.instance.isAttacking) ||
                (SpearController.instance != null && (SpearController.instance.isAttacking || SpearController.instance.isAiming)) ||
                (BowController.instance != null && BowController.instance.isShooting))
            {
                return;
            }

            // Yay geçiþi
            if (GameManager.instance.arrowCount > 0)
            {
                SetActiveWeapon(WeaponType.Bow);
                qTime = qCooldownTime;
            }
        }
    }

    

    // Silah deðiþtirme iþlemi
    void SetActiveWeapon(WeaponType weaponType)
    {
        currentWeapon = weaponType;
        normalPlayer.SetActive(weaponType == WeaponType.None);
        swordPlayer.SetActive(weaponType == WeaponType.Sword);
        spearPlayer.SetActive(weaponType == WeaponType.Spear);
        bowPlayer.SetActive(weaponType == WeaponType.Bow);

        // Eðer None durumundan baþka bir silaha geçiliyorsa, hareketi devam ettir
        if (weaponType != WeaponType.None)
        {
            ResumeMovement();
        }

        // Silah geçiþi yapýlýrken, ok ve mýzrak sayýsýný kontrol et
        if (weaponType == WeaponType.Spear && GameManager.instance.spearCount <= 0)
        {
            currentWeapon = WeaponType.Sword;  // Tekrar SetActiveWeapon çaðýrmadan direkt deðiþtir
        }
        else if (weaponType == WeaponType.Bow && GameManager.instance.arrowCount <= 0)
        {
            currentWeapon = WeaponType.None;
        }
    }

    // Silah saldýrýsý
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
    }

    // Varsayýlan silah ayarlarý (Kýlýç)
    public void DefaultWeapon()
    {
        SetActiveWeapon(WeaponType.Sword);
    }


    // Oyuncunun yatay hareketini yönetir
    void Move()
    {
        if (!isDashing) // Dash sýrasýnda hareket etme
        {
            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

            // Animator’a hýz bilgisini göndererek animasyonlarý tetikle
            float speedValue = Mathf.Abs(rb.linearVelocity.x);

            if (normalPlayer.activeSelf)
                normalAnim.SetFloat("speed", speedValue);
            else if (swordPlayer.activeSelf)
                swordAnim.SetFloat("speed", speedValue);
            else if (spearPlayer.activeSelf)
                spearAnim.SetFloat("speed", speedValue);
            else if (bowPlayer.activeSelf)
                bowAnim.SetFloat("speed", speedValue);
        }
    }
    public void MoveLeft() { moveInput = -1f; }
    public void MoveRight() { moveInput = 1f; }
    public void StopMoving() { moveInput = 0f; }
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
    public void SetJumpButtonState(bool state)
    {
        // Zýplama sýnýrýna ulaþýldýðýnda ve yere düþülene kadar buton devre dýþý býrakýlacak
        if (jumpCount >= maxJumps-1)
        {
            // Yere deðene kadar buton týklanamaz, buton devre dýþý býrakýlýr
            jumpButtonPressed = false;
        }
        else
        {
            // Zýplama sýnýrýna ulaþýlmadýysa, buton aktif
            jumpButtonPressed = state;
        }
    }

    public void Jump()
    {
        if (jumpButtonPressed && jumpCount < maxJumps) // Zýplama sayýsý maxJumps'dan küçükse zýpla
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Önceki düþüþ hýzýný sýfýrla

            float jumpPower = (jumpCount == 0) ? firstJumpForce : doubleJumpForce; // Ýlk zýplama ve double jump kuvveti
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse); // Zýplama kuvveti uygula
            AudioManager.instance.PlayAudio(0); // Zýplama sesi
            jumpCount++; // Zýplama sayýsýný artýr
            jumpButtonPressed = false; // Butonu devre dýþý býrak
        }

        // Daha gerçekçi bir zýplama eðrisi için ekstra fiziksel kuvvet uygula
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !jumpButtonPressed)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // Yere deðdiðinde zýplama sayýsýný sýfýrla ve butonu tekrar aktif et
        if (isGrounded)
        {
            jumpCount = 0; // Zýplama sayýsýný sýfýrla
            jumpButtonPressed = false; // Butonu devre dýþý býrak
            jumpButton.interactable = true; // Zýplama butonunu tekrar aktif et
        }

        // Animator’a zemin durumu ve zýplama kuvveti bilgisini gönder
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


    // Dash (Atýlma) fonksiyonu
    public void SetDashButtonState(bool state)
    {
        // Butona týklanýnca çaðrýlacak, cooldown kontrolü yapýlýyor
        if (Time.time >= lastDashTime + dashCooldown)
        {
            dashButtonPressed = state;
        }
        
    }

    public void DashButtonPressed()
    {
        if (dashButtonPressed && !isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            DashCheck();
            lastDashTime = Time.time; // Son dash zamaný güncellenir
            dashButtonPressed = false; // Butonu devre dýþý býrak
        }
    }
    void Dash()
    {
        if (dashButtonPressed && !isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            DashCheck();
            lastDashTime = Time.time; // Son dash zamaný güncellenir
            dashButtonPressed = false; // Butonu devre dýþý býrak
        }

        if (isDashing)
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                isDashing = false;
                weaponSwitchTime = weaponSwitchCooldown;
            }
        }
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
    // Karakter geri itilme durumuna geçtiðinde çalýþýr
    public void BackLeash()
    {
        backLeashCounter = backLeashTime; // Geri itilme zamanýný baþlat
        if (normalPlayer.activeSelf)
        {
            normalSpriteRenderer.color = new Color(normalSpriteRenderer.color.r, normalSpriteRenderer.color.g, normalSpriteRenderer.color.b, 0.5f); // Karakteri yarý saydam yap
        }
        else if (swordPlayer.activeSelf)
        {
            swordSprite.color = new Color(swordSprite.color.r, swordSprite.color.g, swordSprite.color.b, 0.5f); // Karakteri yarý saydam yap
        }
        else if (spearPlayer.activeSelf)
        {
            spearSprite.color = new Color(spearSprite.color.r, spearSprite.color.g, spearSprite.color.b, 0.5f); // Karakteri yarý saydam yap
        }
        else if (bowPlayer.activeSelf)
        {
            bowSprite.color = new Color(bowSprite.color.r, bowSprite.color.g, bowSprite.color.b, 0.5f); // Karakteri yarý saydam yap
        }
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Karakterin hýzýný sýfýrla

        // Eðer karakter merdivendeyse düþmesini saðla
        if (isClimbing)
        {
            isClimbing = false;
            rb.gravityScale = 3; // Yerçekimini hemen devreye sok
            if (normalPlayer.activeSelf)
            {
                normalAnim.SetBool("isClimbing", false);
            }
            else if (swordPlayer.activeSelf)
            {
                swordAnim.SetBool("isClimbing", false);
            }
            else if (spearPlayer.activeSelf)
            {
                spearAnim.SetBool("isClimbing", false);
            }
            else if (bowPlayer.activeSelf)
            {
                bowAnim.SetBool("isClimbing", false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = true;

            // Merdivenin sýnýrlarýný alýyoruz
            ladderMinX = collision.bounds.min.x; // Merdivenin sol sýnýrý
            ladderMaxX = collision.bounds.max.x; // Merdivenin sað sýnýrý
        }
        if (collision.CompareTag("BlackSmith")) { isNearBlackSmith = true; }
        if (collision.CompareTag("Tent")) { isNearTent = true; }
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder")) isOnLadder = false;
        if (collision.CompareTag("BlackSmith")) { isNearBlackSmith = false; }
        if (collision.CompareTag("Tent")) { isNearTent = false; }
    }

    public void SetClimbButtonState(bool state)
    {
        // Butona basýldýðýnda climb iþlemine baþla
        if (state && isOnLadder && qTime <= 0)
        {
            isClimbing = true;
        }
        else if (!state) // Buton býrakýldýðýnda climb iþlemini durdur
        {
            isClimbing = false;
            qTime = qCooldownTime; // Q tuþu için bekleme süresi
        }
    }

    // Dikey hareket için
    public void MoveUp() { verticalInput = 1f; }  // Yukarý hareket
    public void MoveDown() { verticalInput = -1f; }  // Aþaðý hareket
    public void StopClimbing() { verticalInput = 0f; }  // Dikey hareketi durdur

    void Climb()
    {
        
        if (normalPlayer.activeSelf)
        {
            if ((isOnLadder && qTime <= 0)) 
            {
                if (isClimbing)
                {
                    // Yatay hareketi sýnýrlamak için moveInput kullanýyoruz
                    float horizontalInput = moveInput; // Yatay input
                    float newX = Mathf.Clamp(rb.position.x + horizontalInput * climbspeed * Time.deltaTime, ladderMinX, ladderMaxX);
                    rb.position = new Vector2(newX, rb.position.y);  // Yatay hareketi sýnýrlýyoruz, dikey hareketi ayný býrakýyoruz

                    rb.linearVelocity = new Vector2(0, verticalInput * climbspeed); // Yalnýzca dikey hareketi uygula
                    rb.gravityScale = 0; // Yerçekimi devre dýþý býrakýlýyor
                    normalAnim.SetBool("isClimbing", true);
                    normalAnim.SetFloat("climbSpeed", Mathf.Abs(rb.linearVelocity.y));
                }
                else
                {
                    normalAnim.SetBool("isClimbing", false);
                    rb.gravityScale = 3; // Yerçekimini devreye sok
                }
            }
            else
            {
                // Merdiven dýþýnda veya týrmanmýyorsa
                rb.gravityScale = 3; // Yerçekimi devreye girer
                normalAnim.SetBool("isClimbing", false);
            }
        }
        else if (swordPlayer.activeSelf)
        {
            if (isOnLadder && qTime <= 0)
            {
                if (isClimbing)
                {
                    // Yatay hareketi sýnýrlamak için moveInput kullanýyoruz
                    float horizontalInput = moveInput; // Yatay input
                    float newX = Mathf.Clamp(rb.position.x + horizontalInput * climbspeed * Time.deltaTime, ladderMinX, ladderMaxX);
                    rb.position = new Vector2(newX, rb.position.y);  // Yatay hareketi sýnýrlýyoruz, dikey hareketi ayný býrakýyoruz

                    rb.linearVelocity = new Vector2(0, verticalInput * climbspeed); // Yalnýzca dikey hareketi uygula
                    rb.gravityScale = 0; // Yerçekimi devre dýþý býrakýlýyor
                    swordAnim.SetBool("isClimbing", true);
                    swordAnim.SetFloat("climbSpeed", Mathf.Abs(rb.linearVelocity.y));
                }
                else
                {
                    swordAnim.SetBool("isClimbing", false);
                    rb.gravityScale = 3; // Yerçekimini devreye sok
                }
            }
            else
            {
                // Merdiven dýþýnda veya týrmanmýyorsa
                rb.gravityScale = 3; // Yerçekimi devreye girer
                swordAnim.SetBool("isClimbing", false);
            }
        }
        else if (spearPlayer.activeSelf)
        {
            if (isOnLadder && qTime <= 0)
            {
                if (isClimbing)
                {
                    // Yatay hareketi sýnýrlamak için moveInput kullanýyoruz
                    float horizontalInput = moveInput; // Yatay input
                    float newX = Mathf.Clamp(rb.position.x + horizontalInput * climbspeed * Time.deltaTime, ladderMinX, ladderMaxX);
                    rb.position = new Vector2(newX, rb.position.y);  // Yatay hareketi sýnýrlýyoruz, dikey hareketi ayný býrakýyoruz

                    rb.linearVelocity = new Vector2(0, verticalInput * climbspeed); // Yalnýzca dikey hareketi uygula
                    rb.gravityScale = 0; // Yerçekimi devre dýþý býrakýlýyor
                    spearAnim.SetBool("isClimbing", true);
                    spearAnim.SetFloat("climbSpeed", Mathf.Abs(rb.linearVelocity.y));
                }
                else
                {
                    spearAnim.SetBool("isClimbing", false);
                    rb.gravityScale = 3; // Yerçekimini devreye sok
                }
            }
            else
            {
                // Merdiven dýþýnda veya týrmanmýyorsa
                rb.gravityScale = 3; // Yerçekimi devreye girer
                spearAnim.SetBool("isClimbing", false);
            }
        }
        else if (bowPlayer.activeSelf)
        {
            if (isOnLadder && qTime <= 0)
            {
                if (isClimbing)
                {
                    // Yatay hareketi sýnýrlamak için moveInput kullanýyoruz
                    float horizontalInput = moveInput; // Yatay input
                    float newX = Mathf.Clamp(rb.position.x + horizontalInput * climbspeed * Time.deltaTime, ladderMinX, ladderMaxX);
                    rb.position = new Vector2(newX, rb.position.y);  // Yatay hareketi sýnýrlýyoruz, dikey hareketi ayný býrakýyoruz

                    rb.linearVelocity = new Vector2(0, verticalInput * climbspeed); // Yalnýzca dikey hareketi uygula
                    rb.gravityScale = 0; // Yerçekimi devre dýþý býrakýlýyor
                    bowAnim.SetBool("isClimbing", true);
                    bowAnim.SetFloat("climbSpeed", Mathf.Abs(rb.linearVelocity.y));
                }
                else
                {
                    bowAnim.SetBool("isClimbing", false);
                    rb.gravityScale = 3; // Yerçekimini devreye sok
                }
            }
            else
            {
                // Merdiven dýþýnda veya týrmanmýyorsa
                rb.gravityScale = 3; // Yerçekimi devreye girer
                bowAnim.SetBool("isClimbing", false);
            }
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

        // Ölüm efektini 1 saniye sonra oluþtur
        StartCoroutine(CreateDeathEffect());
    }

    IEnumerator CreateDeathEffect()
    {
        yield return new WaitForSeconds(0.5f);
        Instantiate(deathEffect, transform.position, Quaternion.identity);
    }

    public void StopPlayer()
    {
        Debug.Log("StopPlayer called");
        rb.linearVelocity = Vector2.zero; // Karakterin hýzýný sýfýrla
        rb.gravityScale = 0; // Yerçekimini devre dýþý býrak
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; // Karakterin pozisyonunu ve rotasyonunu dondur
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

    public void ResumeMovement()
    {
        Debug.Log("ResumeMovement called");
        rb.gravityScale = 3; // Yerçekimini yeniden etkinleþtir
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Sadece rotasyonu dondur
        if (normalPlayer.activeSelf)
        {
            normalAnim.SetFloat("speed", rb.linearVelocity.x);
        }
        else if (swordPlayer.activeSelf)
        {
            swordAnim.SetFloat("speed", rb.linearVelocity.x);
        }
        else if (spearPlayer.activeSelf)
        {
            spearAnim.SetFloat("speed", rb.linearVelocity.x);
        }
        else if (bowPlayer.activeSelf)
        {
            bowAnim.SetFloat("speed", rb.linearVelocity.x);
        }
    }
    void OpenBlacksmithPanel()
    {
        blacksmithPanel.SetActive(true);
        StopPlayer();
    }
    void OpenTentPanel()
    {
        Tent.SetActive(true);
        StopPlayer();
    }
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
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

