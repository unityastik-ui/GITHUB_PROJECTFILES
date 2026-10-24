using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public Slider slider;
    public TMP_Text percentText;
    public string sceneName;

    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float timer = 0f;
        float progress = 0f;

        while (timer < 5f)
        {
            timer += Time.deltaTime;

            // Slowly increase the loading bar
            progress += Time.deltaTime / 5f;

            // Add small random pauses
            if (Random.value < 0.01f)
            {
                yield return new WaitForSeconds(0.2f);
            }

            progress = Mathf.Clamp01(progress);

            slider.value = progress;
            percentText.text = Mathf.RoundToInt(progress * 100f) + "%";

            yield return null;
        }

        // 100%
        slider.value = 1f;
        percentText.text = "100%";

        // Give the 100% a tiny moment to display
        yield return new WaitForSeconds(0.1f);

        // Open scene
        operation.allowSceneActivation = true;
    }
}