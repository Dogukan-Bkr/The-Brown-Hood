using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Singleton tasarımı: UIController örneğine her yerden erişimi sağlar
    public static UIController instance;

    // UI bileşenleri
    public Slider healthSlider;  // Sağlık göstergesi (Slider)
    public TMP_Text coinTxt;     // Altın sayısını gösteren metin
    public TMP_Text arrowTxt;    // Ok sayısını gösteren metin
    public TMP_Text spearTxt;    // Mızrak sayısını gösteren metin

    private void Awake()
    {
        // Singleton örneğini atama
        instance = this;
    }

    // Sağlık göstergesini günceller
    public void SetHealthSlider(int currentHP, int maxHP)
    {
        healthSlider.maxValue = maxHP;  // Maksimum sağlık değerini ayarla
        healthSlider.value = currentHP; // Mevcut sağlık değerini güncelle
    }

    // UI’daki altın sayısını günceller
    public void SetCoinCount()
    {
        coinTxt.text = GameManager.instance.coinCount.ToString();
    }

    // UI’daki ok sayısını günceller
    public void SetArrowCount()
    {
        arrowTxt.text = GameManager.instance.arrowCount.ToString();
    }

    // UI’daki mızrak sayısını günceller
    public void SetSpearCount()
    {
        spearTxt.text = GameManager.instance.spearCount.ToString();
    }

    // Mızrak sayısını döndürür
    public int GetSpearCount()
    {
        return GameManager.instance.spearCount;
    }

    // Mızrak sayısını azaltır ve UI’ı günceller
    public void DecreaseSpearCount()
    {
        if (GameManager.instance.spearCount > 0)
        {
            GameManager.instance.spearCount--;
            SetSpearCount();
        }
    }
}
