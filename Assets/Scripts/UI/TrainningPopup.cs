using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TrainningPopup : MonoBehaviour
{
    private static bool DEBUG_MODE = true ;
    private static string DEBUG_NAME = "TrainPopup" ;

    private static void _Log(string text)
    {
        if (DEBUG_MODE == true)
        {
            Debug.Log("[" + DEBUG_NAME + "] " + text) ;
        }
    }

    public TextMeshProUGUI title;

    public GameObject inputPanel;
    public GameObject outputPanel;

    public Sprite blankSprite;

    public struct GaugeAbstract
    {
        public string gaugeName;
        public float gaugePercent;

        public GaugeAbstract (string name, float percent=0f)
        {
            gaugeName = name;
            gaugePercent = percent;
        }
        public GaugeAbstract (string name, double percent=0)
        {
            gaugeName = name;
            gaugePercent = (float)percent;
        }
    };

    private void SetGaugeData (UI_GaugeObject gaugeObj, GaugeAbstract gaugeAbs)
    {
        _Log("Set " + gaugeAbs.gaugeName + " gauge to " + gaugeAbs.gaugePercent);
        gaugeObj.SetGaugeIcon(gaugeAbs.gaugeName);
        gaugeObj.SetGaugePercent(gaugeAbs.gaugePercent);
        gaugeObj.gameObject.SetActive(true);
    }

    public void SetLayout (List<GaugeAbstract> inputGauges,
                           List<GaugeAbstract> outputGauges,
                           Sprite targetSprite = null,
                           Sprite actionSprite = null)
    {
        int index = 0;
        foreach (Transform child in inputPanel.transform)
        {
            UI_GaugeObject childGauge = child.gameObject.GetComponent<UI_GaugeObject>();
            if (childGauge != null)
            {
                if (index < inputGauges.Count)
                {
                    SetGaugeData(childGauge, inputGauges[index]);
                }
                else
                {
                    childGauge.gameObject.SetActive(false);
                }
                index = index + 1;
            }
        }

        index = 0;
        foreach (Transform child in outputPanel.transform)
        {
            UI_GaugeObject childGauge = child.gameObject.GetComponent<UI_GaugeObject>();
            if (childGauge != null)
            {
                if (index < outputGauges.Count)
                {
                    SetGaugeData(childGauge, outputGauges[index]);
                }
                else
                {
                    childGauge.gameObject.SetActive(false);
                }
                index = index + 1;
            }
        }
    }

    private GaugeAbstract CreatePair (string iconName, float value)
    {
        return new GaugeAbstract(iconName, value);
    }

    public void ActivatePopup (int actionIndex, List<double> states, List<double> qs)
    {
        if (actionIndex == 0) // *** Move
        {
            List<GaugeAbstract> inputGaugeData = new List<GaugeAbstract>();
            inputGaugeData.Add(CreatePair("Hunger", ((float)states[0] + 0.5f)*100f));
            inputGaugeData.Add(CreatePair("Energy", ((float)states[1] + 0.5f)*100f));
            inputGaugeData.Add(CreatePair("Mood", ((float)states[2] + 0.5f)*100f));

            List<GaugeAbstract> outputGaugeData = new List<GaugeAbstract>();
            outputGaugeData.Add(CreatePair("Eat", ((float)qs[0])*100f));
            outputGaugeData.Add(CreatePair("Sleep", ((float)qs[1])*100f));
            outputGaugeData.Add(CreatePair("Work", ((float)qs[2])*100f));

            SetLayout(inputGaugeData, outputGaugeData);
        }
        else if (actionIndex == 1) // *** Action
        {
            //_Log(qs.Count.ToString());
            List<GaugeAbstract> inputGaugeData = new List<GaugeAbstract>();
            inputGaugeData.Add(CreatePair("Water", ((float)states[0])*100f));
            inputGaugeData.Add(CreatePair("Growth", ((float)states[1])*100f));

            List<GaugeAbstract> outputGaugeData = new List<GaugeAbstract>();
            //outputGaugeData.Add(CreatePair("Water", ((float)qs[0])*100f));
            //outputGaugeData.Add(CreatePair("Harvest", ((float)qs[1])*100f));
            //outputGaugeData.Add(CreatePair("Idle", ((float)qs[2])*100f));

            SetLayout(inputGaugeData, outputGaugeData);
        }
    }

    void Start()
    {
    }

    public void ClickPraise ()
    {
        ActionManager.instance.praise();
    }

    public void ClickPunish ()
    {
        ActionManager.instance.punish();
    }

    public void ClosePanel ()
    {
        ActionManager.instance.SetTrainingPopup(false);
    }
}
