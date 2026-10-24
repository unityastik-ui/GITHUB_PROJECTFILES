using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopButtonscript : MonoBehaviour
{
    public void OpenShop()
    {
        SceneManager.LoadScene("SHOPSCENE");
        print("Shopbuttonwrkng");
    }
}
