using UnityEngine;
using DG.Tweening;
using System.Collections;

public class LeverController : MonoBehaviour
{
    [SerializeField] private GameObject Gate;
    [SerializeField] private bool isGateOpen = false;
    [SerializeField] private bool isVertical = true; // Kapý yönünü Unity üzerinden seçebilmek için
    [SerializeField] private float moveDistance = 4.59f; // Kapýnýn hareket mesafesi
    [SerializeField] private float moveDuration = 1f; // Kapýnýn hareket süresi
    [SerializeField] private float cooldownTime = 3f; // 3 saniye tetikleme süresi

    private Animator anim;
    private bool isCooldown = false; // Tetikleme süresi kontrolü

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
        Debug.Log("Çarpýþan: " + collision.gameObject.name);

        // Eðer cooldown aktifse (3 saniye dolmamýþsa) kapý açýlmayacak
        if (isCooldown)
        {
            Debug.Log("Kapý açýlma süresi dolmadý, bekleyin...");
            return;
        }

        if (collision.CompareTag("Player") || collision.CompareTag("Arrow") || collision.CompareTag("Spear"))
        {
            StartCoroutine(ToggleGate());
        }
    }

    private IEnumerator ToggleGate()
    {
        isCooldown = true; // Tetikleme süresi baþlat

        if (!isGateOpen)
        {
            Debug.Log("Kapý Açýlýyor!");
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
            Debug.Log("Kapý Kapanýyor!");
            isGateOpen = false;

            if (anim != null)
            {
                anim.SetTrigger("closeGate");
            }
            AudioManager.instance?.PlayAudio(8);
            MoveGate(-moveDistance);
        }

        // 3 saniyelik cooldown baþlat
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
