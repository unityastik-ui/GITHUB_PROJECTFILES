using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager1 : MonoBehaviour
{
    [Header("Pause UI")]
    public GameObject pauseUI;

    [Header("Level Complete UI")]
    public GameObject levelCompleteUI;
    [Header("Game Over UI")]
    public GameObject gameOverUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
                Pause();
            else
                Resume();
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1");
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        if (pauseUI != null)
            pauseUI.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        if (pauseUI != null)
            pauseUI.SetActive(false);
    }

    // 🔥 LEVEL COMPLETE
    public void LevelComplete()
    {
        Time.timeScale = 0f;
        if (levelCompleteUI != null)
            levelCompleteUI.SetActive(true);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Replay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Home()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("HomeScreen");
    }

    public void Quit()
    {
        Application.Quit();
    }
    public void PlayerDied()
    {
        Time.timeScale = 0f;          // STOP EVERYTHING
        if (gameOverUI != null)
            gameOverUI.SetActive(true);
    }
}
