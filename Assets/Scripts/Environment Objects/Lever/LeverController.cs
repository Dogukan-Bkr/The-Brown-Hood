using UnityEngine;
using DG.Tweening;

public class LeverController : MonoBehaviour
{
    [SerializeField] private GameObject Gate;
    [SerializeField] private bool isGateOpen = false;
    [SerializeField] private bool isVertical = true; // Kap� y�n�n� Unity �zerinden se�ebilmek i�in
    [SerializeField] private float moveDistance = 4.59f; // Kap�n�n hareket mesafesi
    [SerializeField] private float moveDuration = 1f; // Kap�n�n hareket s�resi

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator bile�eni eksik! " + gameObject.name);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("�arp��an: " + collision.gameObject.name); // �arp��an objeyi yazd�r

        if (collision.CompareTag("Player")) // Player ile �arp���yorsa
        {
            if (!isGateOpen) // Kap� kapal�ysa a�
            {
                Debug.Log("Kap� A��l�yor!");
                isGateOpen = true;

                if (anim != null)
                {
                    anim.SetTrigger("openGate");
                }

                // Animasyon s�resini bekleyerek kap�y� a�
                StartCoroutine(OpenGateAfterDelay(0.5f)); // 0.5 saniye gecikme ile a�
            }
            else // Kap� a��kken tekrar tetiklenirse kapat
            {
                Debug.Log("Kap� Kapan�yor!");
                isGateOpen = false;
                if (anim != null)
                {
                    anim.SetTrigger("closeGate");
                }

                // Kap�y� geri kapat
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
