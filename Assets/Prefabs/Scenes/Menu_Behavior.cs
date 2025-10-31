using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Behavior : MonoBehaviour
{
    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Specific method for Settings button
    public void LoadSettings(string SceneName)
    {
        SceneManager.LoadScene("Settings");
    }

    // Specific method for About button
    public void LoadAbout(string SceneName)
    {
        SceneManager.LoadScene("About");
    }

    //back button
    public void LoadMainMenu(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
}
