using UnityEngine;
using DG.Tweening;
using System.Collections;

public class LeverController : MonoBehaviour
{
    [SerializeField] private GameObject Gate;
    [SerializeField] private bool isGateOpen = false;
    [SerializeField] private bool isVertical = true; // Kap� y�n�n� Unity �zerinden se�ebilmek i�in
    [SerializeField] private float moveDistance = 4.59f; // Kap�n�n hareket mesafesi
    [SerializeField] private float moveDuration = 1f; // Kap�n�n hareket s�resi
    [SerializeField] private float cooldownTime = 3f; // 3 saniye tetikleme s�resi

    private Animator anim;
    private bool isCooldown = false; // Tetikleme s�resi kontrol�

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
        Debug.Log("�arp��an: " + collision.gameObject.name);

        // E�er cooldown aktifse (3 saniye dolmam��sa) kap� a��lmayacak
        if (isCooldown)
        {
            Debug.Log("Kap� a��lma s�resi dolmad�, bekleyin...");
            return;
        }

        if (collision.CompareTag("Player") || collision.CompareTag("Arrow") || collision.CompareTag("Spear"))
        {
            StartCoroutine(ToggleGate());
        }
    }

    private IEnumerator ToggleGate()
    {
        isCooldown = true; // Tetikleme s�resi ba�lat

        if (!isGateOpen)
        {
            Debug.Log("Kap� A��l�yor!");
            isGateOpen = true;

            if (anim != null)
            {
                anim.SetTrigger("openGate");
            }
            AudioManager.instance?.PlayAudio(8);
            yield return new WaitForSeconds(0.5f);
            MoveGate(moveDistance);
        }
        else
        {
            Debug.Log("Kap� Kapan�yor!");
            isGateOpen = false;

            if (anim != null)
            {
                anim.SetTrigger("closeGate");
            }
            AudioManager.instance?.PlayAudio(8);
            MoveGate(-moveDistance);
        }

        // 3 saniyelik cooldown ba�lat
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;
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
