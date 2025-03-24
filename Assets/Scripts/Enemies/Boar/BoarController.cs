using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BoarController : MonoBehaviour
{
    public Transform[] positions;
    public Slider healthSlider;
    public float walkSpeed; // Normal yürüme hýzý
    public float runSpeed; // Koþma hýzý
    public float waitTime;
    public float attackArea;
    public Vector2 chaseBoxSize; // Kovalama alaný boyutu
    private float waitTimeCounter;
    int targetPosIndex;
    bool isAttackable;
    Animator anim;
    Transform targetPlayer;
    BoxCollider2D boarCollider;
    private bool isChasingPlayer = false; // Oyuncuyu takip edip etmediðini kontrol eder

    [Header("Health System")]
    public int health = 10;
    private int currentHealth;
    private bool isDead = false;

    [Header("Effects & Loot")]
    public GameObject coinPrefab;
    public GameObject hitEffect;
    public GameObject dieEffect;

    private void Awake()
    {
        boarCollider = GetComponent<BoxCollider2D>();
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

        // Oyuncuyu algýla
        isChasingPlayer = IsPlayerInChaseBox();

        if (isChasingPlayer)
        {
            waitTimeCounter = 0; // Oyuncu gördüðünde bekleme süresi sýfýrlanýr
        }

        if (waitTimeCounter > 0)
        {
            waitTimeCounter -= Time.deltaTime;
            anim.SetBool("isMove", false);
            return;
        }

        MoveToNextPosition();
    }

    bool IsPlayerInChaseBox()
    {
        Vector2 boxCenter = (Vector2)transform.position + boarCollider.offset;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(boxCenter, chaseBoxSize, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    void MoveToNextPosition()
    {
        if (isDead) return;

        float currentSpeed = walkSpeed; // Baþlangýçta normal yürüyüþ hýzý

        if (isChasingPlayer)
        {
            Vector3 newPosition = new Vector3(targetPlayer.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, newPosition, runSpeed * Time.deltaTime);
            FollowDirection(targetPlayer);
            anim.SetBool("isMove", true); // Koþma animasyonu
        }
        else
        {
            Transform targetPosition = positions[targetPosIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, currentSpeed * Time.deltaTime);

            FollowDirection(targetPosition);
            anim.SetBool("isMove", true); // Normal yürüme animasyonu

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player") && isAttackable)
        {
            isAttackable = false;
            anim.SetTrigger("isAttack");
            Debug.Log("Player hit by boar");
            PlayerMovementController.instance.BackLeash();
            PlayerHealthController.instance.TakeDamage(2);
            StartCoroutine(AttackCooldown());
        }

        if (collision.gameObject.CompareTag("Object"))
        {
            int damage = SwordController.instance.DetermineDamage(collision);
            TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        healthSlider.value = health;
        Debug.Log("Boar health: " + health);
        Instantiate(hitEffect, transform.position, Quaternion.identity);

        if (health <= 0)
        {
            healthSlider.gameObject.SetActive(false);
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        anim.SetTrigger("isDead");
        Instantiate(dieEffect, transform.position, Quaternion.identity);
        if (boarCollider != null)
        {
            boarCollider.enabled = false;
        }
        Destroy(gameObject, 0.5f);

        // Coin düþürme
        int randomCountCoin = Random.Range(0, 4);
        Vector2 coinSpawnPos = transform.position;
        Debug.Log("Coin count in Boar: " + randomCountCoin);
        for (int i = 0; i < randomCountCoin; i++)
        {
            GameObject coin = Instantiate(coinPrefab, coinSpawnPos, Quaternion.identity);
            coinSpawnPos.x += 0.5f;
            coin.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100, 100), Random.Range(300, 500)));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < positions.Length; i++)
        {
            Gizmos.DrawWireSphere(positions[i].position, attackArea);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, chaseBoxSize);
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(0.5f); // Yarým saniye bekle
        isAttackable = true;
    }
}
