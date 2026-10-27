using UnityEngine;

public class Cuttypenguin : MonoBehaviour
{
    private Santaplayer player;

    public float damageInterval = 1f; // 1 second
    private float damageTimer;
    public int health = 100;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<Santaplayer>();
            player.TakeDamage(5);      // instant hit
            damageTimer = 0f;          // reset timer
        }
    }
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && player != null)
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                player.TakeDamage(1);  // damage over time
                damageTimer = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = null;
            damageTimer = 0f;
        }
    }
}
