using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public static class VisualPolishSetup
{
    private const string GameplayScenePath = "Assets/05Scenes/03Gameplay.unity";
    private const string PlayerPrefabPath = "Assets/Resources/Player01.prefab";
    private const string MaterialsFolder = "Assets/00Materiall/VisualPolish";
    private const string CoinMaterialPath = "Assets/00Materiall/Coin_Mat.mat";
    private const string VolumeProfilePath = "Assets/05Scenes/03Gameplay/PostProcessVolume Profile.asset";

    private static readonly Color FloorColor = HexColor("#202B3A");
    private static readonly Color WallColor = HexColor("#3F7184");
    private static readonly Color ObstacleColor = HexColor("#56667A");
    private static readonly Color PlayerOneColor = HexColor("#00D9FF");
    private static readonly Color PlayerTwoColor = HexColor("#FF654A");

    [MenuItem("Tools/Visual Polish/Apply to 03Gameplay")]
    private static void ApplyToGameplay()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != GameplayScenePath)
        {
            EditorUtility.DisplayDialog(
                "Visual Polish Setup",
                "Open Assets/05Scenes/03Gameplay.unity and make it the active scene before applying this setup.",
                "OK");
            return;
        }

        if (!EditorUtility.DisplayDialog(
                "Apply Visual Polish",
                "This updates the active gameplay scene, its post-processing profile, the Player01 prefab, and generated materials. The gameplay scene will be saved.",
                "Apply",
                "Cancel"))
        {
            return;
        }

        GameObject ground = FindSceneObject(scene, "Ground");
        GameObject[] walls =
        {
            FindSceneObject(scene, "WallTop"),
            FindSceneObject(scene, "WallBottom"),
            FindSceneObject(scene, "WallLeft"),
            FindSceneObject(scene, "WallRight")
        };
        GameObject[] obstacles =
        {
            FindSceneObject(scene, "Obstacle01"),
            FindSceneObject(scene, "Obstacle02"),
            FindSceneObject(scene, "Obstacle03"),
            FindSceneObject(scene, "Obstacle04")
        };
        GameObject volumeObject = FindSceneObject(scene, "PostProcessVolume");
        GameObject lightObject = FindSceneObject(scene, "Directional Light");

        if (ground == null || volumeObject == null || lightObject == null ||
            Array.Exists(walls, item => item == null) ||
            Array.Exists(obstacles, item => item == null))
        {
            Debug.LogError("Visual Polish Setup stopped: expected Ground, four walls, four obstacles, PostProcessVolume, and Directional Light in 03Gameplay.");
            return;
        }

        Volume volume = volumeObject.GetComponent<Volume>();
        Light mainLight = lightObject.GetComponent<Light>();
        if (volume == null || mainLight == null)
        {
            Debug.LogError("Visual Polish Setup stopped: PostProcessVolume needs a Volume component and Directional Light needs a Light component.");
            return;
        }

        EnsureMaterialsFolder();
        Material floorMaterial = CreateOrUpdateMaterial("Arena_Floor.mat", FloorColor, 0.12f, 0.25f);
        Material wallMaterial = CreateOrUpdateMaterial("Arena_Walls.mat", WallColor, 0.18f, 0.38f);
        Material obstacleMaterial = CreateOrUpdateMaterial("Arena_Obstacles.mat", ObstacleColor, 0.12f, 0.3f);
        Material playerOneMaterial = CreateOrUpdateMaterial("Player_01_Cyan.mat", PlayerOneColor, 0.08f, 0.4f);
        Material playerTwoMaterial = CreateOrUpdateMaterial("Player_02_Coral.mat", PlayerTwoColor, 0.08f, 0.4f);

        SetRendererMaterial(ground, floorMaterial);
        foreach (GameObject wall in walls)
            SetRendererMaterial(wall, wallMaterial);
        foreach (GameObject obstacle in obstacles)
            SetRendererMaterial(obstacle, obstacleMaterial);

        ConfigurePlayerPrefab(playerOneMaterial, playerTwoMaterial);
        ConfigureCoinEmission();
        ConfigureVolume(volume);
        ConfigureLight(mainLight);

        EditorSceneManager.MarkSceneDirty(scene);
        AssetDatabase.SaveAssets();
        if (!EditorSceneManager.SaveScene(scene))
        {
            Debug.LogError("Visual Polish Setup applied assets but could not save the active gameplay scene.");
            return;
        }

        Debug.Log("Visual Polish Setup applied to 03Gameplay, Player01, and the gameplay volume profile.");
    }

    private static GameObject FindSceneObject(Scene scene, string objectName)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == objectName)
                    return child.gameObject;
            }
        }

        return null;
    }

    private static void SetRendererMaterial(GameObject target, Material material)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
        {
            Debug.LogError("Visual Polish Setup found no Renderer on " + target.name + " or its children.");
            return;
        }

        foreach (Renderer renderer in renderers)
        {
            Undo.RecordObject(renderer, "Apply Visual Polish Materials");
            Material[] materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
                materials[i] = material;
            renderer.sharedMaterials = materials;
            EditorUtility.SetDirty(renderer);
        }
    }

    private static void ConfigurePlayerPrefab(Material playerOneMaterial, Material playerTwoMaterial)
    {
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(PlayerPrefabPath);
        try
        {
            Playercolor playerColor = prefabRoot.GetComponentInChildren<Playercolor>(true);
            if (playerColor == null)
                throw new InvalidOperationException("Player01 prefab does not contain a Playercolor component.");

            SerializedObject serializedPlayer = new SerializedObject(playerColor);
            serializedPlayer.Update();
            SetMaterialProperty(serializedPlayer, "blueBody", playerOneMaterial);
            SetMaterialProperty(serializedPlayer, "blueArms", playerOneMaterial);
            SetMaterialProperty(serializedPlayer, "blueLegs", playerOneMaterial);
            SetMaterialProperty(serializedPlayer, "redBody", playerTwoMaterial);
            SetMaterialProperty(serializedPlayer, "redArms", playerTwoMaterial);
            SetMaterialProperty(serializedPlayer, "redLegs", playerTwoMaterial);
            serializedPlayer.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(prefabRoot, PlayerPrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }
    }

    private static void SetMaterialProperty(SerializedObject serializedObject, string propertyName, Material material)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property == null)
            throw new InvalidOperationException("Playercolor is missing the serialized material field '" + propertyName + "'.");

        property.objectReferenceValue = material;
    }

    private static void ConfigureVolume(Volume volume)
    {
        VolumeProfile profile = volume.sharedProfile;
        if (profile == null)
        {
            profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(VolumeProfilePath);
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                profile.name = "Gameplay Visual Polish Profile";
                AssetDatabase.CreateAsset(profile, VolumeProfilePath);
            }

            volume.sharedProfile = profile;
        }

        Undo.RecordObject(volume, "Configure Gameplay Volume");
        volume.isGlobal = true;
        volume.weight = 1f;
        volume.priority = 10f;

        Bloom bloom = GetOrAddComponent<Bloom>(profile);
        bloom.active = true;
        bloom.threshold.overrideState = true;
        bloom.threshold.value = 1f;
        bloom.intensity.overrideState = true;
        bloom.intensity.value = 1.5f;

        Vignette vignette = GetOrAddComponent<Vignette>(profile);
        vignette.active = true;
        vignette.intensity.overrideState = true;
        vignette.intensity.value = 0.3f;
        vignette.smoothness.overrideState = true;
        vignette.smoothness.value = 0.4f;

        ColorAdjustments colorAdjustments = GetOrAddComponent<ColorAdjustments>(profile);
        colorAdjustments.active = true;
        colorAdjustments.postExposure.overrideState = true;
        colorAdjustments.postExposure.value = 0.1f;
        colorAdjustments.contrast.overrideState = true;
        colorAdjustments.contrast.value = 12f;
        colorAdjustments.saturation.overrideState = true;
        colorAdjustments.saturation.value = 10f;

        EditorUtility.SetDirty(profile);
        EditorUtility.SetDirty(volume);
    }

    private static void ConfigureCoinEmission()
    {
        Material coinMaterial = AssetDatabase.LoadAssetAtPath<Material>(CoinMaterialPath);
        if (coinMaterial == null || !coinMaterial.HasProperty("_EmissionColor"))
            throw new InvalidOperationException("Could not find an emissive Coin_Mat material at " + CoinMaterialPath + ".");

        Undo.RecordObject(coinMaterial, "Add Gold Coin Emission");
        coinMaterial.EnableKeyword("_EMISSION");
        coinMaterial.SetColor("_EmissionColor", new Color(1.7f, 0.85f, 0.08f, 1f));
        EditorUtility.SetDirty(coinMaterial);
    }

    private static T GetOrAddComponent<T>(VolumeProfile profile) where T : VolumeComponent
    {
        if (!profile.TryGet(out T component))
            component = profile.Add<T>(true);

        return component;
    }

    private static void ConfigureLight(Light light)
    {
        Undo.RecordObject(light, "Configure Gameplay Directional Light");
        light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        light.shadows = LightShadows.Soft;
        light.shadowStrength = 0.75f;
        EditorUtility.SetDirty(light);
        EditorUtility.SetDirty(light.transform);
    }

    private static Material CreateOrUpdateMaterial(string fileName, Color color, float metallic, float smoothness)
    {
        string path = MaterialsFolder + "/" + fileName;
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                throw new InvalidOperationException("Could not find the Universal Render Pipeline/Lit shader.");

            material = new Material(shader);
            material.name = System.IO.Path.GetFileNameWithoutExtension(fileName);
            AssetDatabase.CreateAsset(material, path);
        }

        if (material.shader == null || material.shader.name != "Universal Render Pipeline/Lit")
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                throw new InvalidOperationException("Could not find the Universal Render Pipeline/Lit shader.");
            material.shader = shader;
        }

        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color"))
            material.SetColor("_Color", color);
        if (material.HasProperty("_Metallic"))
            material.SetFloat("_Metallic", metallic);
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", smoothness);

        material.enableInstancing = true;
        EditorUtility.SetDirty(material);
        return material;
    }

    private static void EnsureMaterialsFolder()
    {
        if (AssetDatabase.IsValidFolder(MaterialsFolder))
            return;

        if (!AssetDatabase.IsValidFolder("Assets/00Materiall"))
            throw new InvalidOperationException("Expected material folder Assets/00Materiall was not found.");

        AssetDatabase.CreateFolder("Assets/00Materiall", "VisualPolish");
    }

    private static Color HexColor(string html)
    {
        if (!ColorUtility.TryParseHtmlString(html, out Color color))
            throw new InvalidOperationException("Invalid palette color: " + html);

        return color;
    }
}
