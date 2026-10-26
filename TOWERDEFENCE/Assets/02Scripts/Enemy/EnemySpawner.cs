using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public float spawnDelay = 2f;
    public EnemyPath path;
    [SerializeField] private bool spawnIndependently;

    public bool HasValidPath => path != null && path.points != null && path.points.Length > 0;

    private void Start()
    {
        if (!spawnIndependently)
            return;

        if (enemyPrefab == null || !HasValidPath)
        {
            Debug.LogError("[EnemySpawner] Independent spawning requires an enemy prefab and a valid path.", this);
            return;
        }

        if (enemyPrefab.GetComponent<EnemyHealth>() == null ||
            enemyPrefab.GetComponent<EnemyMovement>() == null)
        {
            Debug.LogError("[EnemySpawner] Enemy prefab needs EnemyHealth and EnemyMovement.", this);
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnEnemy(enemyPrefab, 1f, 1f, null, null);
            yield return new WaitForSeconds(Mathf.Max(0.05f, spawnDelay));
        }
    }

    public EnemyHealth SpawnEnemy(
        GameObject prefab,
        float healthMultiplier,
        float speedMultiplier,
        CastleHealth castle,
        WaveSpawner waveSpawner)
    {
        if (prefab == null || !HasValidPath)
        {
            Debug.LogError("[EnemySpawner] Cannot spawn without an enemy prefab and a valid path.", this);
            return null;
        }

        EnemyMovement movementPrefab = prefab.GetComponent<EnemyMovement>();
        EnemyHealth healthPrefab = prefab.GetComponent<EnemyHealth>();
        if (movementPrefab == null || healthPrefab == null)
        {
            Debug.LogError("[EnemySpawner] Enemy prefab needs EnemyMovement and EnemyHealth.", prefab);
            return null;
        }

        Transform origin = spawnPoint != null ? spawnPoint : transform;
        GameObject enemyObject = Instantiate(prefab, origin.position, Quaternion.identity);
        EnemyMovement movement = enemyObject.GetComponent<EnemyMovement>();
        EnemyHealth health = enemyObject.GetComponent<EnemyHealth>();

        health.Configure(healthMultiplier, waveSpawner);
        movement.ConfigurePath(path.points, speedMultiplier, castle, health);
        return health;
    }
}