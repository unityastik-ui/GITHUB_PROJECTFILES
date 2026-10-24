using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
                PauseGame();
            else
                ResumeGame();
        }
    }
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        UImanager.Instance.ShowPauseUI();
    }


    public void ResumeGame()
    {
        Time.timeScale = 1f;
        UImanager.Instance.HidePauseUI();
    }
    public void LevelComplete()
    {
        Time.timeScale = 0f;
        UImanager.Instance.ShowLevelCompleteUI();
    }
    public void Home()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("HomeScreen");
    }
    public void Quit()
    {
        print("Quit");
        Application.Quit();
    }
}


