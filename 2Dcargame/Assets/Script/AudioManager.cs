using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource backgroundSource;
    public AudioSource engineSource;
    public AudioSource sfxSource;

    [Header("SFX Clips")]
    public AudioClip crashClip;
    public AudioClip coinClip;
    public AudioClip nearMissClip;

    [Header("Volumes")]
    public float bgNormalVolume = 0.6f;
    public float bgLowVolume = 0.25f;
    public float engineVolume = 0.4f;

    [Header("Engine Pitch Settings")]
    public float baseEnginePitch = 0.9f;
    public float maxEnginePitch = 1.5f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (backgroundSource != null)
        {
            backgroundSource.volume = bgNormalVolume;
            backgroundSource.loop = true;
            backgroundSource.Play();
        }

        if (engineSource != null)
        {
            engineSource.volume = engineVolume;
            engineSource.pitch = baseEnginePitch;
            engineSource.loop = true;
        }
    }

    // ▶ GAME START
    public void StartEngine()
    {
        if (backgroundSource != null)
            backgroundSource.volume = bgLowVolume;

        if (engineSource != null)
        {
            engineSource.pitch = baseEnginePitch;
            if (!engineSource.isPlaying)
                engineSource.Play();
        }
    }

    // Dynamic pitch based on score/speed
    public void UpdateEnginePitch(float speedFactor)
    {
        if (engineSource != null && engineSource.isPlaying)
        {
            // Smoothly shift pitch higher as speedFactor goes from 0 to 1
            float targetPitch = Mathf.Lerp(baseEnginePitch, maxEnginePitch, speedFactor);
            engineSource.pitch = Mathf.Lerp(engineSource.pitch, targetPitch, Time.deltaTime * 2f);
        }
    }

    // ⏸ PAUSE
    public void PauseEngine()
    {
        if (engineSource != null)
            engineSource.Pause();
    }

    // ▶ RESUME
    public void ResumeEngine()
    {
        if (engineSource != null)
            engineSource.UnPause();
    }

    // 💥 STOP / GAME OVER
    public void StopEngine()
    {
        if (engineSource != null)
            engineSource.Stop();

        if (backgroundSource != null)
            backgroundSource.volume = bgNormalVolume;
    }

    // 💥 SFX: Crash
    public void PlayCrashSFX()
    {
        if (sfxSource != null && crashClip != null)
            sfxSource.PlayOneShot(crashClip);
    }

    // 🪙 SFX: Coin
    public void PlayCoinSFX()
    {
        if (sfxSource != null && coinClip != null)
            sfxSource.PlayOneShot(coinClip);
    }

    // 💨 SFX: Near-Miss Whoosh
    public void PlayNearMissSFX()
    {
        if (sfxSource != null && nearMissClip != null)
            sfxSource.PlayOneShot(nearMissClip);
    }
}