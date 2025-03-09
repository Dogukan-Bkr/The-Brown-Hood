using UnityEngine;

public class GameManager : MonoBehaviour
{
   public static GameManager instance;
    public int coinCount, arrowCount, spearCount;
    private void Awake()
    {
        instance = this;
    }
}
