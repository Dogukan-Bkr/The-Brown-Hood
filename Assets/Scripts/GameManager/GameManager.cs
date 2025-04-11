using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int coinCount, arrowCount, spearCount;

    public Vector3 lastCheckPointPos; // Son checkpoint pozisyonu
    public int lastCheckPointHP; // Checkpoint'te ba�lat�lacak can

    public Vector3 startPosition; // Oyuncunun ba�lang�� noktas�
    public int startHP; // Oyunun ba��ndaki can

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // GameManager sahne de�i�ti�inde kaybolmas�n
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded; // Sahne de�i�ti�inde OnSceneLoaded �al��t�r
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
                startPosition = new Vector3(-1.63f, -1.21f, 0); // Level 1 ba�lang�� noktas�
                break;
            case "Level2":
                startPosition = new Vector3(-27.91f, 6.72f, 0); // Level 2 ba�lang�� noktas�
                break;
            default:
                startPosition = Vector3.zero; // Varsay�lan ba�lang�� noktas�
                break;
        }

        lastCheckPointPos = startPosition; // Sahne de�i�ti�inde checkpoint'i de g�ncelle
    }

    private void SetStartHP(string sceneName)
    {
        switch (sceneName)
        {
            case "Level1":
                startHP = 100; // Level 1 ba�lang�� can�
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
                startHP = 100; // Varsay�lan ba�lang�� can�
                break;
        }

        lastCheckPointHP = startHP; // Sahne de�i�ti�inde checkpoint can�n� da g�ncelle
    }

    private void SetStartResources(string sceneName)
    {
        switch (sceneName)
        {
            case "Level1":
                coinCount = 0; // Level 1 ba�lang�� alt�n say�s�
                arrowCount = 0; // Level 1 ba�lang�� ok say�s�
                spearCount = 0; // Level 1 ba�lang�� m�zrak say�s�
                break;
            case "Level2":
                coinCount = 30; // Level 2 ba�lang�� alt�n say�s�
                arrowCount = 15; // Level 2 ba�lang�� ok say�s�
                spearCount = 4; // Level 2 ba�lang�� m�zrak say�s�
                break;
            case "Level3":
                coinCount = 50; // Level 3 ba�lang�� alt�n say�s�
                arrowCount = 20; // Level 3 ba�lang�� ok say�s�
                spearCount = 6; // Level 3 ba�lang�� m�zrak say�s�
                break;
            default:
                coinCount = 0; // Varsay�lan ba�lang�� alt�n say�s�
                arrowCount = 0; // Varsay�lan ba�lang�� ok say�s�
                spearCount = 0; // Varsay�lan ba�lang�� m�zrak say�s�
                break;
        }
    }
}
