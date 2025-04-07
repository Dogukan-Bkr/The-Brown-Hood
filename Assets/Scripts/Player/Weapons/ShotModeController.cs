using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotModeController : MonoBehaviour
{
    public static ShotModeController instance;

    [Header("Ayarlar")]
    public bool isShotMode = false; // Atýþ modu durumu

    [Header("Butonlar")]
    public List<Button> uiButtons; // Butonlar
    public List<GameObject> panelsToHide; // Paneller
    public Button button0; // Button 0'ý özel olarak belirle

    [Header("Karakterler")]
    [SerializeField] private GameObject bowPlayer;
    [SerializeField] private GameObject spearPlayer;

    private void Awake()
    {
        instance = this;
    }

    public void ToggleShotMode()
    {
        isShotMode = !isShotMode;

        // UI butonlarýný gizle/göster
        foreach (var btn in uiButtons)
        {
            if (btn != null)
                btn.gameObject.SetActive(!isShotMode); // Diðer butonlarý gizle
        }

        // Button 0'ý kontrol et
        if (button0 != null)
        {
            button0.gameObject.SetActive(isShotMode); // Button 0 sadece atýþ modunda aktif olacak
        }

        // Panelleri gizle/göster
        foreach (var panel in panelsToHide)
        {
            if (panel != null)
                panel.SetActive(!isShotMode);
        }
    }

    private void Update()
    {
        if (!isShotMode) return; // Shot Mode aktif deðilse hiçbir þey yapma

        if (Input.GetMouseButtonDown(0)) // Sol týk (button 0)
        {
            if (spearPlayer != null && spearPlayer.activeSelf && SpearController.instance != null)
            {
                SpearController.instance.SpearShoot();
            }
            else if (bowPlayer != null && bowPlayer.activeSelf && BowController.instance != null)
            {
                BowController.instance.BowShoot();
            }
        }
    }
}
