using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    public LevelManager levelManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            levelManager.LevelComplete();
        }
    }
}
