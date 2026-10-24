using UnityEngine;

public class UImanager : MonoBehaviour
{
    public static UImanager Instance;
    public GameObject PauseUI;
    public GameObject LevelcompleteUI;
    public GameObject GameOverPanel;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowPauseUI()
    {
        if (PauseUI != null)
            PauseUI.SetActive(true);
    }
    public void HidePauseUI()
    {
        if (PauseUI != null) PauseUI.SetActive(false);
    }
    public void ShowLevelCompleteUI()
    {
        if (LevelcompleteUI != null)
            LevelcompleteUI.SetActive(true);
    }
}
