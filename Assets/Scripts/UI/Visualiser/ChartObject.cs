using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChartObject : MonoBehaviour
{
    public GameObject chartRoot;
    public TextMeshProUGUI scoreText;
    public ChartBar chartBarPrefab;

    private Dictionary<int, float> chartData = new Dictionary<int, float>();

    public float chartMaxHeight;
    private float dataMaxValue;

    public float visualMaxProfit = 0;
    public float visualMaxDeficit = 0;
    public float latestProfit = 0;

    public Color barColorPositive;
    public Color barColorNegative;

    public ChartBar AddChartBar (float value)
    {
        ChartBar bar = (ChartBar)Instantiate(chartBarPrefab, chartRoot.transform);
        bar.barValue = value;
        return bar;
    }

    public void UpdateText()
    {
        scoreText.text = "This Month Profit:\t" + latestProfit +
            "\nHighest Profit:\t" + visualMaxProfit +
            "\nHighest Deficit:\t" + visualMaxDeficit;
    }

    public void AddValue(int key, float value)
    {
        latestProfit = value;
        chartData[key] = value;
        if (value > visualMaxProfit)
        {
            visualMaxProfit = value;
        }
        if (value < visualMaxDeficit)
        {
            visualMaxDeficit = value;
        }
        if (Mathf.Abs(value) > dataMaxValue)
            dataMaxValue = Mathf.Abs(value);
        AddChartBar(value);
        UpdateChart();
        UpdateText();
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
                {
                    currentBar.SetColor(barColorNegative);
                    currentBar.SetPositive(false);
                }
                else
                {
                    currentBar.SetColor(barColorPositive);
                    currentBar.SetPositive(true);
                }
            }
        }
    }

    private void TestVisualise()
    {
        int MAX_TEST = 100;
        for (int index = 0 ; index < MAX_TEST ; index = index + 1)
        {
            AddValue(index, Random.Range(-256f, 512f));
        }
    }

    void Start()
    {
        TestVisualise();
    }
}
