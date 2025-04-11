using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;  // Scene y�netimi i�in gerekli

public class SceneTransition : MonoBehaviour
{
    public string sceneName;  // Y�nlendirilecek sahnenin ad�

    private void Start()
    {
        // 1 dakika sonra hedef sahneye ge�i� yap
        StartCoroutine(LoadSceneAfterDelay(23f));  
    }

    private IEnumerator LoadSceneAfterDelay(float delay)
    {
        // Bekleme s�resi
        yield return new WaitForSeconds(delay);

        // Sahneyi y�kle
        SceneManager.LoadScene(sceneName);
    }
}
