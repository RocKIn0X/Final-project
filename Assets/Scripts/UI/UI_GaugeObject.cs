using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GaugeObject : MonoBehaviour
{
    private static bool DEBUG_MODE = false ;
    private static string DEBUG_NAME = "GaugeObject" ;

    private static void _Log(string text)
    {
        if (DEBUG_MODE == true)
        {
            Debug.Log("[" + DEBUG_NAME + "] " + text) ;
        }
    }

    //public Sprite needIcon;
    private Sprite needIcon;
    public string gaugeName;
    public Image needImage;

    public Color ColorNeedLevelHigh ;
    public Color ColorNeedLevelMed ;
    public Color ColorNeedLevelLow ;

    public float NeedHighMargin = 66;
    public float NeedMedMargin = 25;

    public GameObject gaugeBar;
    public float gaugeMax = 240.0f;

    public float gaugePercent = 100f;

    public bool isTesting = false;

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

    private IconLibrary iconLib;

    public void SetGaugeIcon(string key)
    {
        if (iconLib == null)
        {
            iconLib = (IconLibrary)FindObjectOfType(typeof(IconLibrary));
        }
        gaugeName = key;
        needIcon = iconLib.GetIcon(gaugeName);
        RefreshGauge();
    }

    public void RefreshGauge()
    {
        needImage.sprite = needIcon;
    }

    public void SetGaugePercent(float percent)
    {
        // ** --- Update GaugePercent Value ----- ** //
        gaugePercent = percent;
        _Log("Set " + gaugeName + " to " + gaugePercent);
        // ** ----- Resize Gauge ----- ** //
        RectTransform gaugeValue = gaugeBar.GetComponent<RectTransform>();
        gaugeValue.sizeDelta = new Vector2 (gaugePercent * gaugeMax / 100f,
                                            gaugeValue.sizeDelta.y);
        Image gaugeImage = gaugeBar.GetComponent<Image>();
        // ** ----- Set Gauge Colour ----- ** //
        if (gaugePercent >= NeedHighMargin)
            gaugeImage.color = ColorNeedLevelHigh ;
        else if (gaugePercent >= NeedMedMargin)
            gaugeImage.color = ColorNeedLevelMed ;
        else
            gaugeImage.color = ColorNeedLevelLow ;
    }

    public float GetGaugePercent()
    {
        return gaugePercent ;
    }

    void Start()
    {
        if (gaugeName != null)
        {
            SetGaugeIcon(gaugeName);
        }
    }

    void Update()
    {
        if (isTesting == true)
            TestGaugeDisplay();
    }
}
