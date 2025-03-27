using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesPassController : MonoBehaviour
{
    public string nextSceneName; // Ge�ilecek sahne ad�
    public int requiredCoins; // Sahne ge�i�i i�in gereken coin say�s� (sadece Level 1 i�in)
    public bool requireCoinsForThisScene; // Bu sahne i�in coin gereksinimi var m�?

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (requireCoinsForThisScene) // E�er bu sahnede coin gereksinimi varsa
            {
                if (GameManager.instance.coinCount >= requiredCoins)
                {
                    ProcessSceneTransition(collision);
                }
                else
                {
                    Debug.Log("Oyuncunun coini yetersiz, sahne ge�i�i yap�lam�yor.");
                }
            }
            else
            {
                ProcessSceneTransition(collision); // Coin gereksinimi yoksa direkt ge�i� yap
            }
        }
    }

    private void ProcessSceneTransition(Collider2D player)
    {
        player.GetComponent<PlayerMovementController>().StopPlayer(); // Oyuncuyu durdur
        player.GetComponent<PlayerMovementController>().enabled = false; // Hareketi kapat
        FadeController.instance.FadeIn(); // Karartmay� a�
        StartCoroutine(LoadScene()); // Sahne ge�i�ini ba�lat
        Debug.Log("Sahne ge�i�i yap�l�yor: " + nextSceneName);
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1f); // 1 saniye bekle
        SceneManager.LoadScene(nextSceneName); // Yeni sahneyi y�kle
    }
}
