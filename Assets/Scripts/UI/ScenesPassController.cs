using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesPassController : MonoBehaviour
{
    public string nextSceneName; // Geçilecek sahne adý
    public int requiredCoins; // Sahne geçiþi için gereken coin sayýsý (sadece Level 1 için)
    public bool requireCoinsForThisScene; // Bu sahne için coin gereksinimi var mý?

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (requireCoinsForThisScene) // Eðer bu sahnede coin gereksinimi varsa
            {
                if (GameManager.instance.coinCount >= requiredCoins)
                {
                    ProcessSceneTransition(collision);
                }
                else
                {
                    Debug.Log("Oyuncunun coini yetersiz, sahne geçiþi yapýlamýyor.");
                }
            }
            else
            {
                ProcessSceneTransition(collision); // Coin gereksinimi yoksa direkt geçiþ yap
            }
        }
    }

    private void ProcessSceneTransition(Collider2D player)
    {
        player.GetComponent<PlayerMovementController>().StopPlayer(); // Oyuncuyu durdur
        player.GetComponent<PlayerMovementController>().enabled = false; // Hareketi kapat
        FadeController.instance.FadeIn(); // Karartmayý aç
        StartCoroutine(LoadScene()); // Sahne geçiþini baþlat
        Debug.Log("Sahne geçiþi yapýlýyor: " + nextSceneName);
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1f); // 1 saniye bekle
        SceneManager.LoadScene(nextSceneName); // Yeni sahneyi yükle
    }
}
