using UnityEngine;

public class GameDifficulty : MonoBehaviour
{
    public static GameDifficulty Instance;

    [Header("Difficulty Settings")]
    public float startSpeed = 5f;
    public float maxSpeed = 13f;
    public float acceleration = 3f;

    public float CurrentSpeed { get; private set; }

    private void Awake()
    {
        Instance = this;
        CurrentSpeed = startSpeed;
    }

    private void Update()
    {
        int score = ScoreManager.instance.score;

        // Score 0 - 14
        if (score < 15)
        {
            maxSpeed = 5f;
        }

        // Score 15 - 49
        else if (score < 50)
        {
            maxSpeed = 7f;
        }

        // Score 50 - 74
        else if (score < 75)
        {
            maxSpeed = 9f;
        }

        // Score 75 - 100
        else if (score <= 100)
        {
            maxSpeed = 10f;
        }

        // Score 101+
        else
        {
            maxSpeed = 13f;
        }

        CurrentSpeed = Mathf.MoveTowards(
            CurrentSpeed,
            maxSpeed,
            acceleration * Time.deltaTime
        );
    }
}