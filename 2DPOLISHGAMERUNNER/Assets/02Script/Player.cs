using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("JUMP SETTINGS")]
    public float jumpForce = 7f;

    [Header("GRAVITY SETTINGS")]
    public float minGravity = 1f;
    public float maxGravity = 5f;
    public float maxGameSpeed = 10f;

    [Header("GROUND CHECK SETTINGS")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("ANIMATION SETTINGS")]
    public Animator animator;

    private Rigidbody2D rb;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckGround();
        Jump();
        UpdateGravity();
        UpdateAnimation();
        CheckFalling();
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        animator.SetBool("IsGrounded", isGrounded);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PerformJump();
        }
    }

    public void MobileJump()
    {
        PerformJump();
    }

    private void PerformJump()
    {
        if (!isGrounded)
            return;

        float gravity = Mathf.Lerp(
            minGravity,
            maxGravity,
            GameDifficulty.Instance.CurrentSpeed / maxGameSpeed
        );

        rb.gravityScale = gravity;

        float jumpVelocity = jumpForce * Mathf.Sqrt(gravity);

        rb.AddForce(
            Vector2.up * jumpVelocity,
            ForceMode2D.Impulse
        );

        animator.SetTrigger("Jump");
        AudioManager.Instance.PlayJumpSound();
    }
    void UpdateGravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            float gravity = Mathf.Lerp(
                minGravity,
                maxGravity,
                GameDifficulty.Instance.CurrentSpeed / maxGameSpeed
            );

            rb.gravityScale = gravity;
        }
    }

    void CheckFalling()
    {
        if (!isGrounded && rb.linearVelocity.y < 0)
        {
            animator.SetBool("Isfalling", true);
        }
        else
        {
            animator.SetBool("Isfalling", false);
        }
    }

    void UpdateAnimation()
    {
        animator.SetFloat("Speed", 1f);

        if (isGrounded && rb.linearVelocity.y <= 0)
        {
            animator.Play("Prun");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //REMOVE ONE ,IFE
            LiifeManager.Instance.LoseLife();
            if (LiifeManager.Instance.currentLife > 0)
            {
                AudioManager.Instance.PlayHitSound();
            }

            //DESTROY ENEMY
            Destroy(collision.gameObject);

            //Camera Shake
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.Shake();
            }

            //Hit Stop
            if (HitStopManager.Instance != null)
            {
                HitStopManager.Instance.StopGameTemperory();
            }
        }
    }
}