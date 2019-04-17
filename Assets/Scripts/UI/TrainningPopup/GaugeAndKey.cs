using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GaugeAndKey : MonoBehaviour
{
    public UI_GaugeObject gaugeObj;
    public Image gaugeImage;

    public void SetGaugeAndKey (Sprite icon, double value)
    {
        gaugeImage.sprite = icon;
        gaugeObj.SetGaugePercent((float)value);
    }
}
