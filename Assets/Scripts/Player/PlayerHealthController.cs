using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;
    public int maxHP, currentHP;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        currentHP = maxHP;
    }

    
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            gameObject.SetActive(false);
            // Die();
        }
    }
}
