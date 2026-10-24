using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Scenemanagement : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("02Loadingscene");
    }
    [Header("Pause Menu")]
    public GameObject pausePanel;

    [Header("Setting Menu")]
    public GameObject settingPanel;

    [Header("Gameover Menu")]
    public GameObject gameoverPanel;
    public TMP_Text gameoverscoreText;
    public TMP_Text gameoverHighScoreText;

    public void QuitGame()
    {
        Application.Quit();
        print("Quit");
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Mainmenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("01MainMenu");
    }

    public void SettingGame()
    {
        settingPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeSetting()
    {
        settingPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Gameover()
    {
        gameoverscoreText.text = ScoreManager.instance.score.ToString();
        gameoverHighScoreText.text = ScoreManager.instance.highscore.ToString();
        gameoverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("03Gameplay");
    }
   
}
