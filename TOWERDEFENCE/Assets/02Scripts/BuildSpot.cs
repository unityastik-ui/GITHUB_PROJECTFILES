using UnityEngine;
using UnityEngine.EventSystems;

public class BuildSpot : MonoBehaviour
{
    [SerializeField] private Transform placementAnchor;
    [SerializeField] private SpriteRenderer highlightRenderer;
    [SerializeField] private Color availableColor = new Color(0.35f, 1f, 0.45f, 0.8f);
    [SerializeField] private GameObject occupyingTower;

    public bool IsAvailable => occupyingTower == null;
    private Transform PlacementAnchor => placementAnchor != null ? placementAnchor : transform;

    private void Awake()
    {
        if (GetComponent<Collider2D>() == null)
            Debug.LogError($"[{nameof(BuildSpot)}] A Collider2D is required for clicking this slot.", this);

        if (highlightRenderer != null)
            highlightRenderer.enabled = false;
    }

    private void OnDisable()
    {
        if (highlightRenderer != null)
            highlightRenderer.enabled = false;
    }

    public void SetHighlighted(bool highlighted)
    {
        if (highlightRenderer == null)
        {
            if (highlighted)
                Debug.LogError($"[{nameof(BuildSpot)}] Assign a SpriteRenderer for the available-slot indicator.", this);
            return;
        }

        highlightRenderer.color = availableColor;
        highlightRenderer.enabled = highlighted && IsAvailable;
    }

    public bool TryPlaceTower(GameObject towerPrefab)
    {
        if (!IsAvailable || towerPrefab == null)
            return false;

        Transform anchor = PlacementAnchor;
        GameObject tower = Instantiate(towerPrefab, anchor.position, anchor.rotation);
        if (tower == null)
            return false;

        occupyingTower = tower;
        SetHighlighted(false);
        return true;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (IsAvailable && TowerPlacementManager.Instance != null)
            TowerPlacementManager.Instance.TryPlaceTower(this);
    }
}
