using UnityEngine;

public class Sanbullet : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 3f;
    public int damage = 10;
    public Santaplayer Player;
            void Start()
    {
        Destroy(gameObject, lifeTime);

    }

    void Update()
    {
       
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // EPENGUIN
        Epenguin epenguin = collision.GetComponentInParent<Epenguin>();
        if (epenguin != null)
        {
            epenguin.TakeDamage(25);
            Destroy(gameObject);
            return;
        }

        // CUTTY PENGUIN
        Cuttypenguin penguin = collision.GetComponentInParent<Cuttypenguin>();
        if (penguin != null)
        {
            penguin.TakeDamage(25);
            Destroy(gameObject);
            return;
        }

        // BOSS


        Enemyscript boss = collision.GetComponent<Enemyscript>();
        if (boss != null)
        {
            boss.TakeDamage(10);
            Destroy(gameObject);
            return;
        }

        // YETI
        YetiAI yeti = collision.GetComponentInParent<YetiAI>();
        if (yeti != null)
        {
            yeti.TakeDamage(10);
            Destroy(gameObject);
            return;
        }

        // HIT ANYTHING ELSE (except player)
        if (!collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
