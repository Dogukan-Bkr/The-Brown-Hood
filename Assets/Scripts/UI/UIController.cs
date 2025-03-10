using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Singleton tasar�m�: UIController �rne�ine her yerden eri�imi sa�lar
    public static UIController instance;

    // UI bile�enleri
    public Slider healthSlider;  // Sa�l�k g�stergesi (Slider)
    public TMP_Text coinTxt;     // Alt�n say�s�n� g�steren metin
    public TMP_Text arrowTxt;    // Ok say�s�n� g�steren metin
    public TMP_Text spearTxt;    // M�zrak say�s�n� g�steren metin

    private void Awake()
    {
        // Singleton �rne�ini atama
        instance = this;
    }

    // Sa�l�k g�stergesini g�nceller
    public void SetHealthSlider(int currentHP, int maxHP)
    {
        healthSlider.maxValue = maxHP;  // Maksimum sa�l�k de�erini ayarla
        healthSlider.value = currentHP; // Mevcut sa�l�k de�erini g�ncelle
    }

    // UI�daki alt�n say�s�n� g�nceller
    public void SetCoinCount()
    {
        coinTxt.text = GameManager.instance.coinCount.ToString();
    }

    // UI�daki ok say�s�n� g�nceller
    public void SetArrowCount()
    {
        arrowTxt.text = GameManager.instance.arrowCount.ToString();
    }

    // UI�daki m�zrak say�s�n� g�nceller
    public void SetSpearCount()
    {
        spearTxt.text = GameManager.instance.spearCount.ToString();
    }
}
