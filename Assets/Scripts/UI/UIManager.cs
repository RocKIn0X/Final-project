using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    CanvasGroup uiCanvasGroup;
    [SerializeField] CanvasGroup marketCanvasGroup;
    [SerializeField] CanvasGroup statCanvasGroup;
    [SerializeField] Image block_Panel;

    public void Start()
    {
        uiCanvasGroup = GetComponent<CanvasGroup>();
    }

    public void OpenMarketUI()
    {
        block_Panel.enabled = true;
        marketCanvasGroup.alpha = 1;
        marketCanvasGroup.blocksRaycasts = true;
        marketCanvasGroup.interactable = true;
        marketCanvasGroup.GetComponent<Animator>().SetTrigger("Active");
        SoundManager.Instance.sfxManager.PlayFromSFXObjectLibrary("OpenPanel");
        GameManager.Instance.PauseGame();
    }

    public void OpenStatUI()
    {
        block_Panel.enabled = true;
        statCanvasGroup.alpha = 1;
        statCanvasGroup.blocksRaycasts = true;
        statCanvasGroup.interactable = true;
        statCanvasGroup.GetComponent<Animator>().SetTrigger("Active");
        SoundManager.Instance.sfxManager.PlayFromSFXObjectLibrary("OpenPanel");
        GameManager.Instance.PauseGame();
    }

    public void CloseStatUI()
    {
        block_Panel.enabled = false;
        statCanvasGroup.alpha = 0;
        statCanvasGroup.blocksRaycasts = false;
        statCanvasGroup.interactable = false;
        SoundManager.Instance.sfxManager.PlayFromSFXObjectLibrary("ClosePanel");
        GameManager.Instance.ResumeGame();
    }

    public void CloseMarketUI()
    {
        block_Panel.enabled = false;
        marketCanvasGroup.alpha = 0;
        marketCanvasGroup.blocksRaycasts = false;
        marketCanvasGroup.interactable = false;
        SoundManager.Instance.sfxManager.PlayFromSFXObjectLibrary("ClosePanel");
        GameManager.Instance.ResumeGame();
    }
}
