using UnityEngine;

public class BoxController : MonoBehaviour
{
    public GameObject boxHitEffect;
    public GameObject coinPrefab;
    public int health = 15; // Kutunun caný
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
            int damage = SwordController.instance.defaultDamage; // SwordController'dan hasar deðerini al
            TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDestroyed) return;

        health -= damage;
        Debug.Log("Box health: " + health);
        anim.SetTrigger("hit");

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

    void BreakBox()
    {
        isDestroyed = true;
        anim.SetTrigger("break");
        GetComponent<BoxCollider2D>().enabled = false;
        Destroy(gameObject, 0.5f);
        int randomCount = Random.Range(0, 4);
        Debug.Log("Coin count in Box: " + randomCount);
        // Coin düþürme
        for (int i = 0; i < randomCount; i++)
        {
            GameObject coin = Instantiate(coinPrefab, (Vector2)transform.position + coinSpawnPos, Quaternion.identity);
            coinSpawnPos.x += 1;
            coin.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-100, 100), Random.Range(300, 500)));
        }
    }
}


