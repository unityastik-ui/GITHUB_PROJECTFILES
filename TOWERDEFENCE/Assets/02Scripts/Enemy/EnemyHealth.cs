using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField, Min(1f)] private float maximumHealth = 100f;

    private float currentHealth;
    private WaveSpawner owningSpawner;
    private bool isAlive = true;
    private bool isRegistered;

    public float MaximumHealth => maximumHealth;
    public float CurrentHealth => currentHealth;
    public bool IsAlive => isAlive;

    private void Awake()
    {
        maximumHealth = Mathf.Max(1f, maximumHealth);
        currentHealth = maximumHealth;
    }

    public void Configure(float healthMultiplier, WaveSpawner spawner)
    {
        UnregisterFromSpawner();
        maximumHealth = Mathf.Max(1f, maximumHealth * Mathf.Max(0.01f, healthMultiplier));
        currentHealth = maximumHealth;
        isAlive = true;
        owningSpawner = spawner;

        if (owningSpawner != null)
        {
            owningSpawner.RegisterEnemy(this);
            isRegistered = true;
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive || damage <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        if (currentHealth <= 0f)
            Die();
    }

    public void RemoveAtCastle()
    {
        if (!isAlive)
            return;

        isAlive = false;
        UnregisterFromSpawner();
        Destroy(gameObject);
    }

    private void Die()
    {
        isAlive = false;
        UnregisterFromSpawner();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        UnregisterFromSpawner();
    }

    private void UnregisterFromSpawner()
    {
        if (!isRegistered || owningSpawner == null)
            return;

        owningSpawner.UnregisterEnemy(this);
        isRegistered = false;
    }
}
