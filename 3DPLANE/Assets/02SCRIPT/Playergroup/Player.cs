using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region
    public float speed = 50f, Mspeed = 25f;
    public float RotateAngle = 30f;
    public float RotateSpeed = 5f;
    public float NoseAngle = 15f;//up down rotation angle
    private Rigidbody rb;
    public Joystick joystick;

    public DatabasePlane databasePlane;
    public Renderer planeRenderer;
    private bool isDead = false;


    #endregion
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StartEngine();
        }

        if (PlayerPrefs.HasKey("SelectedPlane"))
        {
            int index = PlayerPrefs.GetInt("SelectedPlane");
            PlaneData planeData = databasePlane.planeDatas[index];
            planeRenderer.material = planeData.planeMaterial;
        }
    }
    void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        IncreaseDifficulty();
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetEnginePitch(Mathf.Lerp(0.8f, 1.4f, speed / 120f));
        }

        float horizontal = Input.GetAxis("Horizontal") + joystick.Horizontal; // Left & Right        
        float vertical = Input.GetAxis("Vertical") + joystick.Vertical;// Up & Down
        rb.linearVelocity = new Vector3(horizontal * Mspeed, vertical * Mspeed, speed);
        //ROTATION
        float targetAngle = -horizontal * RotateAngle;
        float pitchAngle = -vertical * NoseAngle;
        transform.rotation = Quaternion.Euler(pitchAngle, 0, targetAngle);
    }

    public void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        Debug.Log("PLAYER DEAD");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCrash();
            AudioManager.Instance.StopEngine();
        }

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        foreach (ParticleSystem particleSystem in GetComponentsInChildren<ParticleSystem>())
        {
            particleSystem.Stop();
            particleSystem.Clear();
        }

        foreach (Renderer playerRenderer in GetComponentsInChildren<Renderer>())
        {
            playerRenderer.enabled = false;
        }

        ExplosionSpawner.Spawn(transform.position);

        foreach (Collider playerCollider in GetComponentsInChildren<Collider>())
        {
            playerCollider.enabled = false;
        }

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(0.4f, 0.7f);
        }
        else
        {
            Debug.LogWarning("No CameraShake instance is present in the scene.", this);
        }

        int currentScore = ScoreManager.instance.GetScore();
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        print("Current Score: " + currentScore);
        print("High Score: " + highScore);

        if (currentScore >= highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save();
            print("NEW HIGHSCORE");
        }

        print("TRY AGAIN");
        StartCoroutine(EndGameAfterCrash());
    }

    private IEnumerator EndGameAfterCrash()
    {
        yield return new WaitForSecondsRealtime(3.0f);
        GameManager.instance.EndGame();
        gameObject.SetActive(false);
    }
    void IncreaseDifficulty()
    {
        int score = ScoreManager.instance.GetScore();

        speed = 50 + (score / 100) * 10;

        Mspeed = 25 + (score / 100) * 5;

        RotateAngle = 30 + (score / 100) * 2;
        NoseAngle = 15 + (score / 100) * 1;

        // LIMITS
        speed = Mathf.Clamp(speed, 50, 120);

        Mspeed = Mathf.Clamp(Mspeed, 25, 80);

        RotateAngle = Mathf.Clamp(RotateAngle, 30, 50);

        NoseAngle = Mathf.Clamp(NoseAngle, 15, 30);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("OBSTACLE"))
        {
            print("COLLISION WITH OBSTACLE");
            Die();
        }
    }

}
