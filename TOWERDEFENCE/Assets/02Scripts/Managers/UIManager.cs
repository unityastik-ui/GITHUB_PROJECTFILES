using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject loosePanel;

    [Header("Scene Configuration")]
    [SerializeField] private string homeSceneName = "01HOME SCREEN";
    [SerializeField] private string gameplaySceneName = "02GAMEPLAYSCREEN";
    [SerializeField] private string nextLevelSceneName = "02GAMEPLAYSCREEN";

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("[UIManager] More than one UIManager exists in the scene.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        // Ensure all panels start hidden and game runs normally
        CloseAllPanels();
        Time.timeScale = 1f;
        Debug.Log("<color=cyan>[UIManager]</color> Initialized. All panels closed, TimeScale set to 1.");
    }

    private void Update()
    {
        // Press 'V' to test Victory Panel
        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("<color=yellow>[Input]</color> Key 'V' pressed.");
            ToggleVictoryPanel();
        }

        // Press 'L' to test Lose / Defeat Panel
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("<color=red>[Input]</color> Key 'L' pressed.");
            ToggleLoosePanel();
        }

        // Press 'Escape' or 'P' to Pause / Unpause
        if ((Input.GetKeyDown(KeyCode.Escape) && !TowerPlacementManager.EscapeCancelledPlacementThisFrame) ||
            Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("<color=orange>[Input]</color> Pause toggle key pressed (Escape/P).");
            TogglePause();
        }
    }

    // ==========================================
    // PANEL CONTROLS
    // ==========================================

    public void ToggleVictoryPanel()
    {
        bool state = victoryPanel != null && victoryPanel.activeSelf;
        if (!state)
        {
            CloseAllPanels();
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
                Debug.Log("<color=green>[Panel]</color> VICTORY PANEL OPENED | Game Paused (TimeScale = 0).");
            }
            else
            {
                Debug.LogWarning("[UIManager] Victory Panel reference is MISSING in Inspector!");
            }
            Time.timeScale = 0f;
        }
        else
        {
            Debug.Log("<color=green>[Panel]</color> Victory Panel closed.");
            ResumeGame();
        }
    }

    public void ToggleLoosePanel()
    {
        bool state = loosePanel != null && loosePanel.activeSelf;
        if (!state)
        {
            CloseAllPanels();
            if (loosePanel != null)
            {
                loosePanel.SetActive(true);
                Debug.Log("<color=red>[Panel]</color> DEFEAT / LOSE PANEL OPENED | Game Paused (TimeScale = 0).");
            }
            else
            {
                Debug.LogWarning("[UIManager] Loose Panel reference is MISSING in Inspector!");
            }
            Time.timeScale = 0f;
        }
        else
        {
            Debug.Log("<color=red>[Panel]</color> Defeat Panel closed.");
            ResumeGame();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        CloseAllPanels();
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            Debug.Log("<color=orange>[Panel]</color> PAUSE PANEL OPENED | Game Paused (TimeScale = 0).");
        }
        else
        {
            Debug.LogWarning("[UIManager] Pause Panel reference is MISSING in Inspector!");
        }
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        CloseAllPanels();
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("<color=cyan>[Button Clicked]</color> RESUME BUTTON CLICKED | Game Resumed (TimeScale = 1).");
    }

    public void CloseAllPanels()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (loosePanel != null) loosePanel.SetActive(false);
    }

    // ==========================================
    // SHARED BUTTON ACTIONS
    // ==========================================

    /// <summary>
    /// HOME / MENU BUTTON: Returns to Title Screen
    /// </summary>
    public void OnHomeOrMenuClicked()
    {
        Debug.Log("<color=yellow>[Button Clicked]</color> HOME / MENU BUTTON CLICKED -> Loading Scene: " + homeSceneName);
        Time.timeScale = 1f;
        SceneManager.LoadScene(homeSceneName);
    }

    /// <summary>
    /// RETRY BUTTON: Restarts the current level
    /// </summary>
    public void OnRetryClicked()
    {
        Debug.Log("<color=yellow>[Button Clicked]</color> RETRY BUTTON CLICKED -> Reloading Scene: " + gameplaySceneName);
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplaySceneName);
    }

    /// <summary>
    /// NEXT LEVEL BUTTON: Loads the subsequent stage
    /// </summary>
    public void OnNextLevelClicked()
    {
        Debug.Log("<color=green>[Button Clicked]</color> NEXT LEVEL BUTTON CLICKED -> Loading Scene: " + nextLevelSceneName);
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextLevelSceneName);
    }
}