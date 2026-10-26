using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    private EnemyHealth target;
    private float damage;
    private float speed;
    private LineRenderer line;

    public void Initialize(EnemyHealth targetEnemy, float hitDamage, float travelSpeed, LineRenderer renderer)
    {
        target = targetEnemy;
        damage = hitDamage;
        speed = travelSpeed;
        line = renderer;
    }

    private void Update()
    {
        if (target == null || !target.IsAlive)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPosition = target.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        Vector3 direction = (targetPosition - transform.position).normalized;
        if (line != null)
        {
            line.SetPosition(0, transform.position - direction * 0.12f);
            line.SetPosition(1, transform.position + direction * 0.12f);
        }

        if ((targetPosition - transform.position).sqrMagnitude <= 0.04f)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
