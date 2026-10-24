using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButton : MonoBehaviourPunCallbacks
{
    [Header("Button Audio")]
    [Tooltip("Optional sound played when the Home button is pressed.")]
    public AudioClip buttonClickClip;

    private bool returnToMenuWhenLeft;

    public void Home()
    {
        PlayButtonClick();

        if (PhotonNetwork.InRoom)
        {
            returnToMenuWhenLeft = true;
            PhotonNetwork.LeaveRoom();
            return;
        }

        LoadMainMenu();
    }

    public override void OnLeftRoom()
    {
        if (returnToMenuWhenLeft)
            LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("01MainMenu");
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
