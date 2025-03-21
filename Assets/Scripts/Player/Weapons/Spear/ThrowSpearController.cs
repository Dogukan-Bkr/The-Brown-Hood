using System.Collections;
using UnityEngine;

public class SpearProjectile : MonoBehaviour
{
    public int damage = 5; // M�zra��n verece�i hasar
    private bool hasHit = false; // M�zrak bir �eye �arpt� m�?

    private void Start()
    {
        // E�er hi�bir �eye �arpmazsa 2 saniye sonra kendini yok et
        StartCoroutine(AutoDestroy());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return; // Zaten �arpm��sa tekrar �al��t�rma
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

            // M�zra�� sapla ve yok et
            StartCoroutine(DestroySpear());
        }
    }

    private IEnumerator DestroySpear()
    {
        // M�zra�� sapla
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;

        // Yar�m saniye bekle
        yield return new WaitForSeconds(0.5f);

        // M�zra�� yok et
        Destroy(gameObject);
    }

    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(2f);
        if (!hasHit) // E�er herhangi bir nesneye �arpmad�ysa
        {
            Destroy(gameObject);
        }
    }
}
