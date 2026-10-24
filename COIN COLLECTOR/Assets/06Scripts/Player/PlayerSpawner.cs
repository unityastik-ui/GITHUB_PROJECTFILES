using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    IEnumerator Start()
    {
        yield return null;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Player01",
                spawnPoint1.position,
                spawnPoint1.rotation);
        }
        else
        {
            PhotonNetwork.Instantiate("Player01",
                spawnPoint2.position,
                spawnPoint2.rotation);
        }
    }


    }
