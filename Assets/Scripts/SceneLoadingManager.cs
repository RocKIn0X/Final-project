using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using Cinemachine;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : MonoBehaviour
{
    static SceneLoadingManager instance;
    public static SceneLoadingManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<SceneLoadingManager>();
                if (instance == null)
                {
                    instance = new GameObject().AddComponent<SceneLoadingManager>();
                }
            }
            return instance;
        }
    }

    public bool onLoad = false;
    [SerializeField] List<PlayableAsset> playableAsset = new List<PlayableAsset>();
    [SerializeField] CanvasGroup transition_CanvasGroup;
    [SerializeField] Image loadingGaugeValue;
    private PlayableDirector fade_PlayableDirector;
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
        SetActiveTransitionCanvas(isOn: true);
        fade_PlayableDirector.playableAsset = playableAsset[0];
        fade_PlayableDirector.Play();
        while(fade_PlayableDirector.time < fade_PlayableDirector.duration)
        {
            yield return null;
        }
        loadingGaugeValue.fillAmount = 0;
        ao = SceneManager.LoadSceneAsync(sceneName);
        ao.allowSceneActivation = false;
        while (ao.progress < 0.9f)
        {
            loadingGaugeValue.fillAmount = (0.7f) * ao.progress; 
            yield return null;
        }
        loadingGaugeValue.fillAmount = 0.9f;
        ao.allowSceneActivation = true;
        while (!ao.isDone)
        {
            yield return null;
        }
        PlayerManager.Instance.InitialPlayerData();
        yield return new WaitForSeconds(1f);
        loadingGaugeValue.fillAmount = 1f;
        yield return new WaitForSeconds(1f);
        fade_PlayableDirector.playableAsset = playableAsset[1];
        fade_PlayableDirector.Play();
        SetActiveTransitionCanvas(isOn: false);
        yield return 0;
    }

    private void SetActiveTransitionCanvas(bool isOn)
    {
        transition_CanvasGroup.alpha = isOn ? 1 : 0;
        transition_CanvasGroup.blocksRaycasts = isOn;
        transition_CanvasGroup.interactable = isOn;
    }
}
