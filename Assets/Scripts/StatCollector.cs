using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatCollector : MonoBehaviour
{
    private GameManager gameManager;

    public ChartObject financialChart;

    private int GetWeekNo ()
    {
        if (gameManager == null)
            gameManager = (GameManager)FindObjectOfType(typeof(GameManager));
        return gameManager.weekCount;
    }

    public void FinanceEarn(float amount)
    {
        financialChart.AdjustValue(GetWeekNo(), amount);
    }

    public void FinanceSpend(float amount)
    {
        financialChart.AdjustValue(GetWeekNo(), - amount);
    }
}
