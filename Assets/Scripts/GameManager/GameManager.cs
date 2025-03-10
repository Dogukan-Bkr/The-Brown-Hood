using UnityEngine;

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
        }
    }
}
