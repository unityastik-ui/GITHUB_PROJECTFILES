using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static Score instance;  

    public Text scoreText;
    public Text TotalscoreText;
    public GameObject player;

    float startX;
    float maxDistance;
    public int score;             

    void Awake()
    {
        instance = this;           
    }

    void Start()
    {
        startX = player.transform.position.x;
        maxDistance = 0f;
        score = 0;
    }

    void Update()
    {
        float currentDistance = player.transform.position.x - startX;

        if (currentDistance > maxDistance)
        {
            maxDistance = currentDistance;
            score = Mathf.FloorToInt(maxDistance * 2f);
        }

        scoreText.text ="SCORE:"+ score.ToString();
        TotalscoreText.text = score.ToString();
    }
}
