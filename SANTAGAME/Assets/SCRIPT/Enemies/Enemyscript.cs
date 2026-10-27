using UnityEngine;

public class Enemyscript : MonoBehaviour
{
    [Header("Movement Points")]
    public Transform leftPoint;
    public Transform rightPoint;
    public Transform player;
    public float speed = 2f;
    public float attackAfter = 4f;

    [Header("Health")]
    public int maxHealth = 200;
    public int currentHealth;
    private bool isDead = false;

    private Transform target;
    private SpriteRenderer sr;
    private Animator anim;

    private float timer = 0f;
    private bool attacking = false;

    void Start()
    {
        target = rightPoint;
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return; // stop all logic if dead
        if (attacking) return;

        // WALK between points
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // TURN AT ENDS
        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            if (target == rightPoint)
            {
                target = leftPoint;
                sr.flipX = true;   // face left
            }
            else
            {
                target = rightPoint;
                sr.flipX = false; // face right
            }
        }

        // COUNT TIME
        timer += Time.deltaTime;

        // TRY TO ATTACK
        if (timer >= attackAfter && IsFacingPlayer())
        {
            timer = 0f;
            attacking = true;
            anim.SetTrigger("attack");
        }
    }

    bool IsFacingPlayer()
    {
        bool playerOnRight = player.position.x > transform.position.x;
        bool facingRight = sr.flipX == false;
        return (playerOnRight && facingRight) || (!playerOnRight && !facingRight);
    }

    // Called from attack animation event
    public void AttackFinished()
    {
        attacking = false;
    }

    // 🔴 DAMAGE HANDLER
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;        
        if (currentHealth <= 0)
        {
            isDead = true;
            anim.Play("snwdead");
            FlagGate.instance.OpenGate();           
            this.enabled = false;
            Destroy(gameObject, 5f);
        }
    }

   
}
