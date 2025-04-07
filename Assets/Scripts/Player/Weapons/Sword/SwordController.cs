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
    [SerializeField] public int damage = 20; // Varsay�lan hasar de�eri
    private int miss = 0;
    private bool isDashing = false; // �u an dash yap�l�yor mu?
    public bool isAttacking = false; // Sald�r� durumu
    private bool isButtonPressed = false; // Butona bas�ld���n� kontrol etmek i�in
    private int comboCounter = 0;
    private float lastClickTime;
    private float comboDelay = 0.3f; // Maksimum 1 saniye i�inde kombo devam edebilir

    private void Awake()
    {
        instance = this;
    }

    public void SwordAttack()
    {
        // Sadece butona bas�lm��sa sald�r�y� tetikle
        if (isDashing || isAttacking || !isButtonPressed) return;

        isButtonPressed = false;  // Sald�r� bir kez yap�ld�ktan sonra buton tekrar kullan�labilir
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
            AudioManager.instance?.PlayAudio(6); //sald�r� sesi
        }
        else if (comboCounter == 2)
        {
            swordAnim.SetTrigger("Attack2");
            AudioManager.instance?.PlayAudio(6); 
        }
        else if (comboCounter >= 3)
        {
            comboCounter = 0; // Kombo tamamland���nda s�f�rla
        }

        lastClickTime = currentTime; // Son t�klama zaman�n� g�ncelle
        StartCoroutine(ResetAttackState());
    }

    public void OnAttackButtonPressed()
    {
        // Butona bas�ld���nda sald�r� tetiklensin
        isButtonPressed = true;
    }

    private IEnumerator ResetAttackState()
    {
        float cooldownTime = 0.15f; // Varsay�lan sald�r� bekleme s�resi

        if (comboCounter >= 2)
        {
            cooldownTime = 0.25f; // 2 sald�r� sonras� bekleme s�resi
        }

        yield return new WaitForSeconds(cooldownTime); // Bekleme s�resi
        isAttacking = false;
        PlayerMovementController.instance.ResumeMovement(); // Karakterin hareketini yeniden ba�lat
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
