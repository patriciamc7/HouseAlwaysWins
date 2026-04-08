using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject canvas2;
    public void PlayGame()
    {
        canvas2.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMenu()
    {
        canvas2.SetActive(false);
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
}