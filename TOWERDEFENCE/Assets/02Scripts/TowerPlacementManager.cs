using UnityEngine;

[DefaultExecutionOrder(-100)]
public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager Instance { get; private set; }
    public static bool EscapeCancelledPlacementThisFrame =>
        Instance != null && Instance.escapeCancelledPlacementThisFrame;

    [SerializeField] private GameObject selectedTowerPrefab;
    private bool escapeCancelledPlacementThisFrame;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"[{nameof(TowerPlacementManager)}] More than one placement manager exists.", this);
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            ClearHighlights();
            Instance = null;
        }
    }

    private void Update()
    {
        escapeCancelledPlacementThisFrame = false;
        if (selectedTowerPrefab == null)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escapeCancelledPlacementThisFrame = true;
            CancelSelection();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            CancelSelection();
        }
    }

    public void SelectTower(GameObject towerPrefab)
    {
        if (towerPrefab == null)
        {
            Debug.LogError($"[{nameof(TowerPlacementManager)}] Cannot select an empty tower prefab.", this);
            CancelSelection();
            return;
        }

        if (selectedTowerPrefab == towerPrefab)
        {
            CancelSelection();
            return;
        }

        selectedTowerPrefab = towerPrefab;
        RefreshHighlights();
    }

    public void CancelSelection()
    {
        selectedTowerPrefab = null;
        ClearHighlights();
    }

    public void TryPlaceTower(BuildSpot spot)
    {
        if (selectedTowerPrefab == null || spot == null || !spot.IsAvailable)
            return;

        if (spot.TryPlaceTower(selectedTowerPrefab))
            CancelSelection();
    }

    private void RefreshHighlights()
    {
        ClearHighlights();

        BuildSpot[] spots = FindObjectsByType<BuildSpot>(FindObjectsSortMode.None);
        if (spots.Length == 0)
        {
            Debug.LogWarning($"[{nameof(TowerPlacementManager)}] No active BuildSpot components were found.", this);
            return;
        }

        foreach (BuildSpot spot in spots)
        {
            if (spot != null && spot.IsAvailable)
                spot.SetHighlighted(true);
        }
    }

    private static void ClearHighlights()
    {
        BuildSpot[] spots = FindObjectsByType<BuildSpot>(FindObjectsSortMode.None);
        foreach (BuildSpot spot in spots)
        {
            if (spot != null)
                spot.SetHighlighted(false);
        }
    }
}
