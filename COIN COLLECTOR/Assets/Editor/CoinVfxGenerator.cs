using UnityEditor;
using UnityEngine;

public static class CoinVfxGenerator
{
    private const string PrefabPath = "Assets/04Prefabs/Coins/CoinPickupVFX.prefab";

    [MenuItem("Tools/Generate Coin Pickup VFX")]
    private static void GenerateCoinPickupVfx()
    {
        if (!AssetDatabase.IsValidFolder("Assets/04Prefabs/Coins"))
        {
            EditorUtility.DisplayDialog(
                "Coin Pickup VFX",
                "The folder Assets/04Prefabs/Coins was not found.",
                "OK");
            return;
        }

        if (AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null
            && !EditorUtility.DisplayDialog(
                "Overwrite Coin Pickup VFX?",
                "CoinPickupVFX.prefab already exists. Replace it?",
                "Replace",
                "Cancel"))
        {
            return;
        }

        Material particleMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Particle.mat");
        if (particleMaterial == null)
        {
            EditorUtility.DisplayDialog(
                "Coin Pickup VFX",
                "Unity's built-in Default-Particle material could not be found.",
                "OK");
            return;
        }

        GameObject effectObject = new GameObject("CoinPickupVFX");
        try
        {
            ParticleSystem particleSystem = effectObject.AddComponent<ParticleSystem>();
            ConfigureParticleSystem(particleSystem);

            ParticleSystemRenderer particleRenderer = effectObject.GetComponent<ParticleSystemRenderer>();
            particleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
            particleRenderer.material = particleMaterial;

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(effectObject, PrefabPath);
            if (prefab == null)
            {
                EditorUtility.DisplayDialog(
                    "Coin Pickup VFX",
                    "Unity could not save the pickup effect prefab.",
                    "OK");
                return;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
            Debug.Log($"Generated coin pickup VFX prefab at {PrefabPath}.", prefab);
        }
        finally
        {
            Object.DestroyImmediate(effectObject);
        }
    }

    private static void ConfigureParticleSystem(ParticleSystem particleSystem)
    {
        ParticleSystem.MainModule main = particleSystem.main;
        main.duration = 0.5f;
        main.loop = false;
        main.playOnAwake = true;
        main.stopAction = ParticleSystemStopAction.Destroy;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.6f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 6f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.25f);
        ColorUtility.TryParseHtmlString("#FFD700", out Color gold);
        main.startColor = gold;

        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, (short)15, (short)25)
        });

        ParticleSystem.ShapeModule shape = particleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.2f;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient fadeGradient = new Gradient();
        fadeGradient.SetKeys(
            new[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 0.65f),
                new GradientAlphaKey(0f, 1f)
            });
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(fadeGradient);
    }
}
