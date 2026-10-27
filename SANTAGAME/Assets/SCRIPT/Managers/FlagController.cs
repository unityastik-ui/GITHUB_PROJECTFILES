using UnityEngine;

public class FlagController : MonoBehaviour
{
    public GameObject levelCompleteUI;
    public BoxCollider2D blockerCollider; // solid collider

    void Start()
    {
        // Blocker enabled at start
        blockerCollider.enabled = true;
    }

    public void UnlockFlag()
    {
        // Disable ONLY blocker
        blockerCollider.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            levelCompleteUI.SetActive(true);
        }
    }
}
