using Unity.VisualScripting;
using UnityEngine;

public class Brick : MonoBehaviour
{
    Animator animator;


    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {




        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.attachedRigidbody;
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            }

            GetComponent<Collider2D>().enabled = false;
            animator.Play("Bbreak");
        }
    }
    public void DestroyBrick()
    {
        Destroy(gameObject);
    }
}
