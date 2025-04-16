using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject levelPanel; // Level panelini burada referans alacaðýz
    public GameObject menuPanel; // Ana menü panel
                                 
    public GameObject MoveButtonPanel;  // Buton paneli
    public GameObject JoystickPanel;    // Joystick paneli

    public void TogglePanel()
    {
        // Eðer joystick paneli aktifse, buton panelini aç, joystick panelini kapat
        if (JoystickPanel.activeSelf)
        {
            JoystickPanel.SetActive(false);  // Joystick panelini kapat
            MoveButtonPanel.SetActive(true); // Buton panelini aç
        }
        else
        {
            // Eðer joystick paneli aktif deðilse, buton panelini kapat, joystick panelini aç
            JoystickPanel.SetActive(true);   // Joystick panelini aç
            MoveButtonPanel.SetActive(false); // Buton panelini kapat
        }
    }

    public void OpenLevelPanel()
    {
        if (levelPanel != null)
        {
            levelPanel.SetActive(true); // Paneli aktif hale getir
            menuPanel.SetActive(false); // Ana menü panelini kapat
        }
    }
    public void CloseLevelPanel()
    {
        if (levelPanel != null)
        {
            levelPanel.SetActive(false); // Paneli kapat
            menuPanel.SetActive(true); // Ana menü panelini aç
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1); // Build index 1 olan sahneyi yükler
    }
    public void QuitGame()
    {
        Application.Quit(); // Uygulamayý kapatýr
        Debug.Log("Quit Game"); 
    }
    public void LoadLevel(int levelIndex)
    {
        
        SceneManager.LoadScene(levelIndex);
    }
}
