using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Shopcoinshow : MonoBehaviour
{
    public static Shopcoinshow Instance;

    [FormerlySerializedAs("totalcointext")]
    public TextMeshProUGUI totalCoinText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple Shopcoinshow instances found. Destroying the duplicate.", this);
            Destroy(this);
            return;
        }

        Instance = this;
        UpdateCoinDisplay();
    }

    private void Start()
    {
        UpdateCoinDisplay();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void UpdateCoinDisplay()
    {
        int coins = PlayerPrefs.GetInt("TotalCoins", 0);
        if (totalCoinText == null)
        {
            Debug.LogWarning("Shop coin display text is not assigned.", this);
            return;
        }

        totalCoinText.text = coins.ToString();
    }
}
