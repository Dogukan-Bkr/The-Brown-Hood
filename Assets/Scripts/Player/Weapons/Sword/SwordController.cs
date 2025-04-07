using System.Collections;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    public static SwordController instance;
    [SerializeField] private GameObject attackPoint;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private Animator swordAnim;
    [SerializeField] private SpriteRenderer swordSprite;
    [SerializeField] private GameObject swordPlayer;
    [SerializeField] public int damage = 20; // Varsayýlan hasar deðeri
    private int miss = 0;
    private bool isDashing = false; // Þu an dash yapýlýyor mu?
    public bool isAttacking = false; // Saldýrý durumu
    private bool isButtonPressed = false; // Butona basýldýðýný kontrol etmek için
    private int comboCounter = 0;
    private float lastClickTime;
    private float comboDelay = 0.3f; // Maksimum 1 saniye içinde kombo devam edebilir

    private void Awake()
    {
        instance = this;
    }

    public void SwordAttack()
    {
        // Sadece butona basýlmýþsa saldýrýyý tetikle
        if (isDashing || isAttacking || !isButtonPressed) return;

        isButtonPressed = false;  // Saldýrý bir kez yapýldýktan sonra buton tekrar kullanýlabilir
        isAttacking = true;
        PlayerMovementController.instance.StopPlayer(); // Karakteri durdur
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
            AudioManager.instance?.PlayAudio(6); //saldýrý sesi
        }
        else if (comboCounter == 2)
        {
            swordAnim.SetTrigger("Attack2");
            AudioManager.instance?.PlayAudio(6); 
        }
        else if (comboCounter >= 3)
        {
            comboCounter = 0; // Kombo tamamlandýðýnda sýfýrla
        }

        lastClickTime = currentTime; // Son týklama zamanýný güncelle
        StartCoroutine(ResetAttackState());
    }

    public void OnAttackButtonPressed()
    {
        // Butona basýldýðýnda saldýrý tetiklensin
        isButtonPressed = true;
    }

    private IEnumerator ResetAttackState()
    {
        float cooldownTime = 0.15f; // Varsayýlan saldýrý bekleme süresi

        if (comboCounter >= 2)
        {
            cooldownTime = 0.25f; // 2 saldýrý sonrasý bekleme süresi
        }

        yield return new WaitForSeconds(cooldownTime); // Bekleme süresi
        isAttacking = false;
        PlayerMovementController.instance.ResumeMovement(); // Karakterin hareketini yeniden baþlat
    }

    public int DetermineDamage(Collider2D enemy)
    {
        if (enemy.CompareTag("EnemySpider")) return damage;
        if (enemy.CompareTag("Bat")) return damage;
        if (enemy.CompareTag("Bee")) return damage;
        if (enemy.CompareTag("Boar")) return damage;
        if (enemy.CompareTag("Object")) return damage;

        return miss;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.transform.position, new Vector3(radius * 2, radius * 2, 0));
    }
}
