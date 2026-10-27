using UnityEngine;

public class Epenguin : MonoBehaviour
{
    Animator animator;
    private Santaplayer player;
    public int maxhealth = 100;
    public int currenthealth;


    void Start()
    {
        animator = GetComponent<Animator>();
        currenthealth = maxhealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<Santaplayer>();

            animator.SetBool("Isattack", true);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            animator.SetBool("Isattack", false);
        }
    }

    public void TakeDamage(int Damage)
    {
        if (currenthealth > 0)
            currenthealth -= Damage;
        else
            animator.Play("Pdead");
            Destroy(gameObject,3);
    }
    public void Playerdamage()
    {
        if (player != null)
        {
            player.TakeDamage(10);
        }
    }
}
