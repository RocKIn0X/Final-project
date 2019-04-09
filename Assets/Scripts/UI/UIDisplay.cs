using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDisplay : MonoBehaviour
{
    public static string displayType = "";

    void Start()
    {

    }

    void Update()
    {

    }

    public virtual void UIAppear()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void UIDisappear()
    {
        displayType = "";
        this.gameObject.SetActive(false);
    }

    public virtual void UIToggle()
    {
        if (this.gameObject.activeInHierarchy)
        {
            this.UIDisappear();
        }
        else
        {
            this.UIAppear();
        }
    }

    public virtual void UIShowType(string uiType)
    {

        if (uiType == displayType && this.gameObject.activeInHierarchy)
        {
            UIDisappear();
            return;
        }
        displayType = uiType;
        this.UIDisappear();
        // TODO Rearrange UI Here
        this.UIAppear();
    }
}
