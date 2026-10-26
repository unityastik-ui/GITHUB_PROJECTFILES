using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Transform[] waypoints;
    private int waypointIndex = 0;
    private Animator animator;
    private CastleHealth castleHealth;
    private EnemyHealth enemyHealth;
    public float speed = 1f;
    [SerializeField, Min(0f)] private float castleDamage = 10f;
    public bool Isattack = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetPath(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
        waypointIndex = 0;
        Isattack = false;
    }

    public void ConfigurePath(
        Transform[] newWaypoints,
        float speedMultiplier,
        CastleHealth targetCastle,
        EnemyHealth health)
    {
        SetPath(newWaypoints);
        speed = Mathf.Max(0.01f, speed * Mathf.Max(0.01f, speedMultiplier));
        castleHealth = targetCastle;
        enemyHealth = health;
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        if (waypointIndex >= waypoints.Length)
        {
            ReachCastle();
            return;
        }

        Transform target = waypoints[waypointIndex];
        if (target == null)
        {
            waypointIndex++;
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;

        if (animator != null)
        {
            animator.SetFloat("MoveX", direction.x);
            animator.SetFloat("MoveY", direction.y);
        }

        if (!Isattack && waypointIndex == waypoints.Length - 1)
        {
            Isattack = true;
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            waypointIndex++;
        }

        if (waypointIndex >= waypoints.Length)
        {
            ReachCastle();
        }
    }

    private void ReachCastle()
    {
        if (castleHealth != null)
            castleHealth.TakeDamage(castleDamage);
        else
            Debug.LogWarning($"[{nameof(EnemyMovement)}] No CastleHealth is assigned; enemy will be removed without damage.", this);

        if (enemyHealth != null)
            enemyHealth.RemoveAtCastle();
        else
            Destroy(gameObject);
    }
}