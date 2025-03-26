using System.Collections;
using UnityEngine;

public class ScenesPassController : MonoBehaviour
{
    public string nextSceneName; // Ge�ilecek sahne ad�

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance.coinCount >= 40)
            {
                collision.GetComponent<PlayerMovementController>().StopPlayer(); // Oyuncuyu durdur
                collision.GetComponent<PlayerMovementController>().enabled = false; // Oyuncu hareketini kapat
                FadeController.instance.FadeIn(); // Karartmay� a�
                StartCoroutine(LoadScene()); // Belirtilen s�re sonra sahneyi y�kle
                Debug.Log("Oyuncunun coini yeterli, sahne ge�i�i yap�l�yor.");
            }
            else
            {
                Debug.Log("Oyuncunun coini yetersiz, sahne ge�i�i yap�lam�yor.");
            }
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1f); // 1 saniye bekle
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName); // Belirtilen sahneyi y�kle
    }
}
