using UnityEngine;
using TMPro;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;
    public int maxHP, currentHP;

    [Header("Damage Display")]
    [SerializeField] private GameObject damageTextPrefab; // Hasar metni prefabý
    [SerializeField] private Transform damageTextPosition; // Hasar metninin çýkacaðý nokta

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Oyunun baþlangýç noktasý kaydedilmemiþse, oyuncunun konumunu kaydet
        if (GameManager.instance != null)
        {
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
        }
        if (UIController.instance != null)
        {
            UIController.instance.SetHealthSlider(currentHP, maxHP);  // UI'yi güncelle
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Player took damage: " + damage);
        currentHP -= damage;
        UIController.instance.SetHealthSlider(currentHP, maxHP);  // UI'yi güncelle

        // Hasar metnini göster
        ShowDamageText(damage);

        if (currentHP <= 0)
        {
            PlayerMovementController.instance.PlayerDead();
            Die();  // Oyuncu öldüyse, yeniden doðmasýný saðla
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

    public void GetHeal()
    {
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        UIController.instance.SetHealthSlider(currentHP, maxHP); // UI'yi güncelle
        Debug.Log("Player HP (+20)");
    }
}
