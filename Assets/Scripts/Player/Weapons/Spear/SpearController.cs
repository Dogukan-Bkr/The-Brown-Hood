using System.Collections;
using UnityEngine;

public class SpearController : MonoBehaviour
{
    public static SpearController instance;
    [SerializeField] private GameObject attackPoint;
    [SerializeField] private Vector2 boxSize = new Vector2(1f, 1f); // Kutu boyutu
    [SerializeField] private GameObject spearPrefab; // Mýzrak prefabý
    [SerializeField] private Transform throwPoint; // Mýzraðýn fýrlatýlacaðý nokta
    [SerializeField] private float throwForce = 10f; // Fýrlatma kuvveti
    [SerializeField] private Animator spearAnim;
    [SerializeField] private SpriteRenderer spearSprite;
    [SerializeField] private GameObject spearPlayer;
    [SerializeField] private float throwCooldown = 2f; // Mýzrak fýrlatma bekleme süresi
    [SerializeField] public int damage = 1; // Varsayýlan hasar deðeri
    private int miss = 0;
    private bool isButtonPressed = false; // Butona basýldýðýný kontrol etmek için
    private bool isDashing = false; // Þu an dash yapýlýyor mu?
    public bool isAttacking = false; // Saldýrý durumu
    public bool isAiming = false; // Niþan alma durumu
    private bool canThrow = true; // Mýzrak fýrlatýlabilir mi?
    // Spear Attack deðiþkenleri
    private int comboCounter = 0;
    private float lastClickTime;
    private float comboDelay = 0.3f; // Maksimum 1 saniye içinde kombo devam edebilir

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (!ShotModeController.instance.isShotMode) return; // Shot Mode aktif deðilse hiçbir þey yapma

