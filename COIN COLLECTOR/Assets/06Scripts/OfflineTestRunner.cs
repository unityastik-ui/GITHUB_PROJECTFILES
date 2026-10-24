using UnityEngine;
using Photon.Pun;

public class OfflineTestRunner : MonoBehaviour
{
    private void Awake()
    {
        // Only run this if we started the scene directly without connecting first
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Starting in Photon Offline Mode for testing...");
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.CreateRoom("TestRoom");
        }
    }
}