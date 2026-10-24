using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private CinemachineCameraOffset cameraOffset;
    private Transform shakenTransform;
    private Vector3 initialPosition;
    private Vector3 initialOffset;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        Instance = this;

        CinemachineCamera cinemachineCamera = GetComponent<CinemachineCamera>();
        if (cinemachineCamera == null)
        {
            cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
        }

        if (cinemachineCamera != null)
        {
            cameraOffset = cinemachineCamera.GetComponent<CinemachineCameraOffset>();
            if (cameraOffset == null)
            {
                cameraOffset = cinemachineCamera.gameObject.AddComponent<CinemachineCameraOffset>();
            }

            initialOffset = cameraOffset.Offset;
        }
        else
        {
            shakenTransform = transform;
            initialPosition = shakenTransform.localPosition;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Shake(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(ShakeRoutine(Mathf.Max(0f, duration), Mathf.Max(0f, magnitude)));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 offset = new Vector3(
                Random.Range(-magnitude, magnitude),
                Random.Range(-magnitude, magnitude),
                0f);

            if (cameraOffset != null)
            {
                cameraOffset.Offset = initialOffset + offset;
            }
            else
            {
                shakenTransform.localPosition = initialPosition + offset;
            }

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (cameraOffset != null)
        {
            cameraOffset.Offset = initialOffset;
        }
        else
        {
            shakenTransform.localPosition = initialPosition;
        }

        shakeCoroutine = null;
    }
}
