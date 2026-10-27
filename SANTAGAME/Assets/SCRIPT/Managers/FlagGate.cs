using UnityEngine;

public class FlagGate : MonoBehaviour
{
    public static FlagGate instance;

    BoxCollider2D col;
    

    void Awake()
    {
        instance = this;
        col = GetComponent<BoxCollider2D>();
       
    }

    
    public void OpenGate()
    {
        col.enabled = false; 
       
    }
}
