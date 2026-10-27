using UnityEngine;

public class COINS : MonoBehaviour
{
    [Header("Floating Animation")]
    [SerializeField] private float floatHeight = 0.12f;
    [SerializeField] private float floatSpeed = 2.5f;

    [Header("Pickup Feedback")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField, Range(0f, 1f)] private float pickupVolume = 0.7f;
    [SerializeField] private float popupLifetime = 0.8f;
    [SerializeField] private float popupRiseDistance = 0.6f;

    private Vector3 floatingCenter;
    private bool collected;

    private void Awake()
    {
        floatingCenter = transform.position;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = floatingCenter + Vector3.up * offset;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected || !collision.CompareTag("Player"))
            return;

        collected = true;
        CoinManager.instance.AddCoin();

        CoinPickupPopup.Show(transform.position, popupRiseDistance, popupLifetime);

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupVolume);

        Destroy(gameObject);
    }
}
