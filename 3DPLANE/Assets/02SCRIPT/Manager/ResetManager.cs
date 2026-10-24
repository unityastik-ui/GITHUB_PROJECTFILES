using UnityEngine;

public class ResetManager : MonoBehaviour
{
    public DatabasePlane databasePlane;
   

    void Start()
    {

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        print("PlayerPrefs have been reset. All data cleared.");

    }

}
