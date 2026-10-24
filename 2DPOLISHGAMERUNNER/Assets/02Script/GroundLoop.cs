using UnityEngine;

public class GroundLoop : MonoBehaviour
{
    

    [Header("Layer")]
    [Range(0f, 1f)]
    public float speedMultiplier = 1f;

    [Header("Loop")]
    public float loopLength = 19f;
   
    private void Update()
    {

        // Apply layer speed
        float layerSpeed = GameDifficulty.Instance.CurrentSpeed * speedMultiplier;

        // MOVE LEFT ON X
        transform.position += Vector3.left * layerSpeed * Time.deltaTime;

        // LOOP
        if (transform.position.x <= -loopLength)
        {
            transform.position += Vector3.right * loopLength * 3f;
        }
    }
}