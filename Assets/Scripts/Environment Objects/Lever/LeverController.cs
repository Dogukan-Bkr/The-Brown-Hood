using UnityEngine;
using DG.Tweening;

public class LeverController : MonoBehaviour
{
    [SerializeField] private GameObject Gate;
    [SerializeField] private bool isGateOpen = false;
    [SerializeField] private bool isVertical = true; // Kapý yönünü Unity üzerinden seçebilmek için
    [SerializeField] private float moveDistance = 4.59f; // Kapýnýn hareket mesafesi
    [SerializeField] private float moveDuration = 1f; // Kapýnýn hareket süresi

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator bileþeni eksik! " + gameObject.name);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Çarpýþan: " + collision.gameObject.name); // Çarpýþan objeyi yazdýr

        if (collision.CompareTag("Player")) // Player ile çarpýþýyorsa
        {
            if (!isGateOpen) // Kapý kapalýysa aç
            {
                Debug.Log("Kapý Açýlýyor!");
                isGateOpen = true;

                if (anim != null)
                {
                    anim.SetTrigger("openGate");
                }

                // Animasyon süresini bekleyerek kapýyý aç
                StartCoroutine(OpenGateAfterDelay(0.5f)); // 0.5 saniye gecikme ile aç
            }
            else // Kapý açýkken tekrar tetiklenirse kapat
            {
                Debug.Log("Kapý Kapanýyor!");
                isGateOpen = false;
                if (anim != null)
                {
                    anim.SetTrigger("closeGate");
                }

                // Kapýyý geri kapat
                MoveGate(-moveDistance);
            }
        }
    }

    private System.Collections.IEnumerator OpenGateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        MoveGate(moveDistance);
    }

    private void MoveGate(float distance)
    {
        if (isVertical)
        {
            Gate.transform.DOLocalMoveY(Gate.transform.localPosition.y + distance, moveDuration);
        }
        else
        {
            Gate.transform.DOLocalMoveX(Gate.transform.localPosition.x + distance, moveDuration);
        }
    }
}
