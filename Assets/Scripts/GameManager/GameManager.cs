using UnityEngine;

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
        }
    }
}
