using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BatController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackRadius = 8f;
    [SerializeField] private int attackDamage;
    [SerializeField] private Slider healthSlider;

    [Header("Health System")]
    [SerializeField] private int maxHealth = 10; // Yarasan�n maksimum can�
    [SerializeField] private float healCooldown = 10f; // Son hasardan sonra iyile�me s�resi

    private float lastDamageTime;
    private Coroutine healCoroutine;
    private int currentHealth;

    [Header("Effects & Loot")]
    [SerializeField] private GameObject healthPotionPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoin, maxCoin;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject dieEffect;

    [Header("Health Potion Drop Chances")]
    [SerializeField] private float dropChance0 = 0.5f; 
    [SerializeField] private float dropChance1 = 0.3f; 
    [SerializeField] private float dropChance2 = 0.2f; 

    [Header("Damage Display")]
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private Transform damageTextPosition;

    private bool isAttackable;
    private bool isDead = false;
    private Vector3 initialPosition;
    private Transform targetPlayer;
    private Animator anim;
    private CircleCollider2D batCollider;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        batCollider = GetComponent<CircleCollider2D>();
        initialPosition = transform.position;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        isAttackable = true;
        targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;
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

        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void StartAttack()
    {
        if (!isAttackable) return;

        isAttackable = false;
        anim.SetBool("isFly", false);
        anim.SetTrigger("isAttack");
        Debug.Log("Player hit by bat");

        PlayerMovementController.instance.BackLeash();
        PlayerHealthController.instance.TakeDamage(attackDamage);

        StartCoroutine(AttackCooldown());
    }

    private void ReturnToInitialPosition()
    {
        anim.SetBool("isFly", true);
        Vector3 direction = (initialPosition - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, initialPosition, speed * Time.deltaTime);

        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Vector3.Distance(transform.position, initialPosition) <= 0.1f)
        {
            anim.SetBool("isFly", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("SwordDamageBox"))
        {
            int damage = SwordController.instance.damage;
            TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        healthSlider.value = currentHealth;
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        Debug.Log("Bat health: " + currentHealth);

        ShowDamageText(damage);
        AudioManager.instance?.PlayAudio(1);

        // Hasar al�nd�ktan sonra zaman kayd�n� g�ncelle
        lastDamageTime = Time.time;

        if (healCoroutine == null)
        {
            healCoroutine = StartCoroutine(AutoHeal()); // Yaln�zca bir coroutine �al��acak
        }

        if (currentHealth <= 0)
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
        if (isDead) return; // Tekrar tekrar �lmemesi i�in kontrol

        isDead = true;
        anim.SetTrigger("isDie");

        Instantiate(dieEffect, transform.position, Quaternion.identity);

        if (batCollider != null)
        {
            batCollider.enabled = false;
        }

        DropLoot(); // �l�rken loot d���r!

        Destroy(gameObject, 0.5f);
    }


    private IEnumerator AutoHeal()
    {
        // Hasar al�nd���nda ilk ba�ta iyile�me s�resini ba�lat�yoruz
        yield return new WaitForSeconds(healCooldown); // Son hasardan sonra iyile�me s�resi kadar bekle

        // Hasar s�resi bitiminde iyile�meyi ba�lat
        if (Time.time - lastDamageTime >= healCooldown)
        {
            currentHealth = maxHealth;  // Sa�l�k doluyor
            healthSlider.value = maxHealth;
            Debug.Log("Bat healed to full health!");
        }

        healCoroutine = null; // Coroutine tamamland�ktan sonra referans� temizle
    }
    private void DropLoot()
    {
        // Coin d���rme
        int coinAmount = Random.Range(minCoin, maxCoin);
        for (int i = 0; i < coinAmount; i++)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        // Sa�l�k iksiri d���rme
        float dropChance = Random.value; // 0.0 ile 1.0 aras�nda rastgele say� �retir

        if (dropChance <= dropChance2)
        {
            Instantiate(healthPotionPrefab, transform.position, Quaternion.identity);
            Debug.Log("Rare Health Potion Dropped!");
        }
        else if (dropChance <= dropChance1 + dropChance2)
        {
            Instantiate(healthPotionPrefab, transform.position, Quaternion.identity);
            Debug.Log("Common Health Potion Dropped!");
        }
        else if (dropChance <= dropChance0 + dropChance1 + dropChance2)
        {
            Instantiate(healthPotionPrefab, transform.position, Quaternion.identity);
            Debug.Log("Basic Health Potion Dropped!");
        }
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackSpeed);
        isAttackable = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
