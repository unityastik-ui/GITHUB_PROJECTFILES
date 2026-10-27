using UnityEngine;
using UnityEngine.UI;

public class Santaplayer : MonoBehaviour
{
    #region Variables

    public float H;
    public bool isgrounded = true;

    public int Speed = 5;
    public int jumpstrength = 8;
    public int Jumpcount = 0;
    public GameObject mobileControls;

    Rigidbody2D rb;
    Animator _Animator;

    // MOBILE BUTTONS
    bool moveLeft;
    bool moveRight;

    [Header("Health")]
    public int PlMaxHealth = 100;
    public int PlCurrentHealth;
    public Slider healthSlider;

    public LayerMask GroundLayer;
    public GameManager1 gameManager;

    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _Animator = GetComponent<Animator>();

        PlCurrentHealth = PlMaxHealth;

        healthSlider.maxValue = PlMaxHealth;
        healthSlider.value = PlCurrentHealth;
    }

    void Update()
    {
        // PC KEYBOARD
        H = Input.GetAxisRaw("Horizontal");

        // MOBILE LEFT
        if (moveLeft)
            H = -1;

        // MOBILE RIGHT
        if (moveRight)
            H = 1;

        // PC JUMP
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }

        Move();
    }

    void Move()
    {
        bool wasGrounded = isgrounded;

        var ray = Physics2D.Raycast(transform.position, Vector2.down, 0.8f, GroundLayer);

        if (!ray)
        {
            isgrounded = false;
            _Animator.SetBool("isJump", true);
        }
        else
        {
            isgrounded = true;

            if (!wasGrounded)
                Jumpcount = 0;

            _Animator.SetBool("isJump", false);
        }

        // MOVE
        rb.linearVelocity = new Vector2(H * Speed, rb.linearVelocity.y);

        // WALK ANIMATION
        _Animator.SetInteger("isWalking", H == 0 ? 0 : 1);

        // FLIP PLAYER
        if (H < 0)
            transform.rotation = Quaternion.Euler(0, 180, 0);

        else if (H > 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    // MOBILE LEFT BUTTON
    public void LeftDown()
    {
        moveLeft = true;
    }

    public void LeftUp()
    {
        moveLeft = false;
    }

    // MOBILE RIGHT BUTTON
    public void RightDown()
    {
        moveRight = true;
    }

    public void RightUp()
    {
        moveRight = false;
    }

    // JUMP
    public void Jump()
    {
        if (Jumpcount < 2)
        {
            Jumpcount++;

            rb.AddForce(Vector2.up * jumpstrength, ForceMode2D.Impulse);
        }
    }

    // DAMAGE
    public void TakeDamage(int damage)
    {
        PlCurrentHealth -= damage;

        PlCurrentHealth = Mathf.Clamp(PlCurrentHealth, 0, PlMaxHealth);

        healthSlider.value = PlCurrentHealth;

        if (PlCurrentHealth <= 0)
        {
            Die();
        }
    }

    // HEAL
    public void Heal(int amount)
    {
        PlCurrentHealth += amount;

        PlCurrentHealth = Mathf.Clamp(PlCurrentHealth, 0, PlMaxHealth);

        healthSlider.value = PlCurrentHealth;
    }

    void Die()
    {
        mobileControls.SetActive(false);
        _Animator.Play("deadp");

        rb.linearVelocity = Vector2.zero;

        rb.simulated = false;

        this.enabled = false;

        gameManager.PlayerDied();
    }
}