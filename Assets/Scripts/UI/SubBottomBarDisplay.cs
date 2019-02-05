using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubBottomBarDisplay : UIDisplay
{
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
        showingUI = uiType;
        //Debug.Log("Show type: " + uiType);
        this.UIDisappear();
        // TODO Rearrange UI Here
        this.UIAppear();
    }
}
