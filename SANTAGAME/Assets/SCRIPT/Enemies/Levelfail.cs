using UnityEngine;

public class Levelfail : MonoBehaviour
{
    public GameObject gameOverPanel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Time.timeScale = 0f;   // pause game
            gameOverPanel.SetActive(true);
        }
    }
}
