using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonscr : MonoBehaviour
{
    public void BackShop()
    {
        SceneManager.LoadScene("HomeScreen");
        print("Shop Back buttonwrkng");
    }
}
