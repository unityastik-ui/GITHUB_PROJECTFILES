using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource bgmSource;
    public AudioSource engineSource;
    public AudioSource sfxSource;

    public AudioClip bgmClip;
    public AudioClip engineClip;
    public AudioClip coinClip;
    public AudioClip crashClip;
    public AudioClip gameOverClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ConfigureAudioSources();
        ValidateAudioSetup();
    }

    private void Start()
    {
        PlayBGM();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGM();
    }

    public void PlayBGM()
    {
        if (bgmSource == null || bgmClip == null)
        {
            return;
        }

        if (bgmSource.clip != bgmClip)
        {
            bgmSource.clip = bgmClip;
        }

        bgmSource.loop = true;
        if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }

    public void StartEngine()
    {
        if (engineSource == null || engineClip == null)
        {
            return;
        }

        engineSource.clip = engineClip;
        engineSource.loop = true;
        if (!engineSource.isPlaying)
        {
            engineSource.Play();
        }
    }

    public void StopEngine()
    {
        if (engineSource != null)
        {
            engineSource.Stop();
        }
    }

    public void SetEnginePitch(float pitch)
    {
        if (engineSource != null)
        {
            engineSource.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
        }
    }

    public void PlayCoin()
    {
        if (sfxSource == null || coinClip == null)
        {
            return;
        }

        float originalPitch = sfxSource.pitch;
        sfxSource.pitch = Random.Range(0.95f, 1.05f);
        sfxSource.PlayOneShot(coinClip);
        sfxSource.pitch = originalPitch;
    }

    public void PlayCrash()
    {
        if (sfxSource != null && crashClip != null)
        {
            sfxSource.PlayOneShot(crashClip);
        }
    }

    public void PlayGameOver()
    {
        StopEngine();
        StopBGM();

        if (sfxSource != null && gameOverClip != null)
        {
            sfxSource.PlayOneShot(gameOverClip);
        }
    }

    private void ConfigureAudioSources()
    {
        ConfigureAudioSource(bgmSource, true);
        ConfigureAudioSource(engineSource, true);
        ConfigureAudioSource(sfxSource, false);
    }

    private void ConfigureAudioSource(AudioSource source, bool loop)
    {
        if (source == null)
        {
            return;
        }

        source.enabled = true;
        source.playOnAwake = false;
        source.loop = loop;
        source.spatialBlend = 0f;
    }

    private void ValidateAudioSetup()
    {
        if (bgmSource == null)
        {
            Debug.LogError("AudioManager: Assign a BGM AudioSource.", this);
        }
        if (engineSource == null)
        {
            Debug.LogError("AudioManager: Assign an engine AudioSource.", this);
        }
        if (sfxSource == null)
        {
            Debug.LogError("AudioManager: Assign an SFX AudioSource.", this);
        }
        if (bgmClip == null)
        {
            Debug.LogError("AudioManager: Assign the background music clip.", this);
        }
        if (engineClip == null)
        {
            Debug.LogError("AudioManager: Assign the plane engine clip.", this);
        }
        if (coinClip == null)
        {
            Debug.LogError("AudioManager: Assign the coin pickup clip.", this);
        }
        if (crashClip == null)
        {
            Debug.LogError("AudioManager: Assign the crash clip.", this);
        }
        if (gameOverClip == null)
        {
            Debug.LogError("AudioManager: Assign the game over clip.", this);
        }

        if (bgmSource != null &&
            (bgmSource == engineSource || bgmSource == sfxSource || engineSource == sfxSource))
        {
            Debug.LogError("AudioManager: BGM, engine, and SFX must use separate AudioSources.", this);
        }
    }
}