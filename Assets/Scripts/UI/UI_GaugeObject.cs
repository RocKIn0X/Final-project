using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GaugeObject : MonoBehaviour
{
    public Color ColorNeedLevelHigh ;
    public Color ColorNeedLevelMed ;
    public Color ColorNeedLevelLow ;

    public float NeedHighMargin = 66;
    public float NeedMedMargin = 25;

    public GameObject GaugeBar;
    public float GaugeMax = 240.0f;

    private float gaugePercent = 100f;

    public void SetGaugePercent(float percent)
    {
        // ** ----- Resize Gauge ----- ** //
        RectTransform gaugeValue = GaugeBar.GetComponent<RectTransform>();
        gaugeValue.sizeDelta = new Vector2 (percent * GaugeMax / 100f,
                                            gaugeValue.sizeDelta.y);
        Image gaugeImage = GaugeBar.GetComponent<Image>();
        // ** ----- Set Gauge Colour ----- ** //
        if (percent >= NeedHighMargin)
            gaugeImage.color = ColorNeedLevelHigh ;
        else if (percent >= NeedMedMargin)
            gaugeImage.color = ColorNeedLevelMed ;
        else
            gaugeImage.color = ColorNeedLevelLow ;
        // ** --- Update GaugePercent Value ----- ** //
        gaugePercent = percent;
    }

    public float GetGaugePercent()
    {
        return gaugePercent ;
    }

    void Start()
    {
        //SetGaugePercent(100);
    }

    void Update()
    {
        //SetGaugePercent(gaugePercent - 0.1f);
    }
}
