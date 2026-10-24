using UnityEngine;
[CreateAssetMenu(fileName = "PlaneData", menuName = "ScriptableObjects/PlaneData")]
public class PlaneData : ScriptableObject
{
    [Header("Plane Information")]
    public string planeName;
    public int PlanePrice;

    [Header("Plane Material")]
    public Material planeMaterial;

    [Header("SHOP INFO")]
    public bool isUnlocked;
}
