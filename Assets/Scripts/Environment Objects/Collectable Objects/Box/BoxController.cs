using UnityEngine;

public class BoxController : MonoBehaviour
{
    [SerializeField] private GameObject boxHitEffect;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int health = 50; // Kutunun caný
    [SerializeField] private int minCoin, maxCoin;
    private bool isDestroyed = false;
    private Animator anim;
    private Vector2 coinSpawnPos = new Vector2(0, 0);

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SwordDamageBox"))
        {
            int damage = SwordController.instance.damage; // SwordController'dan hasar deðerini al
            TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDestroyed) return;

        health -= damage;
        Debug.Log("Box health: " + health);
        anim.SetTrigger("hit");
        AudioManager.instance?.PlayAudio(1);
        // Rastgele bir pozisyon belirle
        Vector2 randomPosition = new Vector2(
            transform.position.x + Random.Range(-0.5f, 0.5f),
            transform.position.y + Random.Range(-0.5f, 0.5f)
        );

        Instantiate(boxHitEffect, randomPosition, Quaternion.identity);

        if (health <= 0)
        {
            BreakBox();
        }
    }

    private void BreakBox()
    {
        isDestroyed = true;
        anim.SetTrigger("break");
        AudioManager.instance?.PlayAudio(5);
        GetComponent<BoxCollider2D>().enabled = false;
        Destroy(gameObject, 0.5f);
        int randomCount = Random.Range(minCoin, maxCoin);
        Debug.Log("Coin count in Box: " + randomCount);
        // Coin düþürme
        for (int i = 0; i < randomCount; i++)
        {
            GameObject coin = Instantiate(coinPrefab, (Vector2)transform.position + coinSpawnPos, Quaternion.identity);
            if ((i + 1) % 5 == 0)
            {
                // Her 5 sýrada bir yukarý dikey olarak sýçrat
                coin.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, Random.Range(300, 500)));
                // coinSpawnPos.x'i sýfýrla ve y'yi artýr
                coinSpawnPos.x = -0.5f; // Sola çek
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


}


