using UnityEngine;

public class ArcherAutoAttack : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float range = 5f;
    [SerializeField, Min(0.05f)] private float fireCooldown = 1f;
    [SerializeField, Min(0f)] private float damage = 10f;
    [SerializeField, Min(0.1f)] private float projectileSpeed = 12f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Animator archerAnimator;
    [SerializeField] private SpriteRenderer archerRenderer;
    [SerializeField] private string idleStateName = "Archer_Idle_Black";
    [SerializeField] private string shootStateName = "Archer_Shoot_Black";
    [SerializeField] private bool facesRightByDefault = true;

    private float nextFireTime;
    private int idleStateHash;
    private int shootStateHash;

    private void Awake()
    {
        if (archerAnimator == null)
            archerAnimator = GetComponentInChildren<Animator>();

        if (archerRenderer == null && archerAnimator != null)
            archerRenderer = archerAnimator.GetComponent<SpriteRenderer>();

        idleStateHash = Animator.StringToHash($"Base Layer.{idleStateName}");
        shootStateHash = Animator.StringToHash($"Base Layer.{shootStateName}");

        if (archerAnimator == null)
        {
            Debug.LogError($"[{nameof(ArcherAutoAttack)}] Assign an Animator on the Archer child.", this);
            return;
        }

        if (!archerAnimator.HasState(0, idleStateHash) || !archerAnimator.HasState(0, shootStateHash))
            Debug.LogError($"[{nameof(ArcherAutoAttack)}] The assigned Animator Controller must contain '{idleStateName}' and '{shootStateName}' states.", this);
    }

    private void Update()
    {
        EnemyHealth target = FindNearestEnemy();
        if (target == null)
        {
            PlayIdle();
            return;
        }

        FaceTarget(target);
        if (Time.time < nextFireTime)
        {
            ReturnToIdleWhenShootClipEnds();
            return;
        }

        nextFireTime = Time.time + fireCooldown;
        if (archerAnimator != null)
            archerAnimator.Play(shootStateHash, 0, 0f);

        FireAt(target);
    }

    private void FaceTarget(EnemyHealth target)
    {
        if (archerRenderer == null)
            return;

        Transform archerTransform = archerRenderer.transform;
        bool targetIsLeft = target.transform.position.x < archerTransform.position.x;
        archerRenderer.flipX = targetIsLeft == facesRightByDefault;
    }

    private void PlayIdle()
    {
        if (archerAnimator == null)
            return;

        AnimatorStateInfo currentState = archerAnimator.GetCurrentAnimatorStateInfo(0);
        if (!currentState.IsName($"Base Layer.{idleStateName}"))
            archerAnimator.Play(idleStateHash, 0, 0f);
    }

    private void ReturnToIdleWhenShootClipEnds()
    {
        if (archerAnimator == null)
            return;

        AnimatorStateInfo currentState = archerAnimator.GetCurrentAnimatorStateInfo(0);
        if (currentState.IsName($"Base Layer.{shootStateName}") && currentState.normalizedTime >= 1f)
            archerAnimator.Play(idleStateHash, 0, 0f);
    }

    private EnemyHealth FindNearestEnemy()
    {
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
        EnemyHealth closest = null;
        float rangeSquared = range * range;
        float closestDistanceSquared = rangeSquared;

        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy == null || !enemy.IsAlive)
                continue;

            float distanceSquared = ((Vector2)enemy.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (distanceSquared <= closestDistanceSquared)
            {
                closestDistanceSquared = distanceSquared;
                closest = enemy;
            }
        }

        return closest;
    }

    private void FireAt(EnemyHealth target)
    {
        Vector3 origin = firePoint != null ? firePoint.position : transform.position;
        if (firePoint != null && archerRenderer != null &&
            archerRenderer.flipX && firePoint.IsChildOf(archerRenderer.transform))
        {
            Vector3 mirroredOffset = firePoint.localPosition;
            mirroredOffset.x = -mirroredOffset.x;
            origin = firePoint.parent.TransformPoint(mirroredOffset);
        }

        GameObject projectileObject = new GameObject("ArrowProjectile");
        projectileObject.transform.position = origin;

        LineRenderer line = projectileObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.startWidth = 0.07f;
        line.endWidth = 0.035f;
        line.startColor = new Color(0.95f, 0.82f, 0.45f);
        line.endColor = new Color(0.75f, 0.42f, 0.16f);
        line.sortingLayerName = "Player";
        line.sortingOrder = 10;

        ArrowProjectile projectile = projectileObject.AddComponent<ArrowProjectile>();
        projectile.Initialize(target, damage, projectileSpeed, line);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
