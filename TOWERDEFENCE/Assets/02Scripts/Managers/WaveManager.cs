using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject SpawnLeft;
    public GameObject SpawnRight;
    public GameObject SpawnBottom;

    public int Wave = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Wave++;
            UpdateSpawns();
        }
    }

    private void UpdateSpawns()
    {
        if (SpawnLeft != null) SpawnLeft.SetActive(Wave >= 1);
        if (SpawnRight != null) SpawnRight.SetActive(Wave >= 3);
        if (SpawnBottom != null) SpawnBottom.SetActive(Wave >= 5);
    }
}