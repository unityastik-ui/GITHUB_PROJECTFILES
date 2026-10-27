using UnityEngine;

public class YetiAI : MonoBehaviour
{
    enum State { Sleep, Chase, Attack, Return }
    State currentState = State.Sleep;

    public Transform player;
    public float speed = 5f;

    public float idleTime = 5f;
    private float idleTimer;

    public float attackTime = 1f;   // wait after attack
    private float attackTimer = 0;
    public float Chasetime = 5f;   // wait after attack
    private float ChaseTimer = 0;
    bool isDead = false;
    private bool hasAttacked;

    private Animator anim;
    private Vector3 startPos;
    public int maxhealth = 300;
    public int currenthealth;

    void Start()
    {
        anim = GetComponent<Animator>();
        startPos = transform.position;
        currentState = State.Sleep;
        currenthealth = maxhealth;
    }

    void Update()
    {
        if (currentState == State.Chase || currentState == State.Attack)
        {
            FacePlayer();
        }
        switch (currentState)
        {
            case State.Sleep:
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleTime)
                {
                    idleTimer = 0f;
                    ChaseTimer = 0f;
                    currentState = State.Chase;
                }
                break;

            case State.Chase:

                ChaseTimer += Time.deltaTime;

                if (ChaseTimer < Chasetime)
                {
                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        player.position,
                        speed * Time.deltaTime
                    );
                }
                else
                {
                    currentState = State.Return; // ❌ time over, no touch
                }
                break;

            case State.Attack:
                if (!hasAttacked)
                {
                    hasAttacked = true;
                    attackTimer = attackTime;
                    anim.SetTrigger("attack");
                }

                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0f)
                {
                    currentState = State.Return;
                }
                break;

            case State.Return:
                transform.position = Vector3.MoveTowards(transform.position, startPos, speed * Time.deltaTime);
                if (Vector3.Distance(transform.position, startPos) < 0.05f)
                {
                    hasAttacked = false;
                    ChaseTimer = 0f;     // ✅ FIX
                    attackTimer = 0f;
                    idleTimer = 0f;
                    currentState = State.Sleep;
                }
                break;
        }
    }
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currenthealth -= damage;

        if (currenthealth <= 0)
        {
            isDead = true;

            anim.Play("ytdead");

            // ✅ OPEN FLAG GATE
            FlagGate.instance.OpenGate();

            // stop movement & logic
            currentState = State.Sleep;
            this.enabled = false;

            Destroy(gameObject, 4f);
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && currentState == State.Chase)
        {
            currentState = State.Attack;
            attackTimer = attackTime;
        }
    }

    // Called from attack animation event
    public void DamagePlayer()
    {
        player.GetComponent<Santaplayer>().TakeDamage(10);
    }
    void FacePlayer()
    {
        if (player.position.x < transform.position.x)
            transform.rotation = Quaternion.identity;
        else
              if (player.position.x > transform.position.x)
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}
