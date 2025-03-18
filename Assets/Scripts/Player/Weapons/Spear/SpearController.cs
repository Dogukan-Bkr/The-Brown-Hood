using System.Collections;
using UnityEngine;

public class SpearController : MonoBehaviour
{
    public static SpearController instance;
    [SerializeField]
    GameObject attackPoint;
    public Vector2 boxSize = new Vector2(1f, 1f); // Kutu boyutu
    public GameObject spearPrefab; // Mýzrak prefabý
    public Transform throwPoint; // Mýzraðýn fýrlatýlacaðý nokta
    public float throwForce = 10f; // Fýrlatma kuvveti
    private bool isDashing = false; // Þu an dash yapýlýyor mu?
    private float dashTime = 0f; // Dash zamanlayýcýsý
    // Spear Attack deðiþkenleri
    private int comboCounter = 0;
    private float lastClickTime;
    private float comboDelay = 0.3f; // Maksimum 1 saniye içinde kombo devam edebilir
    public Animator spearAnim;
    public SpriteRenderer spearSprite;
    public GameObject spearPlayer;
    public int defaultDamage = 1; // Varsayýlan hasar deðeri
    private bool isAiming = false; // Niþan alma durumu

    private void Awake()
    {
        instance = this;
        // Sprite Renderer bileþeni atanmýþ mý kontrol et, eðer yoksa al
    }

    private void Update()
    {
        if (GameManager.instance.spearCount < 1)
        {
            PlayerMovementController.instance.spearPlayer.SetActive(false);
            PlayerMovementController.instance.normalPlayer.SetActive(true);
        }
        if (GameManager.instance.spearCount > 0)
        {
            if (Input.GetMouseButton(1)) // Sað týk basýlýyken niþan alma
            {
                isAiming = true;
                spearAnim.SetBool("isAiming", true); // Aiming animasyonunu baþlat
            }
            else
            {
                isAiming = false;
                spearAnim.SetBool("isAiming", false); // Aiming animasyonunu bitir
            }

            if (isAiming && Input.GetMouseButtonDown(0)) // Sað týk basýlýyken sol týkla mýzraðý fýrlatma
            {
                ThrowSpear();
                UIController.instance.DecreaseSpearCount();
            }
        }

    }

    public void SpearAttack()
    {
        if (isDashing || isAiming) return;

        if (Input.GetMouseButtonDown(0) && spearPlayer.activeSelf)
        {
            float currentTime = Time.time;
            Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.transform.position, boxSize, 0f);
            for (int i = 0; i < hitEnemies.Length; i++)
            {
                int damage = DetermineDamage(hitEnemies[i]);
                hitEnemies[i].GetComponent<SpiderController>()?.TakeDamage(damage);
                hitEnemies[i].GetComponent<BatController>()?.TakeDamage(damage);
                hitEnemies[i].GetComponent<BoxController>()?.TakeDamage(damage);
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
                comboCounter = 0;
            }
            lastClickTime = currentTime; // Son týklama zamanýný güncelle
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
        rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse); // Bu fýrlatma ayarlarý daha iyi

        // Eðer karakterin yönüne göre mýzraðý yerleþtirmek istersen
        Transform playerTransform = spearPlayer.transform.parent;
        float direction = (playerTransform != null) ? Mathf.Sign(playerTransform.localScale.x) : 1;
        spear.transform.localScale = new Vector3(direction, 1, 1);  // Yönü karaktere göre ayarla
        Rigidbody2D playerRb = spearPlayer.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            // Mýzraðý fýrlatmadan önce karakterin hareketini geçici olarak durdurabiliriz
            playerRb.linearVelocity = Vector2.zero;  // Hýz sýfýrlanýr, geri itme engellenir
        }

        // Mýzraðý fýrlatmanýn ardýndan geri itmeyi engellemek için kýsa bir süre sonra normal haline getirebiliriz
        StartCoroutine(EnablePlayerMovementAfterDelay(playerRb, 0.1f)); // 0.1 saniye sonra karakter hareketini tekrar aç
    }


    public int DetermineDamage(Collider2D enemy)
    {
        if (enemy.CompareTag("EnemySpider"))
        {
            return 5;
        }
        else if (enemy.CompareTag("Skeleton"))
        {
            return 3;
        }
        else if (enemy.CompareTag("Bat"))
        {
            return 5;
        }
        else if (enemy.CompareTag("Object"))
        {
            return 5;
        }

        return defaultDamage; // Varsayýlan hasar deðeri
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
    }
}
