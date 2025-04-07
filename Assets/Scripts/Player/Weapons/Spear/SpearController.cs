using System.Collections;
using UnityEngine;

public class SpearController : MonoBehaviour
{
    public static SpearController instance;
    [SerializeField] private GameObject attackPoint;
    [SerializeField] private Vector2 boxSize = new Vector2(1f, 1f); // Kutu boyutu
    [SerializeField] private GameObject spearPrefab; // M�zrak prefab�
    [SerializeField] private Transform throwPoint; // M�zra��n f�rlat�laca�� nokta
    [SerializeField] private float throwForce = 10f; // F�rlatma kuvveti
    [SerializeField] private Animator spearAnim;
    [SerializeField] private SpriteRenderer spearSprite;
    [SerializeField] private GameObject spearPlayer;
    [SerializeField] private float throwCooldown = 2f; // M�zrak f�rlatma bekleme s�resi
    [SerializeField] public int damage = 1; // Varsay�lan hasar de�eri
    private int miss = 0;
    private bool isButtonPressed = false; // Butona bas�ld���n� kontrol etmek i�in
    private bool isDashing = false; // �u an dash yap�l�yor mu?
    public bool isAttacking = false; // Sald�r� durumu
    public bool isAiming = false; // Ni�an alma durumu
    private bool canThrow = true; // M�zrak f�rlat�labilir mi?
    // Spear Attack de�i�kenleri
    private int comboCounter = 0;
    private float lastClickTime;
    private float comboDelay = 0.3f; // Maksimum 1 saniye i�inde kombo devam edebilir

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (!ShotModeController.instance.isShotMode) return; // Shot Mode aktif de�ilse hi�bir �ey yapma

        if (GameManager.instance.spearCount < 1)
        {
            PlayerMovementController.instance.DefaultWeapon(); // K�l�c� aktif hale getir
            PlayerMovementController.instance.ResumeMovement(); // Karakterin hareketini yeniden ba�lat
        }
        else
        {
            // T�klama ile m�zrak f�rlatma i�lemini tetikle
            if (Input.GetMouseButtonDown(0)) // Sol t�k ile m�zrak f�rlat
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
        // Butona bas�ld���nda sald�r� tetiklensin
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

        // E�er zaman fark� belirlenen s�reden b�y�kse, komboyu s�f�rla
        if (currentTime - lastClickTime > comboDelay)
        {
            comboCounter = 0;
        }

        // Kombo sayac�n� artt�r
        comboCounter++;

        // Sald�r� animasyonlar�n� s�rayla tetikle
        if (comboCounter == 1)
        {
            spearAnim.SetTrigger("attack1");
        }
        else if (comboCounter == 2)
        {
            spearAnim.SetTrigger("attack2");

        }
        else if (comboCounter == 3) { comboCounter = 0; }
        lastClickTime = currentTime; // Son t�klama zaman�n� g�ncelle
        StartCoroutine(ResetAttackState());

    }

    private IEnumerator ResetAttackState()
    {
        float cooldownTime = 0.15f; // Varsay�lan sald�r� bekleme s�resi

        if (comboCounter >= 2)
        {
            cooldownTime = 0.5f; // 2 sald�r� sonras� 0.75 saniye bekle
        }

        yield return new WaitForSeconds(cooldownTime); // Bekleme s�resi
        isAttacking = false;
        if (!isAiming) // E�er ni�an alm�yorsa hareketi yeniden ba�lat
        {
            PlayerMovementController.instance.ResumeMovement();
        }
    }

    public void ThrowSpear()
    {
        // M�zra�� olu�tur
        GameObject spear = Instantiate(spearPrefab, throwPoint.position, Quaternion.identity);
        Rigidbody2D rb = spear.GetComponent<Rigidbody2D>();

        // Mouse'un d�nya konumunu al
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Z eksenini s�f�rla

        // F�rlatma y�n�n� hesapla (throwPoint ile mouse aras�ndaki fark)
        Vector2 throwDirection = (mousePosition - throwPoint.position).normalized;

        // M�zra��n y�n�n� belirle (m�zra�� hedefe do�ru d�nd�r)
        spear.transform.right = throwDirection;

        // F�rlatma kuvvetini uygula
        rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);

        // E�er karakterin y�n�ne g�re m�zra�� yerle�tirmek istersen
        Transform playerTransform = spearPlayer.transform.parent;
        float direction = (playerTransform != null) ? Mathf.Sign(playerTransform.localScale.x) : 1;
        spear.transform.localScale = new Vector3(direction * Mathf.Sign(throwDirection.x), 1, 1);  // Y�n� karaktere ve f�rlatma y�n�ne g�re ayarla

        Rigidbody2D playerRb = spearPlayer.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            // M�zra�� f�rlatmadan �nce karakterin hareketini ge�ici olarak durdurabiliriz
            playerRb.linearVelocity = Vector2.zero;  // H�z s�f�rlan�r, geri itme engellenir
        }

        // M�zra�� f�rlatman�n ard�ndan geri itmeyi engellemek i�in k�sa bir s�re sonra normal haline getirebiliriz
        StartCoroutine(EnablePlayerMovementAfterDelay(playerRb, 0.1f)); // 0.1 saniye sonra karakter hareketini tekrar a�
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

        return miss; // Varsay�lan hasar de�eri
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.transform.position, boxSize);
    }

    private IEnumerator EnablePlayerMovementAfterDelay(Rigidbody2D playerRb, float delay)
    {
        // Bir s�re sonra hareketi tekrar etkinle�tir
        yield return new WaitForSeconds(delay);
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero; // H�z s�f�rlanabilir, ya da burada normal h�z geri verilerek yeniden ba�lat�labilir.
        }
        if (!isAiming) // E�er ni�an alm�yorsa hareketi yeniden ba�lat
        {
            PlayerMovementController.instance.ResumeMovement();
        }
    }
}
