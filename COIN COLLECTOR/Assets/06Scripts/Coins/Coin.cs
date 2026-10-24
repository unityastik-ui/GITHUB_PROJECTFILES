using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Idle Animation")]
    [Tooltip("Rotation speed around the coin's local Y axis, in degrees per second.")]
    public float rotationSpeed = 90f;

    [Tooltip("Maximum vertical bob distance in world units.")]
    public float bobAmplitude = 0.15f;

    [Tooltip("Bob animation angular frequency in radians per second.")]
    public float bobFrequency = 2f;

    [Header("Collection Feedback")]
    [Tooltip("Optional particle effect spawned at the coin when collected.")]
    public GameObject pickupVfxPrefab;

    [Tooltip("Optional sound played at the coin's position when collected.")]
    public AudioClip pickupClip;

    private Collider coinCollider;
    private MeshRenderer coinRenderer;
    private Vector3 initialLocalPosition;
    private bool collected;

    private void Awake()
    {
        coinCollider = GetComponent<Collider>();
        coinRenderer = GetComponentInChildren<MeshRenderer>();
        initialLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (coinCollider == null)
            return;

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        transform.localPosition = initialLocalPosition
            + Vector3.up * (Mathf.Sin(Time.time * bobFrequency) * bobAmplitude);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Player"))
            return;

        PlayerScore playerScore = other.GetComponent<PlayerScore>();
        if (playerScore == null)
            return;

        collected = true;

        if (coinCollider != null)
            coinCollider.enabled = false;
        if (coinRenderer != null)
            coinRenderer.enabled = false;

        if (pickupVfxPrefab != null)
        {
            GameObject effect = Instantiate(pickupVfxPrefab, transform.position, Quaternion.identity);
            ParticleSystem particles = effect.GetComponentInChildren<ParticleSystem>();
            float effectLifetime = 5f;
            if (particles != null)
            {
                ParticleSystem.MainModule main = particles.main;
                effectLifetime = main.duration + main.startLifetime.constantMax;
            }

            Destroy(effect, Mathf.Max(1f, effectLifetime));
        }

        if (pickupClip != null)
            AudioSource.PlayClipAtPoint(pickupClip, transform.position);

        playerScore.AddCoin();
        Destroy(gameObject);
    }
}
