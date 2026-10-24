using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerCar : MonoBehaviour
{
    [Header("Steering")]
    public float steeringSpeed = 8f;
    public float minX = -1.3f;
    public float maxX = 1.3f;

    [Header("Visual Tilt")]
    public float maxTurnAngle = 15f;
    public float turnSmoothing = 15f;

    [Header("Tire Skid Trails")]
    public TrailRenderer leftTrail;
    public TrailRenderer rightTrail;

    void Start()
    {
        if (Time.timeScale == 0f)
            Time.timeScale = 1f;

        SetSkidEmitting(false);
    }

    void Update()
    {
        float input = ReadInput();

        // Horizontal movement
        if (input != 0f)
        {
            Vector3 pos = transform.position;
            pos.x += input * steeringSpeed * Time.deltaTime;
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            transform.position = pos;

            // Turn ON skid marks while steering
            SetSkidEmitting(true);
        }
        else
        {
            // Turn OFF skid marks when going straight
            SetSkidEmitting(false);
        }

        // Visual tilt
        float targetAngle = -input * maxTurnAngle;
        float currentAngle = transform.eulerAngles.z;
        float smoothedAngle = Mathf.LerpAngle(currentAngle, targetAngle, turnSmoothing * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, smoothedAngle);
    }

    float ReadInput()
    {
        float steer = 0f;

        // 1. Check New Input System (PC Keyboard)
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
                steer -= 1f;
            if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
                steer += 1f;

            if (steer != 0f)
                return steer;
        }
#endif

        // 2. Check Legacy Input (Fallback)
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            steer -= 1f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            steer += 1f;

        if (steer != 0f)
            return steer;

        // 3. Fallback to Unity Input Axis
        float axis = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(axis) > 0.05f)
            return Mathf.Sign(axis);

        // 4. Fallback to Mobile Tilt (Accelerometer)
        if (Mathf.Abs(Input.acceleration.x) > 0.1f)
            return Mathf.Clamp(Input.acceleration.x * 2f, -1f, 1f);

        return 0f;
    }

    void SetSkidEmitting(bool emit)
    {
        if (leftTrail != null) leftTrail.emitting = emit;
        if (rightTrail != null) rightTrail.emitting = emit;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        SetSkidEmitting(false);
        enabled = false;

        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
    }
}