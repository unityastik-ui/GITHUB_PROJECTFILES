using UnityEngine;

public class EnemyCar : MonoBehaviour
{
    public float speed = 6f;
    public float destroyY = -8f;
    [SerializeField] float despawnBelowCamera = 1f;

    float baseSpeed;
    Camera mainCamera;

    void Awake()
    {
        baseSpeed = speed;
        mainCamera = Camera.main;
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);

        float despawnY = destroyY;
        if (mainCamera != null)
        {
            float depth = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
            despawnY = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f, depth)).y - despawnBelowCamera;
        }

        if (transform.position.y < despawnY)
            Destroy(gameObject);
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speed = baseSpeed * Mathf.Max(1f, multiplier);
    }
}
