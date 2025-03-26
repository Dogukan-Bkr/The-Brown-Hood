using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpiderController : MonoBehaviour
{
    [SerializeField] private Transform[] positions;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private float speed;
    [SerializeField] private float waitTime;
    [SerializeField] private float attackArea;
    [SerializeField] private float attackSpeed;
    [SerializeField] private int attackDamage;
    private float waitTimeCounter;
    private int targetPosIndex;
    private bool isAttackable;
    private Animator anim;
    private Transform targetPlayer;
    private BoxCollider2D spiderCollider;

    [Header("Health System")]
    [SerializeField] private int health;
    private int currentHealth;
    private bool isDead = false;

    [Header("Effects & Loot")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoin, maxCoin;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject dieEffect;

    private void Awake()
    {
        spiderCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        foreach (Transform pos in positions)
        {
            pos.parent = null;
        }
    }

    private void Start()
    {
        currentHealth = health;
        healthSlider.maxValue = health;
        healthSlider.value = health;
        isAttackable = true;
        transform.position = positions[targetPosIndex].position;
        targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (isDead) return;

        if (!isAttackable)
        {
            anim.SetBool("isMove", false);
            return;
        }

        if (targetPlayer.position.x > positions[0].position.x && targetPlayer.position.x < positions[1].position.x)
        {
            waitTimeCounter = 0;
        }

        if (waitTimeCounter > 0)
        {
            waitTimeCounter -= Time.deltaTime;
            anim.SetBool("isMove", false);
            return;
        }

        MoveToNextPosition();
    }

    private void MoveToNextPosition()
    {
        if (isDead) return; // Ölü ise hareket etme

        if (targetPlayer.position.x > positions[0].position.x && targetPlayer.position.x < positions[1].position.x)
        {
            Vector3 newPosition = new Vector3(targetPlayer.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);

            FollowDirection(targetPlayer);
            anim.SetBool("isMove", true);
        }
        else
        {
            Transform targetPosition = positions[targetPosIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, speed * Time.deltaTime);

            FollowDirection(targetPosition);
            anim.SetBool("isMove", true);

            if (Vector3.Distance(transform.position, targetPosition.position) < 0.1f)
            {
                waitTimeCounter = waitTime;
                targetPosIndex = (targetPosIndex + 1) % positions.Length;
            }
        }
    }

    private void FollowDirection(Transform target)
    {
        if (target.position.x > transform.position.x)
            transform.localScale = Vector3.one; // Saða bak
        else
            transform.localScale = new Vector3(-1, 1, 1); // Sola bak
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < positions.Length; i++)
        {
            Gizmos.DrawWireSphere(positions[i].position, attackArea);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return; // Ölü ise vurulamaz

        if (collision.gameObject.CompareTag("Player") && isAttackable)
        {
            isAttackable = false;
            anim.SetTrigger("attack");
            Debug.Log("Player hit by spider");
            PlayerHealthController.instance.TakeDamage(attackDamage);
            StartCoroutine(AttackCooldown());
        }

        // SwordDamageArea tag'ini kaldýrdýk, hasar kontrolünü Layer ve Tag kombinasyonlarýna göre yapýyoruz
        if (collision.gameObject.CompareTag("Object"))
        {
            int damage = SwordController.instance.DetermineDamage(collision); // SwordController'dan hasar deðerini al
            TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        healthSlider.value = health;
        Debug.Log("Spider health: " + health);
        Instantiate(hitEffect, transform.position, Quaternion.identity);

        if (health <= 0)
        {
            healthSlider.gameObject.SetActive(false);
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        anim.SetTrigger("die");
        Instantiate(dieEffect, transform.position, Quaternion.identity);
        if (spiderCollider != null)
        {
            spiderCollider.enabled = false;
        }
        Destroy(gameObject, 0.5f);

        // Coin düþürme
        int randomCountCoin = Random.Range(minCoin, maxCoin);
        Vector2 coinSpawnPos = transform.position;
        Debug.Log("Coin count in Spider: " + randomCountCoin);
        for (int i = 0; i < randomCountCoin; i++)
        {
            GameObject coin = Instantiate(coinPrefab, coinSpawnPos, Quaternion.identity);
            if ((i + 1) % 5 == 0)
            {
                // Her 5 sýrada bir yukarý dikey olarak sýçrat
                coin.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, Random.Range(300, 500)));
                // coinSpawnPos.x'i sýfýrla ve y'yi artýr
                coinSpawnPos.x = transform.position.x - 0.5f; // Sola çek
                coinSpawnPos.y += 0.5f; // Mesafeyi artýr
            }
            else
            {
                // Diðerleri yana doðru gitsin
                coin.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100, 100), Random.Range(300, 500)));
                coinSpawnPos.x += 0.5f; // Mesafeyi artýr
            }
        }
    }


    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackSpeed);
        isAttackable = true;
    }
}
