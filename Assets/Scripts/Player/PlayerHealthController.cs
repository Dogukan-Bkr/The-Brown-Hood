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
        // Oyunun ba�lang�� noktas� kaydedilmemi�se, oyuncunun konumunu kaydet
        if (GameManager.instance.startPosition == Vector3.zero)
        {
            GameManager.instance.startPosition = transform.position;
            GameManager.instance.startHP = maxHP;
        }

        // E�er checkpoint kaydedilmi�se oradan ba�lat, yoksa ba�lang�� noktas�ndan ba�lat
        if (GameManager.instance.lastCheckPointPos != Vector3.zero)
        {
            transform.position = GameManager.instance.lastCheckPointPos;
            currentHP = GameManager.instance.lastCheckPointHP; // Checkpoint'teki can
        }
        else
        {
            transform.position = GameManager.instance.startPosition;
            currentHP = GameManager.instance.startHP; // Ba�lang��taki can
        }

        UIController.instance.SetHealthSlider(currentHP, maxHP);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Player took damage: " + damage);
        currentHP -= damage;
        UIController.instance.SetHealthSlider(currentHP, maxHP);  // UI'yi g�ncelle

        if (currentHP <= 0)
        {
            PlayerMovementController.instance.PlayerDead();
            Die();  // Oyuncu �ld�yse, yeniden do�mas�n� sa�la
        }
    }

    public void Die()
    {
        if (GameManager.instance.lastCheckPointPos != Vector3.zero)
        {
            // Checkpoint'e ���nla ve checkpoint'teki can� ver
            transform.position = GameManager.instance.lastCheckPointPos;
            currentHP = GameManager.instance.lastCheckPointHP;
        }
        else
        {
            // Hi� checkpoint al�nmad�ysa ba�lang�� noktas�na d�n
            transform.position = GameManager.instance.startPosition;
            currentHP = GameManager.instance.startHP;
        }

        UIController.instance.SetHealthSlider(currentHP, maxHP);  // UI'yi g�ncelle
    }
}
