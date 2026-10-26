using System;
using UnityEngine;

[Serializable]
public class WaveData
{
    [Min(1)] public int waveNumber = 1;
    [Min(1)] public int enemyCount = 5;
    [Min(0.05f)] public float spawnInterval = 1f;
    [Min(0f)] public float delayBeforeNextWave = 5f;
    public GameObject[] enemyPrefabs;
}
