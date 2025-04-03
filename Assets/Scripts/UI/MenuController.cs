using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject levelPanel; // Level panelini burada referans alacaðýz
    public GameObject menuPanel; // Ana menü paneli
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
