using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    CanvasGroup uiCanvasGroup;
    [SerializeField] CanvasGroup marketCanvasGroup;

    public void Start()
    {
        uiCanvasGroup = GetComponent<CanvasGroup>();
    }

    public void OpenMarketUI()
    {
        marketCanvasGroup.alpha = 1;
        marketCanvasGroup.blocksRaycasts = true;
        marketCanvasGroup.interactable = true;
    }

    public void CloseMarketUI()
    {
        marketCanvasGroup.alpha = 0;
        marketCanvasGroup.blocksRaycasts = false;
        marketCanvasGroup.interactable = false;
    }
}
