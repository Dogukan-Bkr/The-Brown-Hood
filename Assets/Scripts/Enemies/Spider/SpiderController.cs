using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpiderController : MonoBehaviour
{
    public Transform[] positions;
    public Slider healthSlider;
    public float speed;
    public float waitTime;
    public float attackArea;
    private float waitTimeCounter;
    int targetPosIndex;
    bool isAttackable;
    Animator anim;
    Transform targetPlayer;
    BoxCollider2D spiderCollider;

    [Header("Health System")]
    public int health = 10;  // Örümcek caný
    public int currentHealth;
    private bool isDead = false;

    [Header("Effects & Loot")]
    // public GameObject bloodEffect;
    public GameObject coinPrefab;
    public GameObject hitEffect;
    public GameObject dieEffect;
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

    void MoveToNextPosition()
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

    void FollowDirection(Transform target)
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
            PlayerMovementController.instance.BackLeash();
            PlayerHealthController.instance.TakeDamage(2);
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
        //anim.SetTrigger("hit");  // Hasar animasyonu
        // Instantiate(bloodEffect, transform.position, Quaternion.identity);

        if (health <= 0)
        {
            healthSlider.gameObject.SetActive(false);
            Die();
        }
    }

    void Die()
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
        int randomCount = Random.Range(0, 4);
        Vector2 coinSpawnPos = transform.position;
        Debug.Log("Coin count in Spider: " + randomCount);
        for (int i = 0; i < randomCount; i++)
        {
            GameObject coin = Instantiate(coinPrefab, coinSpawnPos, Quaternion.identity);
            coinSpawnPos.x += 0.5f;
            coin.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100, 100), Random.Range(300, 500)));
        }
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(3f);
        isAttackable = true;
    }
}



