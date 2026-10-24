using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightManager : MonoBehaviour
{
    [Header("Environment Light")]
    public Light2D globalLight;
    public float dayIntensity = 1.5f;
    [Min(0f)] public float nightIntensity = 0.1f;
    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.1f, 0.14f, 0.24f, 1f);

    [Header("Headlights")]
    public Light2D leftHeadLight;
    public Light2D rightHeadLight;
    [Min(0f)] public float headLightIntensity = 1.5f;
    [Tooltip("Headlights begin fading in when the global light reaches this intensity.")]
    [Min(0f)] public float headlightsOnAtIntensity = 0.75f;
    [Min(0f)] public float headlightFadeSpeed = 1.5f;

    [Header("Optional High Beams")]
    public Light2D leftHighBeam;
    public Light2D rightHighBeam;
    [Min(0f)] public float highBeamIntensity = 1.8f;

    [Header("Cycle Timings")]
    [Min(0f)] public float dayHoldTime = 30f;
    [Min(0f)] public float nightHoldTime = 45f;
    [Min(0.01f)] public float fadeDuration = 5f;

    enum State
    {
        DayHold,
        FadeToNight,
        NightHold,
        FadeToDay
    }

    State state;
    float timer;
    float startIntensity;
    bool missingGlobalLightLogged;

    void Start()
    {
        if (globalLight == null)
        {
            Debug.LogError("Assign the scene's Global Light 2D to DayNightManager.", this);
            enabled = false;
            return;
        }

        state = State.DayHold;
        globalLight.intensity = dayIntensity;
        globalLight.color = dayColor;
        SetHeadlights(0f);
    }

    void Update()
    {
        if (globalLight == null)
        {
            if (!missingGlobalLightLogged)
            {
                Debug.LogError("The Global Light 2D reference was removed from DayNightManager.", this);
                missingGlobalLightLogged = true;
            }

            return;
        }

        timer += Time.deltaTime;

        switch (state)
        {
            case State.DayHold:
                if (timer >= dayHoldTime)
                    BeginFade(State.FadeToNight);
                break;

            case State.FadeToNight:
                globalLight.intensity = Mathf.Lerp(startIntensity, nightIntensity, FadeProgress());
                if (timer >= fadeDuration)
                {
                    globalLight.intensity = nightIntensity;
                    BeginHold(State.NightHold);
                }
                break;

            case State.NightHold:
                globalLight.intensity = nightIntensity;
                if (timer >= nightHoldTime)
                    BeginFade(State.FadeToDay);
                break;

            case State.FadeToDay:
                globalLight.intensity = Mathf.Lerp(startIntensity, dayIntensity, FadeProgress());
                if (timer >= fadeDuration)
                {
                    globalLight.intensity = dayIntensity;
                    BeginHold(State.DayHold);
                }
                break;
        }

        UpdateLighting();
    }

    void UpdateLighting()
    {
        float darkness = Mathf.InverseLerp(dayIntensity, nightIntensity, globalLight.intensity);
        globalLight.color = Color.Lerp(dayColor, nightColor, darkness);

        float headlightBlend = Mathf.Approximately(headlightsOnAtIntensity, nightIntensity)
            ? (globalLight.intensity <= nightIntensity ? 1f : 0f)
            : Mathf.InverseLerp(headlightsOnAtIntensity, nightIntensity, globalLight.intensity);

        float fadeStep = headlightFadeSpeed * Time.deltaTime;
        FadeLight(leftHeadLight, headLightIntensity * headlightBlend, fadeStep);
        FadeLight(rightHeadLight, headLightIntensity * headlightBlend, fadeStep);
        FadeLight(leftHighBeam, highBeamIntensity * headlightBlend, fadeStep);
        FadeLight(rightHighBeam, highBeamIntensity * headlightBlend, fadeStep);
    }

    float FadeProgress()
    {
        return Mathf.Clamp01(timer / Mathf.Max(0.01f, fadeDuration));
    }

    void BeginFade(State nextState)
    {
        timer = 0f;
        startIntensity = globalLight.intensity;
        state = nextState;
    }

    void BeginHold(State nextState)
    {
        timer = 0f;
        state = nextState;
    }

    void SetHeadlights(float intensity)
    {
        SetLightIntensity(leftHeadLight, intensity);
        SetLightIntensity(rightHeadLight, intensity);
        SetLightIntensity(leftHighBeam, intensity);
        SetLightIntensity(rightHighBeam, intensity);
    }

    static void FadeLight(Light2D light, float target, float step)
    {
        if (light != null)
            light.intensity = Mathf.MoveTowards(light.intensity, target, step);
    }

    static void SetLightIntensity(Light2D light, float intensity)
    {
        if (light != null)
            light.intensity = intensity;
    }
}
