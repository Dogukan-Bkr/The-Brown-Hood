using UnityEngine;

public class SwordController : MonoBehaviour
{
    public static SwordController instance;
    [SerializeField]
    GameObject attackPoint;
    public float radius = 0.5f;
    private bool isDashing = false; // �u an dash yap�l�yor mu?
    //private float dashTime = 0f; // Dash zamanlay�c�s�
    // Sword Attack de�i�kenleri
    private int comboCounter = 0;
    private float lastClickTime;
    private float comboDelay = 0.3f; // Maksimum 1 saniye i�inde kombo devam edebilir
    public Animator swordAnim;
    public SpriteRenderer swordSpirte;
    public GameObject swordPlayer;
    public int defaultDamage = 1; // Varsay�lan hasar de�eri

    private void Awake()
    {
        instance = this;
        // Sprite Renderer bile�eni atanm�� m� kontrol et, e�er yoksa al
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
                swordAnim.SetTrigger("Attack1");
            }
            else if (comboCounter == 2)
            {
                swordAnim.SetTrigger("Attack2");
            }
            else if (comboCounter >= 3)
            {
                swordAnim.SetTrigger("Attack3");
                comboCounter = 0; // Kombo tamamland���nda s�f�rla
            }

            lastClickTime = currentTime; // Son t�klama zaman�n� g�ncelle
        }
    }

    public int DetermineDamage(Collider2D enemy)
    {
        if (enemy.CompareTag("EnemySpider")) return 5;
        if (enemy.CompareTag("Skeleton")) return 3;
        if (enemy.CompareTag("Bat")) return 5;
        if (enemy.CompareTag("Bee")) return 5;
        if (enemy.CompareTag("Boar")) return 5;
        if (enemy.CompareTag("Object")) return 5;

        return defaultDamage;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.transform.position, new Vector3(radius * 2, radius * 2, 0));
    }
}
