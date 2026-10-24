using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerScore : MonoBehaviourPun
{
    [Header("Score")]
    [Tooltip("Number of coins collected by this player.")]
    public int coin;

    [Tooltip("Set at runtime from the owning Photon player's master-client status.")]
    public bool isPlayer1;

    private bool winReported;

    private void Start()
    {
        if (photonView == null || photonView.Owner == null)
            return;

        isPlayer1 = photonView.Owner.IsMasterClient;
        if (isPlayer1)
            Debug.Log("I am Player 1.");
        else
            Debug.Log("I am Player 2.");

        CoinManager manager = CoinManager.Instance;
        if (manager != null)
            manager.UpdateScoreText(isPlayer1, coin);
    }

    [PunRPC]
    public void UpdateScoreOnOtherPlayer(bool player1, int score)
    {
        CoinManager manager = CoinManager.Instance;
        if (manager != null)
            manager.UpdateScoreText(player1, score);
    }

    public void AddCoin()
    {
        if (photonView == null || !photonView.IsMine)
            return;

        coin++;

        CoinManager manager = CoinManager.Instance;
        if (manager != null)
        {
            manager.UpdateScoreText(isPlayer1, coin);
            manager.SpawnFloatingScore(transform.position);

            if (coin >= 20 && !winReported)
            {
                winReported = true;
                if (manager.photonView != null)
                {
                    manager.photonView.RPC(
                        nameof(CoinManager.GameOver),
                        RpcTarget.All,
                        isPlayer1);
                }
                else
                {
                    Debug.LogError("CoinManager needs a PhotonView to broadcast the game result.", manager);
                }
            }
        }

        photonView.RPC(nameof(UpdateScoreOnOtherPlayer), RpcTarget.Others, isPlayer1, coin);
        Debug.Log("My coins: " + coin);
    }
}
