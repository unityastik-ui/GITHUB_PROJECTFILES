using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject HUDPANEL;

    public GameObject VictoryPanel;
    public GameObject DefeatPanel;
    public GameObject joystick;
    private void Awake()
    {
        instance = this;
    }

    public void EndGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOver();
        }

        HUDPANEL.SetActive(false);
        DefeatPanel.SetActive(true);

        ScoreManager.instance.ShowFinalScore();
        joystick.SetActive(false);

        Time.timeScale = 0f;
    }

    public void WonGame()
    {
        joystick.SetActive(false);
        HUDPANEL.SetActive(false);
        VictoryPanel.SetActive(true);
        ScoreManager.instance.ShowVictoryScore();
        CoinManager.Instance.ShowVictoryCoins();
        Time.timeScale = 0f;
    }
}
