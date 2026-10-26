using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Wave Configuration")]
    [SerializeField] private WaveData[] waves;
    [SerializeField] private EnemySpawner[] spawnLanes;
    [SerializeField, Min(0)] private int additionalEnemiesPerWave = 2;
    [SerializeField, Range(0f, 1f)] private float healthIncreasePerWave = 0.12f;
    [SerializeField, Range(0f, 1f)] private float speedIncreasePerWave = 0.05f;

    [Header("References")]
    [SerializeField] private CastleHealth castleHealth;
    [SerializeField] private UIManager uiManager;

    private readonly HashSet<EnemyHealth> activeEnemies = new HashSet<EnemyHealth>();

    public int CurrentWave { get; private set; }
    public int RemainingToSpawn { get; private set; }
    public int ActiveEnemyCount => activeEnemies.Count;

    private void Awake()
    {
        if (spawnLanes == null || spawnLanes.Length == 0)
            spawnLanes = FindObjectsOfType<EnemySpawner>();

        if (castleHealth == null)
            castleHealth = FindObjectOfType<CastleHealth>();

        if (uiManager == null)
            uiManager = UIManager.Instance != null ? UIManager.Instance : FindObjectOfType<UIManager>();
    }

    private void Start()
    {
        if (!ValidateConfiguration())
            return;

        StartCoroutine(RunWaves());
    }

    public void RegisterEnemy(EnemyHealth enemy)
    {
        if (enemy != null)
            activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(EnemyHealth enemy)
    {
        if (enemy != null)
            activeEnemies.Remove(enemy);
    }

    private bool ValidateConfiguration()
    {
        if (waves == null || waves.Length == 0)
        {
            Debug.LogError("[WaveSpawner] Configure at least one wave.", this);
            return false;
        }

        if (spawnLanes == null || spawnLanes.Length == 0)
        {
            Debug.LogError("[WaveSpawner] Assign at least one active EnemySpawner lane.", this);
            return false;
        }

        if (castleHealth == null)
        {
            Debug.LogError("[WaveSpawner] Assign the CastleHealth component.", this);
            return false;
        }

        if (uiManager == null)
        {
            Debug.LogError("[WaveSpawner] Could not find UIManager in the gameplay scene.", this);
            return false;
        }

        for (int i = 0; i < spawnLanes.Length; i++)
        {
            if (spawnLanes[i] == null || !spawnLanes[i].HasValidPath)
            {
                Debug.LogError($"[WaveSpawner] Spawn lane at index {i} is missing a valid path.", this);
                return false;
            }
        }

        for (int i = 0; i < waves.Length; i++)
        {
            WaveData wave = waves[i];
            if (wave == null || wave.enemyPrefabs == null || wave.enemyPrefabs.Length == 0)
            {
                Debug.LogError($"[WaveSpawner] Wave at index {i} needs at least one enemy prefab.", this);
                return false;
            }

            if (wave.enemyCount < 1 || wave.spawnInterval <= 0f)
            {
                Debug.LogError($"[WaveSpawner] Wave at index {i} must have a positive count and spawn interval.", this);
                return false;
            }

            foreach (GameObject prefab in wave.enemyPrefabs)
            {
                if (prefab == null || prefab.GetComponent<EnemyMovement>() == null ||
                    prefab.GetComponent<EnemyHealth>() == null)
                {
                    Debug.LogError(
                        $"[WaveSpawner] Every enemy prefab in wave {wave.waveNumber} must have EnemyMovement and EnemyHealth.",
                        this);
                    return false;
                }
            }
        }

        return true;
    }

    private IEnumerator RunWaves()
    {
        for (int i = 0; i < waves.Length; i++)
        {
            WaveData wave = waves[i];
            CurrentWave = wave.waveNumber;
            int waveOffset = Mathf.Max(0, wave.waveNumber - 1);
            int waveCount = wave.enemyCount + waveOffset * additionalEnemiesPerWave;
            float healthMultiplier = 1f + waveOffset * healthIncreasePerWave;
            float speedMultiplier = 1f + waveOffset * speedIncreasePerWave;

            RemainingToSpawn = waveCount;
            Debug.Log($"[WaveSpawner] Starting wave {CurrentWave}: {waveCount} enemies.", this);

            for (int enemyIndex = 0; enemyIndex < waveCount; enemyIndex++)
            {
                EnemySpawner lane = spawnLanes[Random.Range(0, spawnLanes.Length)];
                GameObject enemyPrefab = wave.enemyPrefabs[Random.Range(0, wave.enemyPrefabs.Length)];
                EnemyHealth spawnedEnemy = lane.SpawnEnemy(
                    enemyPrefab,
                    healthMultiplier,
                    speedMultiplier,
                    castleHealth,
                    this);

                if (spawnedEnemy == null)
                {
                    Debug.LogError($"[WaveSpawner] Failed to spawn an enemy for wave {CurrentWave}.", this);
                    yield break;
                }

                RemainingToSpawn--;
                if (enemyIndex + 1 < waveCount)
                    yield return new WaitForSeconds(wave.spawnInterval);
            }

            while (activeEnemies.Count > 0)
                yield return null;

            if (i + 1 < waves.Length)
                yield return new WaitForSeconds(wave.delayBeforeNextWave);
        }

        uiManager.ToggleVictoryPanel();
    }
}
