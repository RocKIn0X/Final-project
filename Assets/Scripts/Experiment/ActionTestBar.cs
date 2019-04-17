using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionTestBar : MonoBehaviour
{
    public BarController workingBar;
    public BarController eattingBar;
    public BarController sleepingBar;

    public void SetActionBar (float wValue, float eValue, float sValue)
    {
        workingBar.SetActionBar(wValue);
        eattingBar.SetActionBar(eValue);
        sleepingBar.SetActionBar(sValue);
    }
}
