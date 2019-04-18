using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChartObject : MonoBehaviour
{
    public GameObject chartRoot;
    public ChartBar chartBarPrefab;

    private Dictionary<int, float> chartData = new Dictionary<int, float>();

    public float chartMaxHeight;
    private float dataMaxValue;

    public Color barColorPositive;
    public Color barColorNegative;

    public ChartBar AddChartBar (float value)
    {
        ChartBar bar = (ChartBar)Instantiate(chartBarPrefab, chartRoot.transform);
        bar.barValue = value;
        return bar;
    }


    public void AddValue(int key, float value)
    {
        chartData[key] = value;
        if (Mathf.Abs(value) > dataMaxValue)
            dataMaxValue = Mathf.Abs(value);
        AddChartBar(value);
        UpdateChart();
    }

    public void UpdateChart()
    {
        foreach (Transform child in chartRoot.transform)
        {
            ChartBar currentBar = child.gameObject.GetComponent<ChartBar>();
            if (currentBar != null)
            {
                float calculatedHeight = chartMaxHeight * (Mathf.Abs(currentBar.barValue) / dataMaxValue);
                currentBar.SetHeight(calculatedHeight);
                if (currentBar.barValue < 0)
                    currentBar.SetColor(barColorNegative);
                else
                    currentBar.SetColor(barColorPositive);
            }
        }
    }

    private void TestVisualise()
    {
        int MAX_TEST = 100;
        for (int index = 0 ; index < MAX_TEST ; index = index + 1)
        {
            AddValue(index, Random.Range(-255f, 255f));
        }
    }

    void Start()
    {
        TestVisualise();
    }
}
