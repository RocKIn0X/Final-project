using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBubble : MonoBehaviour
{
    private static bool DEBUG_MODE = true ;
    private static string DEBUG_NAME = "ActionBubble" ;

    public float BUBBLE_TIMEOUT_SEC = 3.0f;

    private static void _Log(string text)
    {
        if (DEBUG_MODE == true)
        {
            Debug.Log("[" + DEBUG_NAME + "] " + text) ;
        }
    }

    public GameObject actionSprite;
    SpriteRenderer actionRenderer;
    SpriteRenderer bubbleRenderer;
    private IEnumerator waitTimeout;
    private IconLibrary iconLib;

    public IEnumerator WaitAndDisappear(float timeout)
    {
        yield return new WaitForSeconds(timeout);
        this.Disappear();
    }

    public void Disappear()
    {
        bubbleRenderer = this.GetComponent<SpriteRenderer>();
        bubbleRenderer.color = Color.clear;
        actionRenderer = actionSprite.GetComponent<SpriteRenderer>();
        actionRenderer.color = Color.clear;
    }

    public void ShowAction(int actionIndex)
    {
        string key = ActionDict.GetActionName(actionIndex);
        _Log("Showing action: " + key);

        bubbleRenderer = this.GetComponent<SpriteRenderer>();
        bubbleRenderer.color = Color.white;
        actionRenderer = actionSprite.GetComponent<SpriteRenderer>();
        actionRenderer.color = Color.white;
        if (iconLib == null)
            iconLib = (IconLibrary)FindObjectOfType(typeof(IconLibrary));
        actionRenderer.sprite = iconLib.GetIcon(key);
        //waitTimeout = WaitAndDisappear(BUBBLE_TIMEOUT_SEC);
        //StartCoroutine(waitTimeout);
    }
}
