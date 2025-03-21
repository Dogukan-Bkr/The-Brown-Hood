using System.Collections;
using UnityEngine;

public class SpearProjectile : MonoBehaviour
{
    public int damage = 5; // Mýzraðýn vereceði hasar
    private bool hasHit = false; // Mýzrak bir þeye çarptý mý?

    private void Start()
    {
        // Eðer hiçbir þeye çarpmazsa 2 saniye sonra kendini yok et
        StartCoroutine(AutoDestroy());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return; // Zaten çarpmýþsa tekrar çalýþtýrma
        if (collision.CompareTag("EnemySpider") || collision.CompareTag("Skeleton") || collision.CompareTag("Bat") || collision.CompareTag("Object") || collision.CompareTag("Boar")|| collision.CompareTag("Bee"))
        {
            hasHit = true;

            // Hasar ver
            collision.GetComponent<SpiderController>()?.TakeDamage(damage);
            collision.GetComponent<BatController>()?.TakeDamage(damage);
            collision.GetComponent<BeeController>()?.TakeDamage(damage);
            collision.GetComponent<BoarController>()?.TakeDamage(damage);
            collision.GetComponent<BoxController>()?.TakeDamage(damage);
            collision.GetComponent<DummyController>()?.TakeDamage(damage);

            // Mýzraðý sapla ve yok et
            StartCoroutine(DestroySpear());
        }
    }

    private IEnumerator DestroySpear()
    {
        // Mýzraðý sapla
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;

        // Yarým saniye bekle
        yield return new WaitForSeconds(0.5f);

        // Mýzraðý yok et
        Destroy(gameObject);
    }

    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(2f);
        if (!hasHit) // Eðer herhangi bir nesneye çarpmadýysa
        {
            Destroy(gameObject);
        }
    }
}
