using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionTestBar : MonoBehaviour
{
    public Slider workingBar;
    public Slider eattingBar;
    public Slider sleepingBar;

    public void SetActionBar (float wValue, float eValue, float sValue)
    {
        workingBar.value = wValue + 0.5f;
        eattingBar.value = eValue + 0.5f;
        sleepingBar.value = sValue + 0.5f;
    }
}
