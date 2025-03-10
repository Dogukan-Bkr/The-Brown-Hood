using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Singleton tasarýmý: UIController örneðine her yerden eriþimi saðlar
    public static UIController instance;

    // UI bileþenleri
    public Slider healthSlider;  // Saðlýk göstergesi (Slider)
    public TMP_Text coinTxt;     // Altýn sayýsýný gösteren metin
    public TMP_Text arrowTxt;    // Ok sayýsýný gösteren metin
    public TMP_Text spearTxt;    // Mýzrak sayýsýný gösteren metin

    private void Awake()
    {
        // Singleton örneðini atama
        instance = this;
    }

    // Saðlýk göstergesini günceller
    public void SetHealthSlider(int currentHP, int maxHP)
    {
        healthSlider.maxValue = maxHP;  // Maksimum saðlýk deðerini ayarla
        healthSlider.value = currentHP; // Mevcut saðlýk deðerini güncelle
    }

    // UI’daki altýn sayýsýný günceller
    public void SetCoinCount()
    {
        coinTxt.text = GameManager.instance.coinCount.ToString();
    }

    // UI’daki ok sayýsýný günceller
    public void SetArrowCount()
    {
        arrowTxt.text = GameManager.instance.arrowCount.ToString();
    }

    // UI’daki mýzrak sayýsýný günceller
    public void SetSpearCount()
    {
        spearTxt.text = GameManager.instance.spearCount.ToString();
    }
}
