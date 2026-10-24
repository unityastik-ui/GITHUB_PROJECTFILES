using Photon.Pun;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : MonoBehaviourPun
{
    private void Start()
    {
        if (photonView == null)
        {
            Debug.LogError("PlayerNetwork requires a PhotonView.", this);
            return;
        }

        if (photonView.IsMine)
        {
            Debug.Log("This is my player.");
            return;
        }

        Debug.Log("This is the other player.");

        ThirdPersonController controller = GetComponent<ThirdPersonController>();
        if (controller != null)
            controller.enabled = false;

        StarterAssetsInputs inputs = GetComponent<StarterAssetsInputs>();
        if (inputs != null)
            inputs.enabled = false;

        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = false;
    }
}
