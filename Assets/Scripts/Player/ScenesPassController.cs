using System.Collections;
using UnityEngine;

public class ScenesPassController : MonoBehaviour
{
    public string nextSceneName; // Ge�ilecek sahne ad�

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMovementController>().StopPlayer(); // Oyuncuyu durdur
            collision.GetComponent<PlayerMovementController>().enabled = false; // Oyuncu hareketini kapat
            FadeController.instance.FadeIn(); // Karartmay� a�
            StartCoroutine(LoadScene()); // Belirtilen s�re sonra sahneyi y�kle
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1f); // 1 saniye bekle
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName); // Belirtilen sahneyi y�kle
    }
}
