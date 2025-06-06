using UnityEngine;
using TMPro;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;
    public int maxHP, currentHP;

    [Header("Damage Display")]
    [SerializeField] private GameObject damageTextPrefab; // Hasar metni prefab�
    [SerializeField] private Transform damageTextPosition; // Hasar metninin ��kaca�� nokta
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Oyunun ba�lang�� noktas� kaydedilmemi�se, oyuncunun konumunu kaydet
        if (GameManager.instance != null)
        {
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
        }
        if (UIController.instance != null)
        {
            UIController.instance.SetHealthSlider(currentHP, maxHP);  // UI'yi g�ncelle
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Player took damage: " + damage);
        currentHP -= damage;
        UIController.instance.SetHealthSlider(currentHP, maxHP);  // UI'yi g�ncelle
        // Hasar metnini g�ster
        ShowDamageText(damage);

        // Hasar sesi �al
        AudioManager.instance?.PlayAudio(3); // �rne�in 3 = damage sesi

        if (currentHP <= 0)
        {
            PlayerMovementController.instance.PlayerDead();
            Die();  // Oyuncu �ld�yse, yeniden do�mas�n� sa�la
        }
    }


    private void ShowDamageText(int damage)
    {
        if (damageTextPrefab != null && damageTextPosition != null)
        {
            GameObject damageText = Instantiate(damageTextPrefab, damageTextPosition.position, Quaternion.identity);
            damageText.transform.SetParent(null);

            TextMeshPro textMesh = damageText.GetComponent<TextMeshPro>();
            if (textMesh != null)
            {
                textMesh.text = damage.ToString();
            }

            Destroy(damageText, 1f);
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

    public void GetHeal()
    {
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        UIController.instance.SetHealthSlider(currentHP, maxHP); // UI'yi g�ncelle
        AudioManager.instance?.PlayAudio(4);
    }
}
