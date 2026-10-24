using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{
    [Header("Motion")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float lifetime = 0.8f;

    [Header("Appearance")]
    [SerializeField] private TextMeshPro textMesh;

    private Color textColor;
    private float timer;

    private void Awake()
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();

        if (textMesh != null)
            textColor = textMesh.color;
    }

    private void LateUpdate()
    {
        // 1. Face camera directly (Billboard)
        if (Camera.main != null)
        {
            transform.forward = Camera.main.transform.forward;
        }

        // 2. Float straight up
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // 3. Fade out text over lifetime
        timer += Time.deltaTime;
        if (textMesh != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
            textMesh.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
        }

        // 4. Destroy when lifetime expires
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}