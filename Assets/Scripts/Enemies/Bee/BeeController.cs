using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeeController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float waitTime;
    [SerializeField] private float attackRadius = 8f;
    [SerializeField] private int attackDamage;
    [SerializeField] private Slider healthSlider;
    [Header("Health System")]
    [SerializeField] private int health = 10;  // Arý caný
    [Header("Effects & Loot")]
    [SerializeField] private GameObject healthPotionPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoin, maxCoin;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject dieEffect;
    [Header("Health Potion Drop Chances")]
    [SerializeField] private float dropChance0 = 0.5f; // %50 þans
    [SerializeField] private float dropChance1 = 0.3f; // %30 þans
    [SerializeField] private float dropChance2 = 0.2f; // %20 þans
    [Header("Damage Display")]
    [SerializeField] private GameObject damageTextPrefab; // Hasar metni prefabý
    [SerializeField] private Transform damageTextPosition; // Hasar metninin çýkacaðý nokta

    private bool isAttackable;
    private bool isDead = false;
    private Vector3 initialPosition;
    private Transform targetPlayer;
    private Animator anim;
    private CircleCollider2D beeCollider;
    private int currentHealth;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        beeCollider = GetComponent<CircleCollider2D>();
        initialPosition = transform.position;
    }

    private void Start()
    {
        currentHealth = health;
        healthSlider.maxValue = health;
        healthSlider.value = health;
        isAttackable = true;
        targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(ChangeDirectionRoutine()); // Coroutine'i baþlat
    }

    private void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);

        if (distanceToPlayer <= attackRadius && isAttackable)
        {
            if (distanceToPlayer <= 0.5f)
            {
                StartAttack();
            }
            else
            {
                MoveTowardsPlayer();
            }
        }
        else
        {
            ReturnToInitialPosition();
        }
    }

    private void MoveTowardsPlayer()
    {
        anim.SetBool("isFly", true);

        Vector3 direction = (targetPlayer.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, speed * Time.deltaTime);

        // Yönü kontrol et
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Saða bak
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Sola bak
        }
    }

    private void StartAttack()
    {
        if (!isAttackable) return;

        isAttackable = false;
        anim.SetBool("isFly", false);
        anim.SetTrigger("isAttack");
        Debug.Log("Player hit by bee");
        PlayerMovementController.instance.BackLeash();
        PlayerHealthController.instance.TakeDamage(attackDamage);
        StartCoroutine(AttackCooldown());
    }

    private void ReturnToInitialPosition()
    {
        anim.SetBool("isFly", true);

        Vector3 direction = (initialPosition - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, initialPosition, speed * Time.deltaTime);

        // Yönü kontrol et
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Saða bak
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Sola bak
        }

        if (Vector3.Distance(transform.position, initialPosition) <= 0.1f)
        {
            anim.SetBool("isFly", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return; // Ölü ise vurulamaz

        if (collision.gameObject.CompareTag("SwordDamageBox"))
        {
            int damage = SwordController.instance.damage; // SwordController'dan hasar deðerini al
            TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        healthSlider.value = health;
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        Debug.Log("Bee health: " + health);

        // Hasar metnini göster
        ShowDamageText(damage);

        if (health <= 0)
        {
            healthSlider.gameObject.SetActive(false);
            Die();
        }
    }

    private void ShowDamageText(int damage)
    {
        if (damageTextPrefab != null && damageTextPosition != null)
        {
            GameObject damageText = Instantiate(damageTextPrefab, damageTextPosition.position, Quaternion.identity);
            damageText.transform.SetParent(null);

            TextMeshPro textMesh = damageText.GetComponent<TextMeshPro>();
            if (textMesh != null)
            {
                textMesh.text = damage.ToString();
            }

            Destroy(damageText, 1f);
        }
    }

    private void Die()
    {
        isDead = true;
        anim.SetTrigger("isDie");
        Instantiate(dieEffect, transform.position, Quaternion.identity);
        if (beeCollider != null)
        {
            beeCollider.enabled = false;
        }
        Destroy(gameObject, 0.5f);

        // Nesne düþürme
        int randomCountHP = DetermineHealthPotionDrop();
        int randomCountCoin = Random.Range(minCoin, maxCoin);
        Vector2 dropSpawnPos = transform.position;
        Debug.Log("Health potion count in Bee: " + randomCountHP);
        Debug.Log("Coin count in Bee: " + randomCountCoin);

        for (int i = 0; i < randomCountHP; i++)
        {
            GameObject healthPotion = Instantiate(healthPotionPrefab, dropSpawnPos, Quaternion.identity);
            if ((i + 1) % 5 == 0)
            {
                // Her 5 sýrada bir yukarý dikey olarak sýçrat
                healthPotion.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, Random.Range(300, 500)));
                // dropSpawnPos.x'i sýfýrla ve y'yi artýr
                dropSpawnPos.x = transform.position.x - 0.5f; // Sola çek
                dropSpawnPos.y += 0.5f; // Mesafeyi artýr
            }
            else
            {
                // Diðerleri yana doðru gitsin
                healthPotion.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100, 100), Random.Range(300, 500)));
                dropSpawnPos.x += 0.5f; // Mesafeyi artýr
            }
        }

        for (int i = 0; i < randomCountCoin; i++)
        {
            GameObject coin = Instantiate(coinPrefab, dropSpawnPos, Quaternion.identity);
            if ((i + 1) % 5 == 0)
            {
                // Her 5 sýrada bir yukarý dikey olarak sýçrat
                coin.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, Random.Range(300, 500)));
                // dropSpawnPos.x'i sýfýrla ve y'yi artýr
                dropSpawnPos.x = transform.position.x - 0.5f; // Sola çek
                dropSpawnPos.y += 0.5f; // Mesafeyi artýr
            }
            else
            {
                // Diðerleri yana doðru gitsin
                coin.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100, 100), Random.Range(300, 500)));
                dropSpawnPos.x += 0.5f; // Mesafeyi artýr
            }
        }
    }

    private int DetermineHealthPotionDrop()
    {
        float randomValue = Random.value; // 0.0 - 1.0 arasýnda rastgele bir sayý üret

        if (randomValue < dropChance2) // %20 ihtimalle 2 iksir düþer
        {
            return 2;
        }
        else if (randomValue < dropChance1 + dropChance2) // %30 ihtimalle 1 iksir düþer
        {
            return 1;
        }
        else if (randomValue < dropChance0 + dropChance1 + dropChance2) // %50 ihtimalle 0 iksir düþer
        {
            return 0;
        }

        return 0; // Ekstra güvenlik için
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackSpeed);
        isAttackable = true;
    }

    private IEnumerator ChangeDirectionRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(waitTime);
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
