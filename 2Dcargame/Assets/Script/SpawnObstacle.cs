using System.Collections;
using UnityEngine;

public class SpawnObstacle : MonoBehaviour
{
    [Header("Enemy Prefabs & Lanes")]
    public GameObject[] carPrefabs;
    public float[] lanes = new float[] { -1.1f, -0.37f, 0.37f, 1.1f }; // Road lane X coordinates

    [Header("Coin Spawning")]
    public GameObject coinPrefab;
    [Range(0f, 1f)] public float coinSpawnChance = 0.5f; // 50% chance per wave

    [Header("Timing")]
    public float minWaitTime = 1.2f;
    public float maxWaitTime = 2.2f;
    public float spawnY = 7f; // Above screen view

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            SpawnWave();
        }
    }

    void SpawnWave()
    {
        if (lanes.Length == 0) return;

        // 1. Pick a random lane for the enemy car
        int enemyLaneIndex = Random.Range(0, lanes.Length);
        float enemyX = lanes[enemyLaneIndex];

        if (carPrefabs != null && carPrefabs.Length > 0)
        {
            GameObject randomCar = carPrefabs[Random.Range(0, carPrefabs.Length)];
            Instantiate(randomCar, new Vector3(enemyX, spawnY, 0f), Quaternion.identity);
        }

        // 2. Try spawning a coin in a DIFFERENT lane (so it never overlaps the car)
        if (coinPrefab != null && Random.value < coinSpawnChance)
        {
            int coinLaneIndex = Random.Range(0, lanes.Length);

            // If it picked the same lane, shift it to an adjacent lane
            if (coinLaneIndex == enemyLaneIndex)
            {
                coinLaneIndex = (coinLaneIndex + 1) % lanes.Length;
            }

            float coinX = lanes[coinLaneIndex];
            Instantiate(coinPrefab, new Vector3(coinX, spawnY, 0f), Quaternion.identity);
        }
    }
}