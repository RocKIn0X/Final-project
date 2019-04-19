using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatCollector : MonoBehaviour
{
    private GameManager gameManager;

    public ChartObject financialChart;
    public ChartObject trainChart;

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

    public void TrainReward()
    {
        trainChart.AdjustValue(GetWeekNo(), 1);
    }

    public void TrainPunish()
    {
        trainChart.AdjustValue(-GetWeekNo(), 1);
    }
}
