using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BeeController : MonoBehaviour
{
    public float speed;
    public float attackRadius = 8f;
    private bool isAttackable;
    private bool isDead = false;
    private Vector3 initialPosition;
    private Transform targetPlayer;
    private Animator anim;
    private CircleCollider2D beeCollider;
    public Slider healthSlider;
    [Header("Health System")]
    public int health = 10;  // Arý caný
    private int currentHealth;
    [Header("Effects & Loot")]
    public GameObject healthPotionPrefab;
    public GameObject coinPrefab;
    public GameObject hitEffect;
    public GameObject dieEffect;

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

    void MoveTowardsPlayer()
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

    void StartAttack()
    {
        if (!isAttackable) return;

        isAttackable = false;
        anim.SetBool("isFly", false);
        anim.SetTrigger("isAttack");
        Debug.Log("Player hit by bat");
        PlayerMovementController.instance.BackLeash();
        PlayerHealthController.instance.TakeDamage(2);
        StartCoroutine(AttackCooldown());
    }

    void ReturnToInitialPosition()
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
            int damage = SwordController.instance.defaultDamage; // SwordController'dan hasar deðerini al
            TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        healthSlider.value = health;
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        Debug.Log("Bat health: " + health);

        if (health <= 0)
        {
            healthSlider.gameObject.SetActive(false);
            Die();
        }
    }

    void Die()
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
        int randomCountHP = Random.Range(0, 1);
        int randomCountCoin = Random.Range(0, 4);
        Vector2 healthPointSpawnPos = transform.position;
        Debug.Log("Health potion count in Bee: " + randomCountHP);
        Debug.Log("Coin count in Bee: " + randomCountCoin);
        for (int i = 0; i < randomCountHP; i++)
        {
            GameObject healthPotion = Instantiate(healthPotionPrefab, healthPointSpawnPos, Quaternion.identity);
            healthPointSpawnPos.x += 0.5f;
            healthPotion.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100, 100), Random.Range(300, 500)));
        }
        for (int i = 0; i < randomCountCoin; i++)
        {
            GameObject coin = Instantiate(coinPrefab, healthPointSpawnPos, Quaternion.identity);
            healthPointSpawnPos.x += 0.5f;
            coin.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100, 100), Random.Range(300, 500)));
        }
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(2f);
        isAttackable = true;
    }

    private IEnumerator ChangeDirectionRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(2f);
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}

