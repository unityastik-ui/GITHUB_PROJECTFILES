using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Speed")]
    public float speedMultiplier = 1f;

    [Header("Destroy")]
    public float destroyX = -15f;

    private void Update()
    {
        float enemySpeed = GameDifficulty.Instance.CurrentSpeed * speedMultiplier;

        transform.Translate(Vector3.left * enemySpeed * Time.deltaTime);

        if (transform.position.x <= destroyX)
        {
            Destroy(gameObject);
        }
    }
}