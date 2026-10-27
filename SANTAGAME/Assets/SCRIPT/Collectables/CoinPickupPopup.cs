using TMPro;
using UnityEngine;

public class CoinPickupPopup : MonoBehaviour
{
    private TextMeshPro popupText;
    private Vector3 startPosition;
    private Color startColor;
    private float lifetime;
    private float riseDistance;
    private float elapsed;

    public static void Show(Vector3 position, float riseDistance, float lifetime)
    {
        GameObject popupObject = new GameObject("Coin Pickup Popup");
        CoinPickupPopup popup = popupObject.AddComponent<CoinPickupPopup>();
        popup.Initialize(position, riseDistance, lifetime);
    }

    private void Initialize(Vector3 position, float rise, float duration)
    {
        popupText = gameObject.AddComponent<TextMeshPro>();
        popupText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");

        if (popupText.font == null)
        {
            Debug.LogError("Coin pickup popup could not load the LiberationSans SDF font asset.");
            Destroy(gameObject);
            return;
        }

        popupText.text = "+1";
        popupText.alignment = TextAlignmentOptions.Center;
        popupText.fontSize = 36f;
        popupText.fontStyle = FontStyles.Bold;
        popupText.color = new Color(1f, 0.85f, 0.25f, 1f);
        popupText.outlineColor = new Color(0.2f, 0.08f, 0.02f, 1f);
        popupText.outlineWidth = 0.15f;

        MeshRenderer textRenderer = popupText.GetComponent<MeshRenderer>();
        textRenderer.sortingOrder = 100;

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
            transform.rotation = mainCamera.transform.rotation;

        startPosition = position + Vector3.up * 0.2f;
        transform.position = startPosition;
        startColor = popupText.color;
        riseDistance = rise;
        lifetime = Mathf.Max(duration, 0.01f);
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsed / lifetime);
        float easedProgress = progress * progress * (3f - 2f * progress);

        transform.position = startPosition + Vector3.up * (riseDistance * easedProgress);

        float fadeProgress = Mathf.InverseLerp(0.45f, 1f, progress);
        Color color = startColor;
        color.a = 1f - fadeProgress;
        popupText.color = color;

        if (elapsed >= lifetime)
            Destroy(gameObject);
    }
}
