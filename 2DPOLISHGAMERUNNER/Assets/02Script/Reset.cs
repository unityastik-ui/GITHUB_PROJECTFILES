using UnityEngine;

public class Reset : MonoBehaviour
{
   public void ResetGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