        if (GameManager.instance.spearCount < 1)
        {
            PlayerMovementController.instance.DefaultWeapon(); // Kýlýcý aktif hale getir
            PlayerMovementController.instance.ResumeMovement(); // Karakterin hareketini yeniden baþlat
        }
        else
        {
            // Týklama ile mýzrak fýrlatma iþlemini tetikle
            if (Input.GetMouseButtonDown(0)) // Sol týk ile mýzrak fýrlat
            {
                SpearShoot();
            }
        }
    }

    public void SpearShoot()
    {
        if (canThrow && spearPlayer.activeSelf && GameManager.instance.spearCount >= 2)
        {
            ThrowSpear();
            spearAnim.SetTrigger("isThrow");
            UIController.instance.DecreaseSpearCount();
            StartCoroutine(SpearThrowCooldown());
        }
    }

    public void OnAttackButtonPressed()
    {
        // Butona basýldýðýnda saldýrý tetiklensin
        isButtonPressed = true;
    }
    public void SpearAttack()
    {
        if (isDashing || isAiming || isAttacking || !isButtonPressed) return;


        isAttacking = true;
        isButtonPressed = false;
        PlayerMovementController.instance.StopPlayer(); // Karakteri durdur
        float currentTime = Time.time;
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.transform.position, boxSize, 0f);
        for (int i = 0; i < hitEnemies.Length; i++)
        {
            int damage = DetermineDamage(hitEnemies[i]);
            hitEnemies[i].GetComponent<SpiderController>()?.TakeDamage(damage);
            hitEnemies[i].GetComponent<BatController>()?.TakeDamage(damage);
            hitEnemies[i].GetComponent<BeeController>()?.TakeDamage(damage);
            hitEnemies[i].GetComponent<BoarController>()?.TakeDamage(damage);
            hitEnemies[i].GetComponent<BoxController>()?.TakeDamage(damage);
            hitEnemies[i].GetComponent<DummyController>()?.TakeDamage(damage);
        }

        // Eðer zaman farký belirlenen süreden büyükse, komboyu sýfýrla
        if (currentTime - lastClickTime > comboDelay)
        {
            comboCounter = 0;
        }

        // Kombo sayacýný arttýr
        comboCounter++;

        // Saldýrý animasyonlarýný sýrayla tetikle
        if (comboCounter == 1)
        {
            spearAnim.SetTrigger("attack1");
        }
        else if (comboCounter == 2)
        {
            spearAnim.SetTrigger("attack2");

        }
        else if (comboCounter == 3) { comboCounter = 0; }
        lastClickTime = currentTime; // Son týklama zamanýný güncelle
        StartCoroutine(ResetAttackState());

    }

    private IEnumerator ResetAttackState()
    {
        float cooldownTime = 0.15f; // Varsayýlan saldýrý bekleme süresi

        if (comboCounter >= 2)
        {
            cooldownTime = 0.5f; // 2 saldýrý sonrasý 0.75 saniye bekle
        }

        yield return new WaitForSeconds(cooldownTime); // Bekleme süresi
        isAttacking = false;
        if (!isAiming) // Eðer niþan almýyorsa hareketi yeniden baþlat
        {
            PlayerMovementController.instance.ResumeMovement();
        }
    }

    public void ThrowSpear()
    {
        // Mýzraðý oluþtur
        GameObject spear = Instantiate(spearPrefab, throwPoint.position, Quaternion.identity);
        Rigidbody2D rb = spear.GetComponent<Rigidbody2D>();

        // Mouse'un dünya konumunu al
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Z eksenini sýfýrla

        // Fýrlatma yönünü hesapla (throwPoint ile mouse arasýndaki fark)
        Vector2 throwDirection = (mousePosition - throwPoint.position).normalized;

        // Mýzraðýn yönünü belirle (mýzraðý hedefe doðru döndür)
        spear.transform.right = throwDirection;

        // Fýrlatma kuvvetini uygula
        rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);

        // Eðer karakterin yönüne göre mýzraðý yerleþtirmek istersen
        Transform playerTransform = spearPlayer.transform.parent;
        float direction = (playerTransform != null) ? Mathf.Sign(playerTransform.localScale.x) : 1;
        spear.transform.localScale = new Vector3(direction * Mathf.Sign(throwDirection.x), 1, 1);  // Yönü karaktere ve fýrlatma yönüne göre ayarla

        Rigidbody2D playerRb = spearPlayer.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            // Mýzraðý fýrlatmadan önce karakterin hareketini geçici olarak durdurabiliriz
            playerRb.linearVelocity = Vector2.zero;  // Hýz sýfýrlanýr, geri itme engellenir
        }

        // Mýzraðý fýrlatmanýn ardýndan geri itmeyi engellemek için kýsa bir süre sonra normal haline getirebiliriz
        StartCoroutine(EnablePlayerMovementAfterDelay(playerRb, 0.1f)); // 0.1 saniye sonra karakter hareketini tekrar aç
    }

    private IEnumerator SpearThrowCooldown()
    {
        canThrow = false;
        yield return new WaitForSeconds(throwCooldown); // 2 saniye bekle
        canThrow = true;
    }

    public int DetermineDamage(Collider2D enemy)
    {
        if (enemy.CompareTag("EnemySpider"))
        {
            return damage;
        }
        else if (enemy.CompareTag("Bat"))
        {
            return damage;
        }
        else if (enemy.CompareTag("Bee"))
        {
            return damage;
        }
        else if (enemy.CompareTag("Boar"))
        {
            return damage;
        }
        else if (enemy.CompareTag("Object"))
        {
            return damage;
        }

        return miss; // Varsayýlan hasar deðeri
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.transform.position, boxSize);
    }

    private IEnumerator EnablePlayerMovementAfterDelay(Rigidbody2D playerRb, float delay)
    {
        // Bir süre sonra hareketi tekrar etkinleþtir
        yield return new WaitForSeconds(delay);
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero; // Hýz sýfýrlanabilir, ya da burada normal hýz geri verilerek yeniden baþlatýlabilir.
        }
        if (!isAiming) // Eðer niþan almýyorsa hareketi yeniden baþlat
        {
            PlayerMovementController.instance.ResumeMovement();
        }
    }
}
