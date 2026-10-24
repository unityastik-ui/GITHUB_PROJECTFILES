using UnityEngine;

public class End : MonoBehaviour
{
    private EnvironmentEndless environmentEndless;

    private void Start()
    {
        environmentEndless = GameObject.Find("EnvironmentManager").GetComponent<EnvironmentEndless>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            environmentEndless.Shuffle();
            print("End Triggered");
        }
    }
}