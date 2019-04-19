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

    public Dictionary<int, float> chartData = new Dictionary<int, float>();

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
                scoreText.text = "\tFinancial Stat\t" +
                    "\nThis Month Profit:\t" + finalValue +
                    "\nHighest Profit:\t" + visualMaxValue +
                    "\nHighest Deficit:\t" + visualMinValue;
                break;
            case 1:
                scoreText.text = "\tTraining Stat\t" +
                    "\nTotal Praises:\t" + visualMaxValue +
                    "\nTotal Punishes:\t" + visualMinValue;
                break;
            default :
                break;
        }
    }

    public void ScanMaxMin()
    {
        dataMaxValue = 0;
        dataLastKey = 1;
        if (dataset == 1)
        {
            visualMaxValue = 0;
            visualMinValue = 0;
        }
        foreach (KeyValuePair <int, float> entry in chartData)
        {
            if (dataset == 0)
            {
                if (Mathf.Abs(entry.Key) > dataLastKey)
                    dataLastKey = entry.Key;
                if (Mathf.Abs(entry.Value) > dataMaxValue)
                    dataMaxValue = Mathf.Abs(entry.Value);
                if (entry.Value > visualMaxValue)
                    visualMaxValue = entry.Value;
                if (entry.Value < visualMinValue)
                    visualMinValue = entry.Value;
            }
            else if (dataset == 1)
            {
                if (Mathf.Abs(entry.Key) > dataLastKey)
                    dataLastKey = entry.Key;
                if (Mathf.Abs(entry.Value) > dataMaxValue)
                    dataMaxValue = Mathf.Abs(entry.Value);
                if (entry.Key > 0)
                    visualMaxValue = visualMaxValue + entry.Value;
                else
                    visualMinValue = visualMinValue + entry.Value;

            }
        }
    }

    public void AdjustValue(int key, float value)
    {
        if (! chartData.ContainsKey(key))
        {
            chartData[key] = value;
            if (dataset == 1)
                if (chartData.ContainsKey(-key) == false)
                    chartData[-key] = 0;
            AddChartBar();
        }
        else
        {
            chartData[key] = chartData[key] + value;
        }
        ScanMaxMin();
        DisplayChart();
    }

    public void DisplayChart()
    {
        if (dataset == 0)
            DrawChart();
        else
            DrawDualChart();
        UpdateText();
    }

    // Two Bars - Starts from Middle Zero
    public void DrawDualChart()
    {
        for (int index = 1 ; index <= dataLastKey ; index = index + 1)
        {
            if (chartPositiveRegion.transform.childCount < index)
                AddChartBar();
            if (chartData.ContainsKey(index) == true || chartData.ContainsKey(-index) == true)
            {
                Transform posBar, negBar;
                float posValue = 0f;
                float negValue = 0f;
                if (chartData.ContainsKey(index) == true)
                    posValue = chartData[index];
                if (chartData.ContainsKey(-index) == true)
                    negValue = chartData[-index];
                posBar = chartPositiveRegion.transform.GetChild(index - 1);
                negBar = chartNegativeRegion.transform.GetChild(index - 1);
                posBar.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2 (BAR_WIDTH,
                                                                                         chartMaxHeight * Mathf.Abs(posValue / dataMaxValue)
                                                                                         );
                negBar.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2 (BAR_WIDTH,
                                                                                         chartMaxHeight * Mathf.Abs(negValue / dataMaxValue)
                                                                                         );
            }
        }
    }

    // One Bar - Starts from Middle Zero
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
