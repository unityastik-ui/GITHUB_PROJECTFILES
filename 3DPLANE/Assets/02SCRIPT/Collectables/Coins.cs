using System.Collections;
using UnityEngine;

public class Coins : MonoBehaviour
{
    public GameObject coinPickupEffectPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("coin touched");
            CoinManager.Instance.AddCoin();

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCoin();
            }

            if (coinPickupEffectPrefab != null)
            {
                Instantiate(coinPickupEffectPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                SpawnProceduralPickupEffect(transform.position);
            }

            Destroy(gameObject);
        }
    }

    void SpawnProceduralPickupEffect(Vector3 pos)
    {
        // 1. Create a new GameObject for the effect
        GameObject effectObj = new GameObject("CoinSparkleEffect");
        effectObj.transform.position = pos;

        // 2. Add the ParticleSystem component
        ParticleSystem ps = effectObj.AddComponent<ParticleSystem>();

        // FIX 1: Stop immediately so Unity lets us edit duration and settings without error
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // 3. Configure Main Settings (Gold color and speed)
        var main = ps.main;
        main.duration = 0.4f;
        main.loop = false;
        main.startLifetime = 0.4f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 7f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.25f);
        main.startColor = new Color(1f, 0.85f, 0.1f); // Bright Gold ⭐

        // 4. Burst emission
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 20) });

        // 5. Sphere shape so sparkles fly outward
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.2f;

        // FIX 2: Assign a standard unlit shader to fix the pink color
        ParticleSystemRenderer psRenderer = effectObj.GetComponent<ParticleSystemRenderer>();
        Shader unlitShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (unlitShader == null) unlitShader = Shader.Find("Particles/Standard Unlit");
        if (unlitShader == null) unlitShader = Shader.Find("Mobile/Particles/Additive");

        if (unlitShader != null)
        {
            psRenderer.material = new Material(unlitShader);
        }

        // 6. Play after all settings are applied
        ps.Play();

        // 7. Clean up the object after 1 second
        Destroy(effectObj, 1f);
    }
}

public class CoinPickupEffectLifetime : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(1f);
        Destroy(gameObject);
    }
}
