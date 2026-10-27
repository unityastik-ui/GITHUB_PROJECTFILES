using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;

    public Text coinText;
    public int levelCoins = 0;

    public int requiredCoinsToComplete = 200; // SET PER LEVEL

    bool gateOpened = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void AddCoin()
    {
        levelCoins += 10;

        if (coinText != null)
            coinText.text = "COINS: " + levelCoins;

        // ⭐ OPEN FLAG ONLY (NOT LEVEL COMPLETE)
        if (!gateOpened && levelCoins >= requiredCoinsToComplete)
        {
            gateOpened = true;
            FlagGate.instance.OpenGate();
        }
    }

    public void ResetLevelCoins()
    {
        levelCoins = 0;

        if (coinText != null)
            coinText.text = "COINS: 0";
    }
}
