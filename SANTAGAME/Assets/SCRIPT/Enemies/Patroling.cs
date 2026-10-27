using UnityEngine;
using UnityEngine.Splines;

public class Patroling : MonoBehaviour
{
    public Transform leftPoint;
    public Transform rightPoint;
    public float speed = 2f;
    private Transform targetPoint;  
    private bool StopPatrol;
 
    void Start()
    {
        targetPoint = rightPoint;   
     
    }
    void Update()
    {
        if (!StopPatrol)
        {
            Patrol();
        }
    }
    void Patrol()
    {
        transform.position = Vector2.MoveTowards(
           transform.position,
           targetPoint.position,
           speed * Time.deltaTime
       );

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.5f)
        {
            if (targetPoint == rightPoint)
            {
                targetPoint = leftPoint;
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            }
            else
            {
                targetPoint = rightPoint;
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        { StopPatrol = true; }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           
            StopPatrol = false; 
        }
    }
    
}
