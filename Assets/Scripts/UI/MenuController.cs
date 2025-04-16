using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject levelPanel; // Level panelini burada referans alaca��z
    public GameObject menuPanel; // Ana men� panel
                                 
    public GameObject MoveButtonPanel;  // Buton paneli
    public GameObject JoystickPanel;    // Joystick paneli

    public void TogglePanel()
    {
        // E�er joystick paneli aktifse, buton panelini a�, joystick panelini kapat
        if (JoystickPanel.activeSelf)
        {
            JoystickPanel.SetActive(false);  // Joystick panelini kapat
            MoveButtonPanel.SetActive(true); // Buton panelini a�
        }
        else
        {
            // E�er joystick paneli aktif de�ilse, buton panelini kapat, joystick panelini a�
            JoystickPanel.SetActive(true);   // Joystick panelini a�
            MoveButtonPanel.SetActive(false); // Buton panelini kapat
        }
    }

    public void OpenLevelPanel()
    {
        if (levelPanel != null)
        {
            levelPanel.SetActive(true); // Paneli aktif hale getir
            menuPanel.SetActive(false); // Ana men� panelini kapat
        }
    }
    public void CloseLevelPanel()
    {
        if (levelPanel != null)
        {
            levelPanel.SetActive(false); // Paneli kapat
            menuPanel.SetActive(true); // Ana men� panelini a�
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1); // Build index 1 olan sahneyi y�kler
    }
    public void QuitGame()
    {
        Application.Quit(); // Uygulamay� kapat�r
        Debug.Log("Quit Game"); 
    }
    public void LoadLevel(int levelIndex)
    {
        
        SceneManager.LoadScene(levelIndex);
    }
}
