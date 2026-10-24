using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinManager : MonoBehaviourPunCallbacks
{
    public static CoinManager Instance;

    [Header("Score UI")]
    [Tooltip("Local score label for the master-client player.")]
    public TextMeshProUGUI Player1Text;

    [Tooltip("Local score label for the other player.")]
    public TextMeshProUGUI Player2Text;

    [Tooltip("World-space TextMeshPro prefab used for the floating +1 score popup.")]
    public GameObject floatingScorePopupPrefab;

    [Tooltip("Height above the player where the floating score popup is created.")]
    public float floatingPopupHeight = 2f;

    [Tooltip("Duration of the floating score popup animation, in seconds.")]
    public float floatingPopupDuration = 0.8f;

    [Tooltip("Vertical travel distance of the floating score popup, in world units.")]
    public float floatingPopupRise = 1f;

    [Header("Result UI")]
    [Tooltip("Panel shown when a player wins, loses, or the opponent disconnects.")]
    public GameObject WinLosePanel;

    [Tooltip("Text displayed inside the result panel.")]
    public TextMeshProUGUI ResultText;

    [Tooltip("Optional sound played when the local player wins.")]
    public AudioClip winStinger;

    [Tooltip("Optional sound played when the local player loses.")]
    public AudioClip lossStinger;

    [Tooltip("Duration of the result-panel scale animation, in seconds.")]
    public float panelPopDuration = 0.35f;

    private Vector3 panelTargetScale = Vector3.one;
    private Coroutine panelPopRoutine;
    private Coroutine player1PunchRoutine;
    private Coroutine player2PunchRoutine;
    private bool disconnectReturnPending;
    private bool menuLoadStarted;
    private bool resultShown;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (WinLosePanel != null)
        {
            panelTargetScale = WinLosePanel.transform.localScale;
            WinLosePanel.SetActive(false);
        }
    }

    private void Start()
    {
        if (Player1Text != null)
            Player1Text.text = "YOU : 0/20";
        if (Player2Text != null)
            Player2Text.text = "ENEMY : 0/20";
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void UpdateScoreText(bool player1, int score)
    {
        TextMeshProUGUI scoreText = player1 ? Player1Text : Player2Text;
        if (scoreText == null)
            return;

        scoreText.text = player1
            ? $"PLAYER 1  : {score}/20"
            : $"PLAYER 2  : {score}/20";

        if (player1)
        {
            if (player1PunchRoutine != null)
                StopCoroutine(player1PunchRoutine);
            player1PunchRoutine = StartCoroutine(PunchScoreText(scoreText));
        }
        else
        {
            if (player2PunchRoutine != null)
                StopCoroutine(player2PunchRoutine);
            player2PunchRoutine = StartCoroutine(PunchScoreText(scoreText));
        }
    }

    private IEnumerator PunchScoreText(TextMeshProUGUI scoreText)
    {
        Transform scoreTransform = scoreText.transform;
        Vector3 baseScale = scoreTransform.localScale;
        const float duration = 0.22f;
        const float punchAmount = 0.22f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float normalizedTime = Mathf.Clamp01(elapsed / duration);
            float scale = 1f + Mathf.Sin(normalizedTime * Mathf.PI) * punchAmount;
            scoreTransform.localScale = baseScale * scale;
            yield return null;
        }

        scoreTransform.localScale = baseScale;
    }

    public void SpawnFloatingScore(Vector3 playerPosition)
    {
        if (floatingScorePopupPrefab == null)
            return;

        GameObject popup = Instantiate(
            floatingScorePopupPrefab,
            playerPosition + Vector3.up * floatingPopupHeight,
            Quaternion.identity);

        TextMeshPro popupText = popup.GetComponentInChildren<TextMeshPro>();
        if (popupText == null)
        {
            Debug.LogWarning("Floating score popup prefab needs a world-space TextMeshPro component.", popup);
            Destroy(popup);
            return;
        }

        popupText.text = "+1";
        StartCoroutine(AnimateFloatingScore(popup.transform, popupText));
    }

    private IEnumerator AnimateFloatingScore(Transform popupTransform, TextMeshPro popupText)
    {
        Vector3 startPosition = popupTransform.position;
        Color startColor = popupText.color;
        float duration = Mathf.Max(0.01f, floatingPopupDuration);
        float elapsed = 0f;

        while (elapsed < duration && popupTransform != null && popupText != null)
        {
            elapsed += Time.unscaledDeltaTime;
            float normalizedTime = Mathf.Clamp01(elapsed / duration);
            float easedTime = 1f - Mathf.Pow(1f - normalizedTime, 3f);
            popupTransform.position = startPosition + Vector3.up * (floatingPopupRise * easedTime);

            Color color = startColor;
            color.a = startColor.a * (1f - normalizedTime);
            popupText.color = color;
            yield return null;
        }

        if (popupTransform != null)
            Destroy(popupTransform.gameObject);
    }

    public void ShowResult(bool win)
    {
        resultShown = true;
        if (ResultText != null)
            ResultText.text = win ? "YOU WIN!" : "YOU LOSE!";

        AudioClip stinger = win ? winStinger : lossStinger;
        if (stinger != null)
            AudioSource.PlayClipAtPoint(stinger, Camera.main != null ? Camera.main.transform.position : transform.position);

        ShowResultPanel();
    }

    private void ShowResultPanel()
    {
        if (WinLosePanel == null)
            return;

        WinLosePanel.SetActive(true);
        if (panelPopRoutine != null)
            StopCoroutine(panelPopRoutine);
        panelPopRoutine = StartCoroutine(PopResultPanel());
    }

    private IEnumerator PopResultPanel()
    {
        Transform panelTransform = WinLosePanel.transform;
        float duration = Mathf.Max(0.01f, panelPopDuration);
        float elapsed = 0f;
        panelTransform.localScale = Vector3.zero;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float normalizedTime = Mathf.Clamp01(elapsed / duration);
            float easedTime = 1f - Mathf.Pow(1f - normalizedTime, 3f);
            panelTransform.localScale = Vector3.LerpUnclamped(Vector3.zero, panelTargetScale, easedTime);
            yield return null;
        }

        panelTransform.localScale = panelTargetScale;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (resultShown || disconnectReturnPending || menuLoadStarted)
            return;

        disconnectReturnPending = true;
        if (ResultText != null)
            ResultText.text = "OPPONENT DISCONNECTED";
        ShowResultPanel();
        StartCoroutine(ReturnToMenuAfterDisconnect());
    }

    private IEnumerator ReturnToMenuAfterDisconnect()
    {
        yield return new WaitForSecondsRealtime(3f);

        if (!PhotonNetwork.InRoom)
        {
            LoadMainMenu();
            yield break;
        }

        PhotonNetwork.LeaveRoom();
        float timeout = Time.realtimeSinceStartup + 5f;
        while (PhotonNetwork.InRoom && Time.realtimeSinceStartup < timeout)
            yield return null;

        LoadMainMenu();
    }

    public override void OnLeftRoom()
    {
        if (disconnectReturnPending)
            LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        if (menuLoadStarted)
            return;

        menuLoadStarted = true;
        SceneManager.LoadScene("01MainMenu");
    }

    [PunRPC]
    public void GameOver(bool player1Won)
    {
        bool iAmPlayer1 = PhotonNetwork.LocalPlayer != null
            && PhotonNetwork.LocalPlayer.IsMasterClient;

        ShowResult(iAmPlayer1 == player1Won);
    }
}
