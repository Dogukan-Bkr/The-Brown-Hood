using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoarController : MonoBehaviour
{
    [SerializeField] private Transform[] positions;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private float walkSpeed; // Normal y�r�me h�z�
    [SerializeField] private float runSpeed; // Ko�ma h�z�
    [SerializeField] private float waitTime;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackArea;
    [SerializeField] private int attackDamage;
    [SerializeField] private Vector2 chaseBoxSize; // Kovalama alan� boyutu
    [Header("Health System")]
    [SerializeField] private int health;
    [SerializeField] private float regenDelay = 10f; // Hasar ald�ktan sonra iyile�meye ba�lama s�resi
    [SerializeField] private int regenAmount = 5; // �yile�en miktar
    private bool isHealing = false; // �yile�me durumu kontrol�
    [Header("Effects & Loot")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoin, maxCoin;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject dieEffect;
    [Header("Damage Display")]
    [SerializeField] private GameObject damageTextPrefab; // Hasar metni prefab�
    [SerializeField] private Transform damageTextPosition; // Hasar metninin ��kaca�� nokta

    private float waitTimeCounter;
    private int targetPosIndex;
    private bool isAttackable;
    private bool isDead = false;
    private bool isChasingPlayer = false; // Oyuncuyu takip edip etmedi�ini kontrol eder
    private Animator anim;
    private Transform targetPlayer;
    private BoxCollider2D boarCollider;
    private int currentHealth;

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

        // Oyuncuyu alg�la
        isChasingPlayer = IsPlayerInChaseBox();

        if (isChasingPlayer)
        {
            waitTimeCounter = 0; // Oyuncu g�rd���nde bekleme s�resi s�f�rlan�r
        }

        if (waitTimeCounter > 0)
        {
            waitTimeCounter -= Time.deltaTime;
            anim.SetBool("isMove", false);
            return;
        }

        MoveToNextPosition();
    }

    private bool IsPlayerInChaseBox()
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

    private void MoveToNextPosition()
    {
        if (isDead) return;

        float currentSpeed = walkSpeed; // Ba�lang��ta normal y�r�y�� h�z�

        if (isChasingPlayer)
        {
            Vector3 newPosition = new Vector3(targetPlayer.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, newPosition, runSpeed * Time.deltaTime);
            FollowDirection(targetPlayer);
            anim.SetBool("isMove", true); // Ko�ma animasyonu
        }
        else
        {
            Transform targetPosition = positions[targetPosIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, currentSpeed * Time.deltaTime);

            FollowDirection(targetPosition);
            anim.SetBool("isMove", true); // Normal y�r�me animasyonu

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
            transform.localScale = Vector3.one; // Sa�a bak
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
            PlayerHealthController.instance.TakeDamage(attackDamage);
            StartCoroutine(AttackCooldown());
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        healthSlider.value = health;
        Debug.Log("Boar health: " + health);
        Instantiate(hitEffect, transform.position, Quaternion.identity);

        // Hasar metnini g�ster
        ShowDamageText(damage);
        AudioManager.instance?.PlayAudio(1);
        // E�er domuz �lmediyse ve iyile�miyorsa iyile�me ba�lat
        if (health <= 0)
        {
            healthSlider.gameObject.SetActive(false);
            Die();
        }
        else if (!isHealing) // Hasar ald� ve iyile�me ba�lamam��sa
        {
            StartCoroutine(AutoHeal());
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
        anim.SetTrigger("isDead");
        Instantiate(dieEffect, transform.position, Quaternion.identity);
        if (boarCollider != null)
        {
            boarCollider.enabled = false;
        }
        Destroy(gameObject, 0.5f);

        // Coin d���rme
        int randomCountCoin = Random.Range(minCoin, maxCoin);
        Vector2 coinSpawnPos = transform.position;
        Debug.Log("Coin count in Boar: " + randomCountCoin);
        for (int i = 0; i < randomCountCoin; i++)
        {
            GameObject coin = Instantiate(coinPrefab, coinSpawnPos, Quaternion.identity);
            if ((i + 1) % 5 == 0)
            {
                // Her 5 s�rada bir yukar� dikey olarak s��rat
                coin.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, Random.Range(300, 500)));
                // coinSpawnPos.x'i s�f�rla ve y'yi art�r
                coinSpawnPos.x = transform.position.x - 0.5f; // Sola �ek
                coinSpawnPos.y += 0.5f; // Mesafeyi art�r
            }
            else
            {
                // Di�erleri yana do�ru gitsin
                coin.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100, 100), Random.Range(300, 500)));
                coinSpawnPos.x += 0.5f; // Mesafeyi art�r
            }
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

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackSpeed); // Yar�m saniye bekle
        isAttackable = true;
    }

    // AutoHeal Coroutine�i hasar al�nd���nda ba�lar ve iyile�me tamamlan�nca durur
    private IEnumerator AutoHeal()
    {
        isHealing = true;
        yield return new WaitForSeconds(regenDelay); // 10 saniye bekle

        // Yaln�zca sa�l�k tam de�ilse iyile�tirme yap�l�r
        while (health < currentHealth)
        {
            health += regenAmount;
            if (health > currentHealth) health = currentHealth; // Sa�l�k maksimumu a�mas�n
            healthSlider.value = health; // Sa�l�k �ubu�unu g�ncelle
            yield return new WaitForSeconds(1f); // 1 saniyede bir iyile�
        }

        // �yile�me tamamland���nda coroutine�i sonland�r
        isHealing = false;
        Debug.Log("Boar fully healed.");
    }

}

