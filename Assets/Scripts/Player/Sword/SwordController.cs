using UnityEngine;

public class SwordController : MonoBehaviour
{
    public static SwordController instance;
    [SerializeField]
    GameObject attackPoint;
    public float radius = 0.5f;
    private bool isDashing = false; // Þu an dash yapýlýyor mu?
    private float dashTime = 0f; // Dash zamanlayýcýsý
    // Sword Attack deðiþkenleri
    private int comboCounter = 0;
    private float lastClickTime;
    private float comboDelay = 0.3f; // Maksimum 1 saniye içinde kombo devam edebilir
    public Animator swordAnim;
    public SpriteRenderer swordSpirte;
    public GameObject swordPlayer;
    public int defaultDamage = 1; // Varsayýlan hasar deðeri

    private void Awake()
    {
        instance = this;
        // Sprite Renderer bileþeni atanmýþ mý kontrol et, eðer yoksa al
    }

    public void SwordAttack()
    {
        if (isDashing) return;

        if (Input.GetMouseButtonDown(0) && swordPlayer.activeSelf)
        {
            float currentTime = Time.time;
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius);
            for (int i = 0; i < hitEnemies.Length; i++)
            {
                int damage = DetermineDamage(hitEnemies[i]);
                hitEnemies[i].GetComponent<SpiderController>()?.TakeDamage(damage);
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
                swordAnim.SetTrigger("Attack1");
            }
            else if (comboCounter == 2)
            {
                swordAnim.SetTrigger("Attack2");
            }
            else if (comboCounter >= 3)
            {
                swordAnim.SetTrigger("Attack3");
                comboCounter = 0; // Kombo tamamlandýðýnda sýfýrla
            }

            lastClickTime = currentTime; // Son týklama zamanýný güncelle
        }
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
        else if (enemy.CompareTag("Object"))
        {
            return 5;
        }

        return defaultDamage; // Varsayýlan hasar deðeri
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, radius);
    }
}


