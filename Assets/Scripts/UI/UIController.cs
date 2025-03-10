using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public Slider healthSlider;
    public TMP_Text coinTxt, arrowTxt, spearTxt;
    
    private void Awake()
    {
     instance = this;
    }

    public void SetHealthSlider(int currentHP, int maxHP)
    {
        healthSlider.maxValue = maxHP;  // Maksimum saðlýk
        healthSlider.value = currentHP; // Mevcut saðlýk
    }

    public void SetCoinCount()
    {
        coinTxt.text = GameManager.instance.coinCount.ToString();
    }
    public void SetArrowCount()
    {
        arrowTxt.text = GameManager.instance.arrowCount.ToString();
    }
    public void SetSpearCount()
    {
        spearTxt.text = GameManager.instance.spearCount.ToString();
    }
}
