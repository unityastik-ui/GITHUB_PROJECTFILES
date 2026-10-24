using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Lobby UI")]
    [Tooltip("Displays the number of players currently in the room.")]
    public TextMeshProUGUI playerCountText;

    [Tooltip("Displays the waiting or ready status.")]
    public TextMeshProUGUI statusText;

    [Tooltip("Button that allows the master client to start the match.")]
    public Button startGameButton;

    [Header("Button Audio")]
    [Tooltip("Optional sound played when Start Game or Leave Lobby is pressed.")]
    public AudioClip buttonClickClip;

    [Header("Status Animation")]
    [Tooltip("Speed of the waiting-status text pulse.")]
    public float statusPulseSpeed = 2f;

    [Tooltip("Minimum alpha of the waiting-status text during its pulse.")]
    [Range(0f, 1f)]
    public float statusMinimumAlpha = 0.55f;

    private Coroutine statusPulseRoutine;
    private Color statusBaseColor = Color.white;

    private void Start()
    {
        if (statusText != null)
            statusBaseColor = statusText.color;
        UpdateLobbyUI();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateLobbyUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    public void LeaveLobby()
    {
        PlayButtonClick();
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("01MainMenu");
    }

    public void StartGame()
    {
        PlayButtonClick();
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("03Gameplay");
    }

    private void UpdateLobbyUI()
    {
        int playerCount = PhotonNetwork.CurrentRoom != null
            ? PhotonNetwork.CurrentRoom.PlayerCount
            : 0;
        bool waitingForPlayer = playerCount < 2;

        if (playerCountText != null)
            playerCountText.text = $"PLAYERS\n{playerCount}/2";
        if (statusText != null)
            statusText.text = waitingForPlayer
                ? "WAITING FOR ANOTHER PLAYER..."
                : "READY TO START!";
        if (startGameButton != null)
            startGameButton.interactable = !waitingForPlayer && PhotonNetwork.IsMasterClient;

        SetStatusPulse(waitingForPlayer);
    }

    private void SetStatusPulse(bool shouldPulse)
    {
        if (statusText == null)
            return;

        if (statusPulseRoutine != null)
        {
            StopCoroutine(statusPulseRoutine);
            statusPulseRoutine = null;
        }

        if (shouldPulse)
        {
            statusPulseRoutine = StartCoroutine(PulseStatusText());
        }
        else
        {
            Color color = statusText.color;
            color.a = statusBaseColor.a;
            statusText.color = color;
        }
    }

    private IEnumerator PulseStatusText()
    {
        while (statusText != null)
        {
            float pulse = (Mathf.Sin(Time.unscaledTime * statusPulseSpeed) + 1f) * 0.5f;
            Color color = statusBaseColor;
            color.a = Mathf.Lerp(statusMinimumAlpha, statusBaseColor.a, pulse);
            statusText.color = color;
            yield return null;
        }
    }

    private void PlayButtonClick()
    {
        if (buttonClickClip != null)
            AudioSource.PlayClipAtPoint(buttonClickClip, transform.position);
    }
}
