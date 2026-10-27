using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public GameObject levelCompleteScreen;
    public Text coinsText;
    public Text scoreText;
    public GameObject mobileControls;

    public bool levelCompleted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CoinManager.instance.ResetLevelCoins();
    }

    public void LevelComplete()
    {
        mobileControls.SetActive(false);
        Debug.Log("LEVEL COMPLETED TRIGGERED"); 
        levelCompleted = true;

        coinsText.text = CoinManager.instance.levelCoins.ToString();
        scoreText.text = Score.instance.score.ToString();

        levelCompleteScreen.SetActive(true);
        Time.timeScale = 0f;
    }
}
