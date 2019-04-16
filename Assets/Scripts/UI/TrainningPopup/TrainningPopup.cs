using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TrainningPopup : MonoBehaviour
{
    public TextMeshProUGUI title;

    public GaugePanel inputPanel;
    public GaugePanel outputPanel;

    public void ActivatePopup (int actionIndex, List<double> states, List<double> qs)
    {
        title.text = actionIndex == 0 ? "Move state" : "Action state";

        List<double> preValueStates = new List<double>();
        List<double> preValueQS = new List<double>();

        foreach (double v in states)
        {
            if (actionIndex == 0)
                preValueStates.Add((v + 0.5f) * 100);
            else
                preValueStates.Add(v * 100);
        }
            
        foreach (double v in qs)
            preValueQS.Add(v * 100);

        inputPanel.SetAllGauges(actionIndex, preValueStates);
        outputPanel.SetAllGauges(actionIndex, preValueQS);
    }

    public void ClickPraise ()
    {
        ActionManager.instance.praise();
        GameManager.Instance.ResumeGame();
    }

    public void ClickPunish ()
    {
        ActionManager.instance.punish();
        GameManager.Instance.ResumeGame();
    }
}
