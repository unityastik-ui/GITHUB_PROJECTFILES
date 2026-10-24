using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefab;

    [Header("Spawn Settings")]
    public float spawnInterval = 2f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        int score = ScoreManager.instance.score;

        // Spawn time progression
        if (score < 15)
        {
            spawnInterval = 2f;
        }
        else if (score < 50)
        {
            spawnInterval = 1.8f;
        }
        else if (score < 75)
        {
            spawnInterval = 1.6f;
        }
        else if (score <= 100)
        {
            spawnInterval = 1.4f;
        }
        else
        {
            spawnInterval = 1.2f;
        }

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        int score = ScoreManager.instance.score;

        // SCORE 0 - 14
        if (score < 15)
        {
            SpawnSingleEnemy();
        }

        // SCORE 15 - 49
        else if (score < 50)
        {
            if (Random.value < 0.5f)
                SpawnSingleEnemy();
            else
                SpawnDoubleEnemy();
        }

        // SCORE 50 - 74
        else if (score < 75)
        {
            if (Random.value < 0.5f)
                SpawnSingleEnemy();
            else
                SpawnDoubleEnemy();
        }

        // SCORE 75 - 100
        else if (score <= 100)
        {
            SpawnDoubleEnemy();
        }

        // SCORE 101+
        else
        {
            // 50% Double
            // 50% Triple
            if (Random.value < 0.5f)
                SpawnDoubleEnemy();
            else
                SpawnTripleEnemy();
        }
    }

    private void SpawnSingleEnemy()
    {
        int randomIndex = Random.Range(0, enemyPrefab.Length);

        Instantiate(
            enemyPrefab[randomIndex],
            transform.position,
            Quaternion.identity
        );
    }

    private void SpawnDoubleEnemy()
    {
        // Same enemy paired together
        int randomIndex = Random.Range(0, enemyPrefab.Length);

        Instantiate(
            enemyPrefab[randomIndex],
            transform.position,
            Quaternion.identity
        );

        Instantiate(
            enemyPrefab[randomIndex],
            transform.position + Vector3.right * 1.5f,
            Quaternion.identity
        );
    }

    private void SpawnTripleEnemy()
    {
        // Same enemy used three times
        int randomIndex = Random.Range(0, enemyPrefab.Length);

        Instantiate(
            enemyPrefab[randomIndex],
            transform.position,
            Quaternion.identity
        );

        Instantiate(
            enemyPrefab[randomIndex],
            transform.position + Vector3.right * 1.5f,
            Quaternion.identity
        );

        Instantiate(
            enemyPrefab[randomIndex],
            transform.position + Vector3.right * 3f,
            Quaternion.identity
        );
    }
}