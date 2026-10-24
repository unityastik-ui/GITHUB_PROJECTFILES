using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyButton : MonoBehaviour
{
    private const string SelectedPlaneKey = "SelectedPlane";
    private const string TotalCoinsKey = "TotalCoins";
    private const string PlaneUnlockedKeyPrefix = "PlaneUnlocked_";

    public DatabasePlane databasePlane;
    public int planeIndex;
    public Renderer planeRenderer;
    public Button actionButton;
    public TextMeshProUGUI buttonText;

    [Header("Lock Visuals")]
    public GameObject lockIcon;
    public Image buttonBackgroundImage;

    private void Start()
    {
        ResolveUIReferences();

        if (!HasValidPlaneData())
        {
            Debug.LogError($"BuyButton on {name} has an invalid plane database or plane index.", this);
            return;
        }

        int selectedPlane = PlayerPrefs.GetInt(SelectedPlaneKey, 0);
        if (selectedPlane < 0 || selectedPlane >= databasePlane.planeDatas.Length)
        {
            Debug.LogWarning($"Saved selected plane index {selectedPlane} is invalid. Using plane 0.", this);
            selectedPlane = 0;
        }

        ApplyPlaneMaterial(databasePlane.planeDatas[selectedPlane]);
        RefreshUI();
    }

    public void RefreshUI()
    {
        ResolveUIReferences();

        if (!HasValidPlaneData())
        {
            Debug.LogError($"BuyButton on {name} has an invalid plane database or plane index.", this);
            return;
        }

        bool isEquipped = PlayerPrefs.GetInt(SelectedPlaneKey, 0) == planeIndex;
        bool isUnlocked = IsUnlocked(planeIndex);

        if (buttonText != null)
        {
            buttonText.text = isEquipped ? "EQUIPPED" : isUnlocked ? "EQUIP" : "BUY";
        }

        if (actionButton != null)
        {
            actionButton.interactable = !isEquipped;
        }

        if (lockIcon != null)
        {
            lockIcon.SetActive(!isUnlocked);
        }
    }

    public bool IsUnlocked(int index)
    {
        if (index == 0)
        {
            return true;
        }

        if (index < 0)
        {
            return false;
        }

        return PlayerPrefs.GetInt(PlaneUnlockedKeyPrefix + index, 0) == 1;
    }

    public void OnClickButton()
    {
        if (!HasValidPlaneData())
        {
            Debug.LogError($"BuyButton on {name} has an invalid plane database or plane index.", this);
            return;
        }

        PlaneData planeData = databasePlane.planeDatas[planeIndex];
        if (IsUnlocked(planeIndex))
        {
            EquipPlane(planeData);
            Debug.Log("Equipped plane: " + planeIndex);
            return;
        }

        int totalCoins = PlayerPrefs.GetInt(TotalCoinsKey, 0);
        int price = planeData.PlanePrice;
        if (price < 0)
        {
            Debug.LogError($"Plane {planeIndex} has an invalid negative price.", this);
            return;
        }

        if (totalCoins < price)
        {
            Debug.LogWarning("Not enough coins to purchase plane " + planeIndex);
            return;
        }

        totalCoins -= price;
        PlayerPrefs.SetInt(TotalCoinsKey, totalCoins);
        PlayerPrefs.SetInt(PlaneUnlockedKeyPrefix + planeIndex, 1);
        PlayerPrefs.SetInt(SelectedPlaneKey, planeIndex);
        PlayerPrefs.Save();

        if (Shopcoinshow.Instance != null)
        {
            Shopcoinshow.Instance.UpdateCoinDisplay();
        }

        ApplyPlaneMaterial(planeData);
        RefreshAllButtons();
        Debug.Log("Purchased and equipped plane " + planeIndex + "! Remaining coins: " + totalCoins);
    }

    private void EquipPlane(PlaneData planeData)
    {
        PlayerPrefs.SetInt(SelectedPlaneKey, planeIndex);
        PlayerPrefs.Save();
        ApplyPlaneMaterial(planeData);
        RefreshAllButtons();
    }

    private void ApplyPlaneMaterial(PlaneData planeData)
    {
        if (planeRenderer == null)
        {
            Debug.LogWarning($"Plane preview Renderer is not assigned on {name}.", this);
            return;
        }

        if (planeData == null || planeData.planeMaterial == null)
        {
            Debug.LogWarning($"Plane material is not assigned for plane index {planeIndex}.", this);
            return;
        }

        planeRenderer.material = planeData.planeMaterial;
    }

    private void ResolveUIReferences()
    {
        if (actionButton == null)
        {
            actionButton = GetComponent<Button>();
        }

        if (buttonText == null)
        {
            buttonText = GetComponentInChildren<TextMeshProUGUI>(true);
        }
    }

    private bool HasValidPlaneData()
    {
        return databasePlane != null &&
               databasePlane.planeDatas != null &&
               planeIndex >= 0 &&
               planeIndex < databasePlane.planeDatas.Length &&
               databasePlane.planeDatas[planeIndex] != null;
    }

    private static void RefreshAllButtons()
    {
        BuyButton[] buyButtons = FindObjectsByType<BuyButton>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        foreach (BuyButton buyButton in buyButtons)
        {
            buyButton.RefreshUI();
        }
    }
}
