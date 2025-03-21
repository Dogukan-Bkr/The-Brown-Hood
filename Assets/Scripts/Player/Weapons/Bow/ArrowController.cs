using System.Collections;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public int defaultDamage = 5; // Okun vereceði varsayýlan hasar
    private bool hasHit = false; // Ok bir þeye çarptý mý?

    private void Start()
    {
        // Eðer hiçbir þeye çarpmazsa 2 saniye sonra kendini yok et
        StartCoroutine(AutoDestroy());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return; // Zaten çarpmýþsa tekrar çalýþtýrma
        if (collision.CompareTag("EnemySpider") || collision.CompareTag("Skeleton") || collision.CompareTag("Bat") || collision.CompareTag("Object") || collision.CompareTag("Boar") || collision.CompareTag("Bee"))
        {
            hasHit = true;

            // Hasar ver
            int damage = DetermineDamage(collision);
            collision.GetComponent<SpiderController>()?.TakeDamage(damage);
            collision.GetComponent<BatController>()?.TakeDamage(damage);
            collision.GetComponent<BeeController>()?.TakeDamage(damage);
            collision.GetComponent<BoarController>()?.TakeDamage(damage);
            collision.GetComponent<BoxController>()?.TakeDamage(damage);
            collision.GetComponent<DummyController>()?.TakeDamage(damage);

            // Oku sapla ve yok et
            StartCoroutine(DestroyArrow());
        }
    }

    private int DetermineDamage(Collider2D enemy)
    {
        if (enemy.CompareTag("EnemySpider"))
        {
            return 5;
        }
        else if (enemy.CompareTag("Skeleton"))
        {
            return 3;
        }
        else if (enemy.CompareTag("Bat"))
        {
            return 5;
        }
        else if (enemy.CompareTag("Bee"))
        {
            return 3;
        }
        else if (enemy.CompareTag("Boar"))
        {
            return 3;
        }
        else if (enemy.CompareTag("Object"))
        {
            return 5;
        }

        return defaultDamage; // Varsayýlan hasar deðeri
    }

    private IEnumerator DestroyArrow()
    {
        // Oku sapla
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;

        // Yarým saniye bekle
        yield return new WaitForSeconds(0.5f);

        // Oku yok et
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
