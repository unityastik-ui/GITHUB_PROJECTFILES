using System.Collections;
using UnityEngine;

public class ExplosionSpawner : MonoBehaviour
{
    public static void Spawn(Vector3 position)
    {
        GameObject explosionObject = new GameObject("Procedural Explosion");
        explosionObject.transform.position = position;

        ParticleSystem particles = explosionObject.AddComponent<ParticleSystem>();
        ParticleSystemRenderer particleRenderer = explosionObject.GetComponent<ParticleSystemRenderer>();
        Shader particleShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (particleShader == null)
        {
            particleShader = Shader.Find("Particles/Standard Unlit");
        }

        if (particleShader == null)
        {
            Debug.LogError("Could not find a supported particle shader for the procedural explosion.");
            Destroy(explosionObject);
            return;
        }

        Material particleMaterial = new Material(particleShader);
        particleMaterial.name = "Procedural Explosion Material";
        particleRenderer.material = particleMaterial;

        ParticleSystem.MainModule main = particles.main;
        main.duration = 1f;
        main.loop = false;
        main.useUnscaledTime = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.5f, 1f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(5f, 9f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.5f);
        main.startColor = Color.white;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, 40)
        });

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.15f;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(1f, 0.95f, 0.2f), 0f),
                new GradientColorKey(new Color(1f, 0.35f, 0.05f), 0.3f),
                new GradientColorKey(new Color(0.55f, 0.08f, 0.04f), 0.65f),
                new GradientColorKey(new Color(0.35f, 0.35f, 0.35f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.9f, 0.55f),
                new GradientAlphaKey(0f, 1f)
            });
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        particles.Play();
        explosionObject.AddComponent<ExplosionSpawner>().StartCoroutine(
            DestroyAfterRealtimeDelay(explosionObject, 2f));
    }

    private static IEnumerator DestroyAfterRealtimeDelay(GameObject explosionObject, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Destroy(explosionObject);
    }
}
