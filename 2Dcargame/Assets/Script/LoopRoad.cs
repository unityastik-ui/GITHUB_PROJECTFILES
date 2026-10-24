using UnityEngine;

public class LoopRoad : MonoBehaviour
{
    public float Height;
    public int speed = 5;
    void Start()
    {
        Height = GetComponent<SpriteRenderer>().bounds.size.y;
    }
    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (transform.position.y <= -Height)
        {
            transform.position += new Vector3(0, 2 * Height, 0);
        }
    }
}
