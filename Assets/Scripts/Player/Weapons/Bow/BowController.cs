using System.Collections;
using UnityEngine;

public class BowController : MonoBehaviour
{
    public static BowController instance;
    public GameObject arrowPrefab; // Ok prefabý
    public Transform shootPoint; // Okun fýrlatýlacaðý nokta
    public float shootForce = 10f; // Fýrlatma kuvveti
    public Animator bowAnim;
    public GameObject bowPlayer;
    private bool canShoot = true; // Ok fýrlatýlabilir mi?
    public bool isShooting = false; // Saldýrý durumu

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (GameManager.instance.arrowCount < 1)
        {
            if (bowPlayer.activeSelf)
            {
                PlayerMovementController.instance.bowPlayer.SetActive(false);
                PlayerMovementController.instance.normalPlayer.SetActive(true);
            }
        }
        else
        {
            bowAnim.SetInteger("arrowCount", GameManager.instance.arrowCount); // Ok sayýsýný animatöre gönder
            if (Input.GetMouseButtonDown(0) && canShoot && PlayerMovementController.instance.bowPlayer.activeSelf) // Sol týk ile oku fýrlatma
            {
                canShoot = false; // Ok fýrlatýldýktan sonra tekrar fýrlatmayý engelle
                isShooting = true;
                PlayerMovementController.instance.StopPlayer(); // Karakteri durdur
                bowAnim.SetTrigger("attack"); // Attack animasyonunu tetikle
                StartCoroutine(ShootArrowWithDelay(0.7f)); // 0.7 saniye sonra ok fýrlat animasyon uyuþmasý için
            }
        }
    }

    private IEnumerator ShootArrowWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Belirtilen süre bekle
        ShootArrow();
        UIController.instance.DecreaseArrowCount();
        StartCoroutine(ArrowShootCooldown());
        isShooting = false;
        PlayerMovementController.instance.ResumeMovement(); // Karakterin hareketini yeniden baþlat
    }

    public void ShootArrow()
    {
        if (!bowPlayer.activeSelf) return; // Eðer bowPlayer aktif deðilse çýk

        // Oku oluþtur
        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

        // Mouse'un dünya konumunu al
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Z eksenini sýfýrla

        // Fýrlatma yönünü hesapla (shootPoint ile mouse arasýndaki fark)
        Vector2 shootDirection = (mousePosition - shootPoint.position).normalized;

        // Okun yönünü belirle (oku hedefe doðru döndür)
        arrow.transform.right = shootDirection;

        // Fýrlatma kuvvetini uygula
        rb.AddForce(shootDirection * shootForce, ForceMode2D.Impulse); // Bu fýrlatma ayarlarý daha iyi

        // Eðer karakterin yönüne göre oku yerleþtirmek istersen
        Transform playerTransform = bowPlayer.transform.parent;
        float direction = (playerTransform != null) ? Mathf.Sign(playerTransform.localScale.x) : 1;
        arrow.transform.localScale = new Vector3(direction * Mathf.Sign(shootDirection.x), 1, 1);  // Yönü karaktere ve fýrlatma yönüne göre ayarla

        Rigidbody2D playerRb = bowPlayer.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            // Oku fýrlatmadan önce karakterin hareketini geçici olarak durdurabiliriz
            playerRb.linearVelocity = Vector2.zero;  // Hýz sýfýrlanýr, geri itme engellenir
        }

        // Oku fýrlatmanýn ardýndan geri itmeyi engellemek için kýsa bir süre sonra normal haline getirebiliriz
        StartCoroutine(EnablePlayerMovementAfterDelay(playerRb, 0.1f)); // 0.1 saniye sonra karakter hareketini tekrar aç
    }

    private IEnumerator ArrowShootCooldown()
    {
        yield return new WaitForSeconds(0.5f); // 0.5 saniye bekle
        canShoot = true; // Ok fýrlatýlabilir hale getir
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
