using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;  // Scene yönetimi için gerekli

public class SceneTransition : MonoBehaviour
{
    public string sceneName;  // Yönlendirilecek sahnenin adý

    private void Start()
    {
        // 1 dakika sonra hedef sahneye geçiþ yap
        StartCoroutine(LoadSceneAfterDelay(23f));  
    }

    private IEnumerator LoadSceneAfterDelay(float delay)
    {
        // Bekleme süresi
        yield return new WaitForSeconds(delay);

        // Sahneyi yükle
        SceneManager.LoadScene(sceneName);
    }
}
