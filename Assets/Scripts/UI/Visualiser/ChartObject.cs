using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChartObject : MonoBehaviour
{
    public GameObject chartPositiveRegion;
    public GameObject chartNegativeRegion;

    public float BAR_WIDTH = 40f;

    private GameManager gameManager;
    public TextMeshProUGUI scoreText;
    public ChartBar chartBarPrefab;

    private Dictionary<int, float> chartData = new Dictionary<int, float>();

    public float chartMaxHeight;

    private float dataMaxValue;
    private float visualMaxValue;
    private float visualMinValue;
    private int dataLastKey = 1;

    public Color barColorPositive;
    public Color barColorNegative;

    public int dataset = 0;

    public void AddChartBar ()
    {
        ChartBar bar = (ChartBar)Instantiate(chartBarPrefab, chartPositiveRegion.transform);
        bar.GetComponent<RectTransform>().sizeDelta = new Vector2 (BAR_WIDTH, 0f);
        bar.GetComponent<Image>().color = barColorPositive;
        bar = (ChartBar)Instantiate(chartBarPrefab, chartNegativeRegion.transform);
        bar.GetComponent<RectTransform>().sizeDelta = new Vector2 (BAR_WIDTH, 0f);
        bar.GetComponent<Image>().color = barColorNegative;

    }

    public void UpdateText()
    {
        float finalValue = 0f;
        if (gameManager == null)
            gameManager = (GameManager)FindObjectOfType(typeof(GameManager));
        if (chartData.ContainsKey(gameManager.weekCount))
            finalValue = chartData[gameManager.weekCount];
        switch(dataset)
        {
            case 0:
                scoreText.text = "This Month Profit:\t" + finalValue +
                    "\nHighest Profit:\t" + visualMaxValue +
                    "\nHighest Deficit:\t" + visualMinValue;
                break;
            default :
                break;
        }
    }

    public void ScanMaxMin()
    {
        dataMaxValue = 0;
        dataLastKey = 1;
        foreach (KeyValuePair <int, float> entry in chartData)
        {
            if (entry.Key > dataLastKey)
                dataLastKey = entry.Key;
            if (Mathf.Abs(entry.Value) > dataMaxValue)
                dataMaxValue = Mathf.Abs(entry.Value);
            if (entry.Value > visualMaxValue)
                visualMaxValue = entry.Value;
            if (entry.Value < visualMinValue)
                visualMinValue = entry.Value;
        }
    }

    public void AdjustValue(int key, float value)
    {
        if (! chartData.ContainsKey(key))
        {
            chartData[key] = value;
            AddChartBar();
        }
        else
        {
            chartData[key] = chartData[key] + value;
        }
        ScanMaxMin();
        DrawChart();
        UpdateText();
    }

    public void DisplayChart()
    {
        DrawChart();
        UpdateText();
    }

    // Two-sided chart
    public void DrawChart()
    {
        for (int index = 1 ; index <= dataLastKey ; index = index + 1)
        {
            if (chartPositiveRegion.transform.childCount < index)
                AddChartBar();
            if (chartData.ContainsKey(index) == true)
            {
                Transform targetBar;
                float value = chartData[index];
                if (value >= 0)
                {
                    targetBar = chartPositiveRegion.transform.GetChild(index - 1);
                    chartNegativeRegion.transform.GetChild(index - 1).GetComponent<RectTransform>().sizeDelta =
                        new Vector2 (BAR_WIDTH, 0f);
                }
                else
                {
                    chartPositiveRegion.transform.GetChild(index - 1).GetComponent<RectTransform>().sizeDelta =
                        new Vector2 (BAR_WIDTH, 0f);
                    targetBar = chartNegativeRegion.transform.GetChild(index - 1);
                }
                targetBar.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2 (BAR_WIDTH,
                                                                                            chartMaxHeight * Mathf.Abs(value / dataMaxValue)
                                                                                            );
            }
        }
    }

    private void TestVisualise()
    {
        int MAX_TEST = 10;
        for (int index = 1 ; index <= MAX_TEST ; index = index + 1)
        {
            AdjustValue(index, Random.Range(-256f, 512f));
        }
    }

    void Start()
    {
        AdjustValue(1, 0);
        //TestVisualise();
    }
}
