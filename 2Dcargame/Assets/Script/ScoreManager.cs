using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score Settings")]
    public int score = 0;
    public int highScore = 0;
    private float scoreFloat = 0f;
    public float scoreRate = 5f;

    [Header("Coin Settings")]
    public int coinsCollected = 0;
    public int totalSavedCoins = 0;

    [Header("UI Texts")]
    public Text liveScoreText;
    public Text liveCoinText;
    public Text gameOverScoreText;
    public Text gameOverHighScoreText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Load saved High Score from device storage
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        totalSavedCoins = PlayerPrefs.GetInt("TotalCoins", 0);
    }

    void Update()
    {
        scoreFloat += scoreRate * Time.deltaTime;
        score = (int)scoreFloat;

        if (liveScoreText != null)
            liveScoreText.text = "SCORE: " + score;

        // Scale pitch from score 0 up to score 500
        float speedFactor = Mathf.Clamp01(score / 500f);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateEnginePitch(speedFactor);
        }
    }

    public void AddCoin(int amount = 1)
    {
        coinsCollected += amount;
        totalSavedCoins += amount;

        if (liveCoinText != null)
            liveCoinText.text = "COINS: " + coinsCollected;
    }

    public void ShowGameOverScore()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        PlayerPrefs.SetInt("TotalCoins", totalSavedCoins);
        PlayerPrefs.Save();

        if (gameOverScoreText != null)
            gameOverScoreText.text = score.ToString();

        if (gameOverHighScoreText != null)
            gameOverHighScoreText.text = "BEST: " + highScore.ToString();
    }
}