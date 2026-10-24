using System.Collections.Generic;
using UnityEngine;

public class EnvironmentEndless : MonoBehaviour
{
    public Transform[] EnvironmentPrefabs;//ENVIRONMENT PREFAB TO BE ASSIGNED
    public int initialEnvironment = 5;
    private List<Transform> activeEnvironment = new List<Transform>();
    public float EnvironmentLength = 200f;
    private float spawnZ = 0f;

    private void Start()
    {
        for (int i = 0; i < initialEnvironment; i++)
        {
            SpawnEnvironment();
        }
    }
    void SpawnEnvironment()
    {
        Transform prefab = Instantiate(EnvironmentPrefabs[0], new Vector3(0, -6.5f, spawnZ), Quaternion.identity);
        activeEnvironment.Add(prefab);
        spawnZ += EnvironmentLength;
    }
    public void Shuffle()
    {
        Transform FirstEnvironment = activeEnvironment[0];
        activeEnvironment.RemoveAt(0);
        FirstEnvironment.position = new Vector3(0, -6.5f, spawnZ);
        activeEnvironment.Add(FirstEnvironment);
        spawnZ += EnvironmentLength;
    }
}
