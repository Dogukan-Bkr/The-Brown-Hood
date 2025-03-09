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
        UIController.instance.SetHealthSlider(maxHP, currentHP);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        UIController.instance.SetHealthSlider(maxHP, currentHP);
        if (currentHP <= 0)
        {
            gameObject.SetActive(false);
            // Die();
        }
    }
}
