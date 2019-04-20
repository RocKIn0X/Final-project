using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    CanvasGroup uiCanvasGroup;
    [SerializeField] CanvasGroup marketCanvasGroup;
    [SerializeField] CanvasGroup statCanvasGroup;

    public void Start()
    {
        uiCanvasGroup = GetComponent<CanvasGroup>();
    }

    public void OpenMarketUI()
    {
        marketCanvasGroup.alpha = 1;
        marketCanvasGroup.blocksRaycasts = true;
        marketCanvasGroup.interactable = true;
        GameManager.Instance.PauseGame();
    }

    public void OpenStatUI()
    {
        statCanvasGroup.alpha = 1;
        statCanvasGroup.blocksRaycasts = true;
        statCanvasGroup.interactable = true;
    }

    public void CloseStatUI()
    {
        statCanvasGroup.alpha = 0;
        statCanvasGroup.blocksRaycasts = false;
        statCanvasGroup.interactable = false;
    }

    public void CloseMarketUI()
    {
        marketCanvasGroup.alpha = 0;
        marketCanvasGroup.blocksRaycasts = false;
        marketCanvasGroup.interactable = false;
        GameManager.Instance.ResumeGame();
    }
}
