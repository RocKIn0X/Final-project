using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using Cinemachine;
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
    [SerializeField] Image fogFill_image;
    [SerializeField] List<PlayableAsset> playableAsset = new List<PlayableAsset>();
    private PlayableDirector fade_PlayableDirector;
    //Animator[] fog_animators;
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
        fade_PlayableDirector = this.GetComponent<PlayableDirector>();
    }
    
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    public IEnumerator LoadSceneCoroutine(string sceneName)
    {
        onLoad = true;
        fade_PlayableDirector.playableAsset = playableAsset[0];
        fade_PlayableDirector.Play();
        while(fade_PlayableDirector.time < fade_PlayableDirector.duration)
        {
            yield return null;
        }
        ao = SceneManager.LoadSceneAsync(sceneName);
        ao.allowSceneActivation = false;
        while (ao.progress < 0.9f)
        {
            yield return null;
        }
        ao.allowSceneActivation = true;
        while (!ao.isDone)
        {
            yield return null;
        }
        fade_PlayableDirector.playableAsset = playableAsset[1];
        fade_PlayableDirector.Play();
        yield return 0;
    }
}
