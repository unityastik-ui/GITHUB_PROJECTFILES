using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Movement")]
    public float scrollSpeed = 5f;
    public float destroyY = -9f;

    [Header("Score Value")]
    public int coinValue = 1;

    void Update()
    {
        // 1. Move the coin down with the road
        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime, Space.World);

        // 2. Gentle spin so it looks 3D
        transform.Rotate(0f, 180f * Time.deltaTime, 0f);

        // 3. Remove coin if it goes below the screen
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object touching the coin is tagged "Player"
        if (other.CompareTag("Player"))
        {
            // Add to score/coins
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddCoin(coinValue);
            }

            // Remove the coin from the scene
            Destroy(gameObject);
        }
    }
}