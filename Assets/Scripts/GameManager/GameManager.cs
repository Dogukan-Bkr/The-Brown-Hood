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
        SetStartPosition(scene.name); // Yeni sahnenin baþlangýç pozisyonunu ayarla
        SetStartHP(scene.name); // Yeni sahnenin baþlangýç canýný ayarla
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
                startHP = 100; // Level 2 baþlangýç caný
                break;
            default:
                startHP = 100; // Varsayýlan baþlangýç caný
                break;
        }

        lastCheckPointHP = startHP; // Sahne deðiþtiðinde checkpoint canýný da güncelle
    }
}
