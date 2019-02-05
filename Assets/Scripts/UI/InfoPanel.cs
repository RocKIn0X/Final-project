using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : UIDisplay
{
    public Text headerText;
    private string showingUI = "";
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    override public void UIShowType(string uiType)
    {
        if (uiType == showingUI && this.gameObject.activeInHierarchy)
        {
            UIDisappear();
            return;
        }
        this.headerText.text = uiType;
        showingUI = uiType;
        //Debug.Log("Show type: " + uiType);
        this.UIDisappear();
        // TODO Rearrange UI Here
        this.UIAppear();
    }
}
