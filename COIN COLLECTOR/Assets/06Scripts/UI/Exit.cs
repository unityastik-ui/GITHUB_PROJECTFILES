using UnityEngine;

public class Exit : MonoBehaviour
{
    public void ExitGame()
    {
        print("Exiting the game...");
        Application.Quit();
    }
}
