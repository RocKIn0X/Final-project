using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GaugeObject : MonoBehaviour
{
    public Sprite needIcon;
    public Image needImage;

    public Color ColorNeedLevelHigh ;
    public Color ColorNeedLevelMed ;
    public Color ColorNeedLevelLow ;

    public float NeedHighMargin = 66;
    public float NeedMedMargin = 25;

    public GameObject gaugeBar;
    public float gaugeMax = 240.0f;

    private float gaugePercent = 100f;


    private bool testIncrease = false;
    private void TestGaugeDisplay()
    {
        if (testIncrease == false)
            SetGaugePercent(gaugePercent - 0.1f);
        else
            SetGaugePercent(gaugePercent + 0.1f);
        if (gaugePercent <= 0.0f)
            testIncrease = true;
        else if (gaugePercent >= 100.0f)
            testIncrease = false;
    }

    public void SetGaugePercent(float percent)
    {
        // ** ----- Resize Gauge ----- ** //
        RectTransform gaugeValue = gaugeBar.GetComponent<RectTransform>();
        gaugeValue.sizeDelta = new Vector2 (percent * gaugeMax / 100f,
                                            gaugeValue.sizeDelta.y);
        Image gaugeImage = gaugeBar.GetComponent<Image>();
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
            needImage.sprite = needIcon;
            SetGaugePercent(100);
        }

        void Update()
        {
            TestGaugeDisplay();
        }
}
