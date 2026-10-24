using UnityEngine;

[DefaultExecutionOrder(-100)]
public class SceneEnhancer : MonoBehaviour
{
    [Header("Shadow Quality")]
    [Tooltip("Enables both main-light and additional-light shadows through Unity's quality settings.")]
    public bool enableSoftShadows = true;

    [Tooltip("Shadow map resolution applied to the active quality level.")]
    public ShadowResolution shadowResolution = ShadowResolution.High;

    [Tooltip("Maximum distance at which real-time shadows are rendered.")]
    [Min(0f)]
    public float shadowDistance = 80f;

    [Header("Ambient Lighting")]
    [Tooltip("Sky color used by the scene's ambient lighting.")]
    public Color ambientSkyColor = new Color(0.55f, 0.65f, 0.8f, 1f);

    [Tooltip("Horizon color used by the scene's ambient lighting.")]
    public Color ambientEquatorColor = new Color(0.45f, 0.45f, 0.45f, 1f);

    [Tooltip("Ground color used by the scene's ambient lighting.")]
    public Color ambientGroundColor = new Color(0.22f, 0.2f, 0.18f, 1f);

    [Header("Camera")]
    [Tooltip("Background color used when the scene has no skybox material.")]
    public Color cameraBackgroundColor = new Color(0.16f, 0.22f, 0.3f, 1f);

    private void Awake()
    {
        ApplyLighting();
        ApplyCameraBackground();
    }

    private void ApplyLighting()
    {
        QualitySettings.shadows = enableSoftShadows ? ShadowQuality.All : ShadowQuality.HardOnly;
        QualitySettings.shadowResolution = shadowResolution;
        QualitySettings.shadowDistance = shadowDistance;
        QualitySettings.shadowProjection = ShadowProjection.StableFit;
        QualitySettings.shadowCascades = 4;

        Light[] sceneLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light sceneLight in sceneLights)
        {
            if (sceneLight.shadows != LightShadows.None)
                sceneLight.shadows = enableSoftShadows ? LightShadows.Soft : LightShadows.Hard;
        }

        RenderSettings.ambientSkyColor = ambientSkyColor;
        RenderSettings.ambientEquatorColor = ambientEquatorColor;
        RenderSettings.ambientGroundColor = ambientGroundColor;
        RenderSettings.ambientMode = RenderSettings.skybox != null
            ? UnityEngine.Rendering.AmbientMode.Skybox
            : UnityEngine.Rendering.AmbientMode.Trilight;
    }

    private void ApplyCameraBackground()
    {
        if (RenderSettings.skybox != null)
            return;

        Camera sceneCamera = Camera.main;
        if (sceneCamera != null)
        {
            sceneCamera.clearFlags = CameraClearFlags.SolidColor;
            sceneCamera.backgroundColor = cameraBackgroundColor;
        }
    }
}
