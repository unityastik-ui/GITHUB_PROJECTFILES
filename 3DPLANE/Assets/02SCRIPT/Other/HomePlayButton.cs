using UnityEngine;
using UnityEngine.SceneManagement;

public class HomePlayButton : MonoBehaviour
{
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LoadingScene");
    }
}