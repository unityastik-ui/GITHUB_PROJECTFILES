using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public Text gameOverScoreText;

    [Header("Crash Effects")]
    [SerializeField] float cameraShakeDuration = 0.25f;
    [SerializeField] float cameraShakeMagnitude = 0.2f;

    bool isPaused;
    bool isGameOver;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "GamePlay")
            StartGameplayAudio();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (Instance == this)
            Instance = null;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isPaused = false;
        isGameOver = false;

        if (scene.name == "GamePlay")
            StartGameplayAudio();
    }

    void StartGameplayAudio()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StartEngine();
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
        SceneManager.LoadScene("GamePlay");
    }

    public void PauseGame()
    {
        if (isPaused || isGameOver)
            return;

        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PauseEngine();
    }

    public void ResumeGame()
    {
        if (!isPaused || isGameOver)
            return;

        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (AudioManager.Instance != null)
            AudioManager.Instance.ResumeEngine();
    }

    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;
        isPaused = false;
        Time.timeScale = 0f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCrashSFX();
            AudioManager.Instance.StopEngine();
        }

        StartCoroutine(ShakeCameraAndShowGameOver());
    }

    IEnumerator ShakeCameraAndShowGameOver()
    {
        Transform cameraTransform = Camera.main != null ? Camera.main.transform : null;
        Vector3 cameraStartPosition = cameraTransform != null ? cameraTransform.localPosition : Vector3.zero;
        float elapsed = 0f;

        while (elapsed < cameraShakeDuration)
        {
            if (cameraTransform != null)
            {
                Vector2 offset = Random.insideUnitCircle * cameraShakeMagnitude;
                cameraTransform.localPosition = cameraStartPosition + new Vector3(offset.x, offset.y, 0f);
            }

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (cameraTransform != null)
            cameraTransform.localPosition = cameraStartPosition;

        if (gameOverScoreText != null && ScoreManager.Instance != null)
            gameOverScoreText.text = ScoreManager.Instance.score.ToString();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        else
            Debug.LogError("GameOverPanel is not assigned on the GameManager.", this);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopEngine();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoHome()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopEngine();

        SceneManager.LoadScene("HomeSCreen");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
