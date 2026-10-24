using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;
    public TextMeshProUGUI GameoverScoreText;
    public TextMeshProUGUI GameoverhighScoreText;

    public TextMeshProUGUI VictoryHighscoretext;


    private int score;
    private int highscore;

    public GameObject Player;
    private float PstartPosition;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        PstartPosition = Player.transform.position.z;
        score = 0;
        highscore = PlayerPrefs.GetInt("HighScore", 0);
        highscoreText.text = "HighScore: " + highscore.ToString();
    }

    private void Update()
    {
        float currentDistance = Player.transform.position.z - PstartPosition;

        if (currentDistance > 0)
        {
            score = Mathf.FloorToInt(currentDistance * 0.05f);
            scoreText.text = "SCORE: " + score.ToString();
            if (score > highscore)
            {
                highscore = score;
                highscoreText.text = "HighScore: " + highscore.ToString();
                PlayerPrefs.SetInt("HighScore", highscore);
            }
        }
    }
    public int GetScore()
    {
        return score;
    }
    public void ShowFinalScore()
    {
        int score = GetScore();
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        GameoverScoreText.text = score.ToString();
        GameoverhighScoreText.text = highScore.ToString();
    }
    public void ShowVictoryScore()
    {
        int score = GetScore();
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        // 👉 Only high score shown
        VictoryHighscoretext.text = highScore.ToString();

        // Optional: hide score text if it's on same panel
       // GameoverScoreText.gameObject.SetActive(false);
    }
}