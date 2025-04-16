using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotModeController : MonoBehaviour
{
    public static ShotModeController instance;

    [Header("Ayarlar")]
    public bool isShotMode = false; // At�� modu durumu

    [Header("Butonlar")]
    public List<Button> uiButtons; // Butonlar
    public List<GameObject> panelsToHide; // Paneller

    [Header("Karakterler")]
    [SerializeField] private GameObject bowPlayer;
    [SerializeField] private GameObject spearPlayer;

    [Header("Kontrol Panelleri")]
    public GameObject moveButtonPanel; // Move Button Panel
    public GameObject joystickPanel; // Joystick Panel

    private GameObject lastActivePanel; // Son aktif olan paneli takip eder

    private void Awake()
    {
        instance = this;
    }

    public void ToggleShotMode()
    {
        isShotMode = !isShotMode;

        if (isShotMode)
        {
            // Shot Mode a��ld���nda aktif olan paneli kaydet
            if (moveButtonPanel != null && moveButtonPanel.activeSelf)
            {
                lastActivePanel = moveButtonPanel;
            }
            else if (joystickPanel != null && joystickPanel.activeSelf)
            {
                lastActivePanel = joystickPanel;
            }

            // T�m panelleri gizle, moveButtonPanel ve joystickPanel'i ayr� tut
            if (moveButtonPanel != null) moveButtonPanel.SetActive(false);
            if (joystickPanel != null) joystickPanel.SetActive(false);

            // Di�er panelleri gizle
            foreach (var panel in panelsToHide)
            {
                if (panel != null && panel != moveButtonPanel && panel != joystickPanel)
                    panel.SetActive(false);
            }
        }
        else
        {
            // Shot Mode kapat�ld���nda yaln�zca son aktif olan paneli geri getir
            if (lastActivePanel != null)
            {
                lastActivePanel.SetActive(true);
            }

            // Di�er panelleri geri getir
            foreach (var panel in panelsToHide)
            {
                if (panel != null && panel != moveButtonPanel && panel != joystickPanel)
                    panel.SetActive(true);
            }
        }

        // UI butonlar�n� gizle/g�ster
        foreach (var btn in uiButtons)
        {
            if (btn != null)
                btn.gameObject.SetActive(!isShotMode); // Di�er butonlar� gizle
        }

        // Panelleri gizle/g�ster
        foreach (var panel in panelsToHide)
        {
            if (panel != null && panel != moveButtonPanel && panel != joystickPanel)
                panel.SetActive(!isShotMode);
        }
    }

    private void Update()
    {
        if (!isShotMode) return; // Shot Mode aktif de�ilse hi�bir �ey yapma

        if (Input.GetMouseButtonDown(0)) // Sol t�k (button 0)
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

    // Bu fonksiyonu kullanarak panelin aktif oldu�unu belirleyebilirsiniz
    public void SetActivePanel(GameObject panel)
    {
        lastActivePanel = panel;
    }
}
