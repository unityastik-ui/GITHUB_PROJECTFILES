using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Button Audio")]
    [Tooltip("Optional sound played when Host or Join is pressed.")]
    public AudioClip buttonClickClip;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        print("Connecting to Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to Photon Master Server.");
        PhotonNetwork.JoinLobby();
    }
    public void CreateRoom()
    {
        PlayButtonClick();
        print("HOST BUTTON CLICKED Creating Room...");
        PhotonNetwork.CreateRoom("ASTIK");
    }
    public override void OnJoinedLobby()
    {
        print("Joined Lobby.");
    }

    public void JoinRoom()
    {
        PlayButtonClick();
        print("JOIN BUTTON CLICKED Joining Room...");
        PhotonNetwork.JoinRoom("ASTIK");
    }

    public override void OnCreatedRoom()
    {
        print("Room Created Successfully.");
    }

    public override void OnJoinedRoom()
    {
        print("Joined Room Astik.");
        if (PhotonNetwork.IsMasterClient)
        {
            print("I am the Master Client. Loading the game scene...");
            PhotonNetwork.LoadLevel("02Lobby");
        }

    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("Player Joined: " + newPlayer.NickName);
        Debug.Log("Players In Room: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Join Failed");
        Debug.LogError("Code : " + returnCode);
        Debug.LogError("Message : " + message);
    }

    private void PlayButtonClick()
    {
        if (buttonClickClip == null)
            return;

        Camera mainCamera = Camera.main;
        AudioSource.PlayClipAtPoint(
            buttonClickClip,
            mainCamera != null ? mainCamera.transform.position : transform.position);
    }
}
