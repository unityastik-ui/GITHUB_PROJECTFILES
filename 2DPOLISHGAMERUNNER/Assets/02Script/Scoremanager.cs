using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [Header("SCORE SETTINGS")]
    public int score = 0;
    public float distancePerPoint = 10f;

    [Header("UI SETTINGS")]
    public TMP_Text scoreText;

    public int highscore;

    private float distance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        score = 0;
        distance = 0f;

        UpdateScoreUI();
        highscore = PlayerPrefs.GetInt("Highscore", 0);
    }

    private void Update()
    {
        if (GameDifficulty.Instance == null)
            return;

        // Calculate distance travelled by the world
        distance += GameDifficulty.Instance.CurrentSpeed * Time.deltaTime;

        // Every X distance = 1 score
        score = Mathf.FloorToInt(
            distance / distancePerPoint
        );

        if(score > highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("Highscore", highscore);
            PlayerPrefs.Save(); 
        }
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + score;
        }
    }
}