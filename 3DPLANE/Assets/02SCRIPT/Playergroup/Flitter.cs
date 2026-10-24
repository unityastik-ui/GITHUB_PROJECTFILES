using System.Collections;
using UnityEngine;

public class Flitter : MonoBehaviour
{
    public static Flitter Instance;

    private Vector3 initialPosition;
    private bool isShaking;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        if (isShaking)
        {
            return;
        }

        // Normal ambient float when not shaking
        float noise = (Mathf.PerlinNoise(Time.time * 10f, 0f) - 0.5f) * 0.1f;
        transform.localPosition = initialPosition + new Vector3(0f, noise, 0f);
    }

    // Call this to trigger a shake
    public void Shake(float duration = 0.3f, float magnitude = 0.5f)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        isShaking = true;

        while (elapsed < duration)
        {
            // Pick a random offset in X and Y
            float xOffset = Random.Range(-1f, 1f) * magnitude;
            float yOffset = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = initialPosition + new Vector3(xOffset, yOffset, 0f);

            // Use unscaledDeltaTime so shake works even if time is slowed down
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localPosition = initialPosition;
        isShaking = false;
    }
}