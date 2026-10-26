using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Exact name of your gameplay scene from the Assets/01Scenes folder
    [SerializeField] private string gameplaySceneName = "02GAMEPLAYSCREEN";

    /// <summary>
    /// Loads the main gameplay scene.
    /// </summary>
    public void PlayGame()
    {
        // Reset game speed in case previous game was paused
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplaySceneName);
    }

    /// <summary>
    /// Quits the application and prints a confirmation in the Unity Console.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Application Quitted!");
        Application.Quit();
    }
}