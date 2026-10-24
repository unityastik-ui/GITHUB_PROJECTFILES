using System.Collections;
using UnityEngine;

public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance;

    [Header("Hit Stop Settings")]
    public float hitStopDuration = 0.15f;

    private void Awake()
    {
        Instance = this;
    }

    public void StopGameTemperory()
    {
        StartCoroutine(HitStop());
    }
    private IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f;
    }
}
