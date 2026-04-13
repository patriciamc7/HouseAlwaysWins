using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject playCanvas;
    [SerializeField] GameObject languageCanvas;
    public void PlayGame()
    {
        playCanvas.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMenu()
    {
        playCanvas.SetActive(false);
    }

    public void Play()
    {
        GameState.loadSavedGame = false;
        SceneManager.LoadScene("Game");
    }

    public void Load()
    {
        GameState.loadSavedGame = true;
        SceneManager.LoadScene("Game");
    }

    public void OpenLanguage()
    {
        languageCanvas.SetActive(true);
    }

    public void CloseLanguage()
    {
        languageCanvas.SetActive(false);
    }
}