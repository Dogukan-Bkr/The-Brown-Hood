using System.Collections;
using UnityEngine;

public class SpikeTrapController : MonoBehaviour
{
    public int damage = 2; // Verilecek hasar
    public float damageInterval = 0.3f; // Hasar verme süresi

    private bool isPlayerInside = false;
    private Coroutine damageCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D tetiklendi"); // Debug log eklendi
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player ile temas"); // Debug log eklendi
            isPlayerInside = true;
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(DamagePlayer(collision.GetComponent<PlayerHealthController>()));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("OnTriggerExit2D tetiklendi"); // Debug log eklendi
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player temasý sona erdi"); // Debug log eklendi
            isPlayerInside = false;
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    IEnumerator DamagePlayer(PlayerHealthController playerHealth)
    {
        while (isPlayerInside)
        {
            if (playerHealth != null)
            {
                Debug.Log("Player hasar alýyor"); // Debug log eklendi
                playerHealth.TakeDamage(damage);
            }
            yield return new WaitForSeconds(damageInterval);
        }
        damageCoroutine = null;
    }
}
