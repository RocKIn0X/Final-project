using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GaugeAndKey : MonoBehaviour
{
    public TextMeshProUGUI keyText;
    public UI_GaugeObject gaugeObj;

    public void SetGaugeAndKey (string key, double value)
    {
        keyText.text = key;
        gaugeObj.SetGaugePercent((float)value);
    }
}
