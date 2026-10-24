using UnityEngine;

public class LiifeManager : MonoBehaviour
{
    public static LiifeManager Instance;

    [Header("Life Settings")]
    public int maxLife = 3;

    [Header("HEART UI")]
    public GameObject[] hearts;

    [Header("Game Over")]
    public Scenemanagement sceneManagement;

    public int currentLife;

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentLife = maxLife;
    }

    public void LoseLife()
    {
        if(currentLife > 0)
        {
            currentLife--;
            
            hearts[currentLife].SetActive(false);

            if(currentLife <= 0)

            {
                AudioManager.Instance.StopAllAudio();
                AudioManager.Instance.PlaySFX(AudioManager.Instance.gameOverSound);
                sceneManagement.Gameover();
            }

        }
    }
}
