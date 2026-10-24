using UnityEngine;
using Photon.Pun;

public class Playercolor : MonoBehaviourPun
{
    [SerializeField] private Material blueBody;
    [SerializeField] private Material blueArms;
    [SerializeField] private Material blueLegs;

    [SerializeField] private Material redBody;
    [SerializeField] private Material redArms;
    [SerializeField] private Material redLegs;

    private void Start()
    {
        SkinnedMeshRenderer mesh= GetComponentInChildren<SkinnedMeshRenderer>();
        if(photonView.Owner.IsMasterClient)
        { 
            mesh.materials = new Material[] { blueBody, blueArms, blueLegs };
        }
        else
        {
            mesh.materials = new Material[] { redBody, redArms, redLegs };
        }
    }
}
