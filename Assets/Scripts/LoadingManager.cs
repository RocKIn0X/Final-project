using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    static LoadingManager instance;
    public static LoadingManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<LoadingManager>();
                if (instance == null)
                {
                    instance = new GameObject().AddComponent<LoadingManager>();
                }
            }
            return instance;
        }
    }

    public bool onLoad = false;

    private AsyncOperation ao;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    public IEnumerator LoadSceneCoroutine(string sceneName)
    {
        onLoad = true;
        ao = SceneManager.LoadSceneAsync(sceneName);
        ao.allowSceneActivation = false;
        while (ao.progress < 0.9f)
        {
            yield return null;
        }
        ao.allowSceneActivation = true;
        yield return 0;
    }
}
