using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public float speed = 6f;
    private Transform target;
    public float lifeTime = 3f;   // destroy after 3 seconds
    private Santaplayer player;


    // Called by Snowman when bullet is spawned
    public void SetTarget(Transform player)
    {
        target = player.transform;
        Destroy(gameObject, lifeTime); // auto destroy
    }

    void Update()
    {
        if (target == null) return;
        transform.position = Vector2.MoveTowards(
          transform.position,
          target.position,
          speed * Time.deltaTime
      );
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<Santaplayer>();
            player.TakeDamage(5);
            Destroy(gameObject);
        }
    }
}
