using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Loader : MonoBehaviour
{
    public Slider progressBar;

    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");
        asyncLoad.allowSceneActivation = false; //Prevent the scene from activating immediately
        float progress = 0f;
        while (progress < 0.9f)//loop runs till 90%
        {
            progress += Random.Range(0.05f, 0.15f);
            progress = Mathf.Min(progress, 0.9f);//clamp to 90%
            progressBar.value = progress; //update slider
            yield return new WaitForSeconds(Random.Range(0.9f, 1.5f));
            //yield return new WaitForSeconds(Random.Range(0.1f, 0.1f));
        }
        progressBar.value = 1f; //After loop end set progress to 100%
        yield return new WaitForSeconds(0.5f); //wait for a moment before activating the scene
        asyncLoad.allowSceneActivation = true;
    }


}