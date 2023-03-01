using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : SingletonBehaviour<SceneLoader>
{
    private bool m_isSceneLoading;

    public float LoadProgress { get; private set; }


    protected override void AfterAwake()
    {
        m_isSceneLoading = false;
        LoadProgress = 0.0f;
    }

    public void LoadScene(string sceneName)
    {
        if (m_isSceneLoading)
        {
            Debug.LogWarning("A scene is currently loading in progress. Only one scene can be loaded at once.");
            return;
        }

        m_isSceneLoading = true;
        LoadProgress = 0.0f;
        StartCoroutine(Co_LoadSceneProc(sceneName));
    }

    private IEnumerator Co_LoadSceneProc(string sceneName)
    {
        // Load the loading screen first!
        //yield return SceneManager.LoadSceneAsync("loading");

        SceneManager.LoadScene("loading");
        yield return new WaitForSeconds(1.0f);
        ScreenWiper.Instance.SetFilled(false);
        yield return new WaitForSeconds(3.0f);

        // Load the new scene in the background.
        var asyncNewScene = SceneManager.LoadSceneAsync(sceneName);

        asyncNewScene.allowSceneActivation = false;

        Debug.Log("Loading scene: " + sceneName);

        while (!asyncNewScene.isDone)
        {
            LoadProgress = Mathf.Clamp01(asyncNewScene.progress / 0.9f) * 100.0f;
            
            Debug.Log("Progress..." + LoadProgress.ToString() + " %");
            
            if (asyncNewScene.progress >= 0.9f)
            {
                yield return SceneManager.UnloadSceneAsync("loading");
                yield return new WaitForSeconds(1.0f);

                m_isSceneLoading = false;
                LoadProgress = 0.0f;
                asyncNewScene.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}