using System.Collections;
using UnityEngine;

public class BowController : MonoBehaviour
{
    public static BowController instance;
    public GameObject arrowPrefab; // Ok prefab�
    public Transform shootPoint; // Okun f�rlat�laca�� nokta
    public float shootForce = 10f; // F�rlatma kuvveti
    public Animator bowAnim;
    public GameObject bowPlayer;
    private bool canShoot = true; // Ok f�rlat�labilir mi?
    public bool isShooting = false; // Sald�r� durumu

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
            bowAnim.SetInteger("arrowCount", GameManager.instance.arrowCount); // Ok say�s�n� animat�re g�nder
            if (Input.GetMouseButtonDown(0) && canShoot && PlayerMovementController.instance.bowPlayer.activeSelf) // Sol t�k ile oku f�rlatma
            {
                canShoot = false; // Ok f�rlat�ld�ktan sonra tekrar f�rlatmay� engelle
                isShooting = true;
                PlayerMovementController.instance.StopPlayer(); // Karakteri durdur
                bowAnim.SetTrigger("attack"); // Attack animasyonunu tetikle
                StartCoroutine(ShootArrowWithDelay(0.7f)); // 0.7 saniye sonra ok f�rlat animasyon uyu�mas� i�in
            }
        }
    }

    private IEnumerator ShootArrowWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Belirtilen s�re bekle
        ShootArrow();
        UIController.instance.DecreaseArrowCount();
        StartCoroutine(ArrowShootCooldown());
        isShooting = false;
        PlayerMovementController.instance.ResumeMovement(); // Karakterin hareketini yeniden ba�lat
    }

    public void ShootArrow()
    {
        if (!bowPlayer.activeSelf) return; // E�er bowPlayer aktif de�ilse ��k

        // Oku olu�tur
        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

        // Mouse'un d�nya konumunu al
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Z eksenini s�f�rla

        // F�rlatma y�n�n� hesapla (shootPoint ile mouse aras�ndaki fark)
        Vector2 shootDirection = (mousePosition - shootPoint.position).normalized;

        // Okun y�n�n� belirle (oku hedefe do�ru d�nd�r)
        arrow.transform.right = shootDirection;

        // F�rlatma kuvvetini uygula
        rb.AddForce(shootDirection * shootForce, ForceMode2D.Impulse); // Bu f�rlatma ayarlar� daha iyi

        // E�er karakterin y�n�ne g�re oku yerle�tirmek istersen
        Transform playerTransform = bowPlayer.transform.parent;
        float direction = (playerTransform != null) ? Mathf.Sign(playerTransform.localScale.x) : 1;
        arrow.transform.localScale = new Vector3(direction * Mathf.Sign(shootDirection.x), 1, 1);  // Y�n� karaktere ve f�rlatma y�n�ne g�re ayarla

        Rigidbody2D playerRb = bowPlayer.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            // Oku f�rlatmadan �nce karakterin hareketini ge�ici olarak durdurabiliriz
            playerRb.linearVelocity = Vector2.zero;  // H�z s�f�rlan�r, geri itme engellenir
        }

        // Oku f�rlatman�n ard�ndan geri itmeyi engellemek i�in k�sa bir s�re sonra normal haline getirebiliriz
        StartCoroutine(EnablePlayerMovementAfterDelay(playerRb, 0.1f)); // 0.1 saniye sonra karakter hareketini tekrar a�
    }

    private IEnumerator ArrowShootCooldown()
    {
        yield return new WaitForSeconds(0.5f); // 0.5 saniye bekle
        canShoot = true; // Ok f�rlat�labilir hale getir
    }

    private IEnumerator EnablePlayerMovementAfterDelay(Rigidbody2D playerRb, float delay)
    {
        // Bir s�re sonra hareketi tekrar etkinle�tir
        yield return new WaitForSeconds(delay);
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero; // H�z s�f�rlanabilir, ya da burada normal h�z geri verilerek yeniden ba�lat�labilir.
        }
    }
}
