using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;
    public TextMeshProUGUI coinText;
    public int TotalCoins,Runtimecoin=0;
    public TextMeshProUGUI VictorytotalCointext,losecontext;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
       
        TotalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        Runtimecoin = 0;

        coinText.text = "Coins: " + Runtimecoin;

    }
    public void AddCoin() // Call this method when the player collects a coin
    {
        print("Coin Added");
        Runtimecoin++;//PRINT MESSAGE

        TotalCoins++;

        coinText.text = "Coins: " + Runtimecoin;

        PlayerPrefs.SetInt("TotalCoins", TotalCoins);//SAVE TO PLAYER PREFS PERMANANTLY
        coinText.text = "Coins: " + Runtimecoin;
    }
    public void ShowVictoryCoins()
    {
        VictorytotalCointext.text = TotalCoins.ToString();

        print("Total Coins after Victory: " + TotalCoins);

        PlayerPrefs.Save();

    }
    public void ShowLoseCoins()
    {
        losecontext.text = TotalCoins.ToString();

        print("Total Coins after Lose: " + TotalCoins);

        PlayerPrefs.Save();
    }
}

