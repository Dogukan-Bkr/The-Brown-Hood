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
        // Oyunun baþlangýç noktasý kaydedilmemiþse, oyuncunun konumunu kaydet
        if (GameManager.instance.startPosition == Vector3.zero)
        {
            GameManager.instance.startPosition = transform.position;
            GameManager.instance.startHP = maxHP;
        }

        // Eðer checkpoint kaydedilmiþse oradan baþlat, yoksa baþlangýç noktasýndan baþlat
        if (GameManager.instance.lastCheckPointPos != Vector3.zero)
        {
            transform.position = GameManager.instance.lastCheckPointPos;
            currentHP = GameManager.instance.lastCheckPointHP; // Checkpoint'teki can
        }
        else
        {
            transform.position = GameManager.instance.startPosition;
            currentHP = GameManager.instance.startHP; // Baþlangýçtaki can
        }

        UIController.instance.SetHealthSlider(currentHP, maxHP);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Player took damage: " + damage);
        currentHP -= damage;
        UIController.instance.SetHealthSlider(currentHP, maxHP);  // UI'yi güncelle

        if (currentHP <= 0)
        {
            PlayerMovementController.instance.PlayerDead();
            Die();  // Oyuncu öldüyse, yeniden doðmasýný saðla
        }
    }

    public void Die()
    {
        if (GameManager.instance.lastCheckPointPos != Vector3.zero)
        {
            // Checkpoint'e ýþýnla ve checkpoint'teki caný ver
            transform.position = GameManager.instance.lastCheckPointPos;
            currentHP = GameManager.instance.lastCheckPointHP;
        }
        else
        {
            // Hiç checkpoint alýnmadýysa baþlangýç noktasýna dön
            transform.position = GameManager.instance.startPosition;
            currentHP = GameManager.instance.startHP;
        }

        UIController.instance.SetHealthSlider(currentHP, maxHP);  // UI'yi güncelle
    }
}
