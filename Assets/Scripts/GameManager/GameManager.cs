using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int coinCount, arrowCount, spearCount;

    public Vector3 lastCheckPointPos; // Son checkpoint pozisyonu
    public int lastCheckPointHP; // Checkpoint'te baþlatýlacak can

    public Vector3 startPosition; // Oyuncunun baþlangýç noktasý
    public int startHP; // Oyunun baþýndaki can

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // GameManager sahne deðiþtiðinde kaybolmasýn
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded; // Sahne deðiþtiðinde OnSceneLoaded çalýþtýr
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetStartPosition(scene.name);
        SetStartHP(scene.name);
        SetStartResources(scene.name);

        if (UIController.instance != null)
        {
            UIController.instance.UpdateUI();
        }
        else
        {
            Debug.LogWarning("UIController instance is null. UI update skipped.");
        }
    }


    private void SetStartPosition(string sceneName)
    {
        switch (sceneName)
        {
            case "Level1":
                startPosition = new Vector3(-1.63f, -1.21f, 0); // Level 1 baþlangýç noktasý
                break;
            case "Level2":
                startPosition = new Vector3(-27.91f, 6.72f, 0); // Level 2 baþlangýç noktasý
                break;
            default:
                startPosition = Vector3.zero; // Varsayýlan baþlangýç noktasý
                break;
        }

        lastCheckPointPos = startPosition; // Sahne deðiþtiðinde checkpoint'i de güncelle
    }

    private void SetStartHP(string sceneName)
    {
        switch (sceneName)
        {
            case "Level1":
                startHP = 100; // Level 1 baþlangýç caný
                break;
            case "Level2":
                if (PlayerHealthController.instance != null)
                {
                    if (PlayerHealthController.instance.currentHP < 50)
                    {
                        startHP = 50;
                    }
                    else
                    {
                        startHP = 100;
                    }
                }
                else
                {
                    Debug.LogWarning("PlayerHealthController instance is null. Defaulting startHP to 100.");
                    startHP = 100;
                }
                break;
            case "Level3":
                if (PlayerHealthController.instance != null)
                {
                    if (PlayerHealthController.instance.currentHP < 50)
                    {
                        startHP = 50;
                    }
                    else
                    {
                        startHP = 100;
                    }
                }
                else
                {
                    Debug.LogWarning("PlayerHealthController instance is null. Defaulting startHP to 100.");
                    startHP = 100;
                }
                break;
            default:
                startHP = 100; // Varsayýlan baþlangýç caný
                break;
        }

        lastCheckPointHP = startHP; // Sahne deðiþtiðinde checkpoint canýný da güncelle
    }

    private void SetStartResources(string sceneName)
    {
        switch (sceneName)
        {
            case "Level1":
                coinCount = 0; // Level 1 baþlangýç altýn sayýsý
                arrowCount = 0; // Level 1 baþlangýç ok sayýsý
                spearCount = 0; // Level 1 baþlangýç mýzrak sayýsý
                break;
            case "Level2":
                coinCount = 30; // Level 2 baþlangýç altýn sayýsý
                arrowCount = 15; // Level 2 baþlangýç ok sayýsý
                spearCount = 4; // Level 2 baþlangýç mýzrak sayýsý
                break;
            case "Level3":
                coinCount = 50; // Level 3 baþlangýç altýn sayýsý
                arrowCount = 20; // Level 3 baþlangýç ok sayýsý
                spearCount = 6; // Level 3 baþlangýç mýzrak sayýsý
                break;
            default:
                coinCount = 0; // Varsayýlan baþlangýç altýn sayýsý
                arrowCount = 0; // Varsayýlan baþlangýç ok sayýsý
                spearCount = 0; // Varsayýlan baþlangýç mýzrak sayýsý
                break;
        }
    }
}
