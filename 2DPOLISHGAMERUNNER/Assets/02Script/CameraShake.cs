using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
   public static CameraShake Instance;

    [Header("Shake Settings")]  
    public float shakeDuration = 0.15f;
    public float shakeStrength = 0.15f;

    Vector3 originalPosition;

    public bool IsShaking { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void Shake()
    {
        Debug.Log("CAMERA SHAKE CALLED!");
        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        IsShaking = true;
        float elapsed = 0.0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeStrength;
            float y = Random.Range(-1f, 1f) * shakeStrength;
            transform.localPosition = new Vector3(x, y, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
        IsShaking = false;
    }
}
