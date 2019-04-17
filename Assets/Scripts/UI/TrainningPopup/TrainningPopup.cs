using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TrainningPopup : MonoBehaviour
{
    public TextMeshProUGUI title;

    public GameObject inputPanel;
    public GameObject outputPanel;

    public Sprite blankSprite;

    public struct GaugeAbstract
    {
        public Sprite gaugeIcon;
        public float gaugePercent;

        public GaugeAbstract(Sprite icon, float percent)
        {
            gaugeIcon = icon;
            gaugePercent = percent;
        }
    };

    [System.Serializable]
    public struct FakeIconDict
    {
        public string iconKey;
        public Sprite iconSprite;
    };
    public List<FakeIconDict> iconDictionary;
    public Dictionary<string, Sprite> iconDict = new Dictionary<string, Sprite>();

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
                    childGauge.needIcon = inputGauges[index].gaugeIcon;
                    childGauge.SetGaugePercent(inputGauges[index].gaugePercent);
                    childGauge.RefreshGauge();
                    childGauge.gameObject.SetActive(true);
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
                    childGauge.needIcon = outputGauges[index].gaugeIcon;
                    childGauge.SetGaugePercent(outputGauges[index].gaugePercent);
                    childGauge.RefreshGauge();
                    childGauge.gameObject.SetActive(true);
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
        Sprite targetIcon = null;
        try
        {
            targetIcon = iconDict[iconName];
        }
        catch
        {
            Debug.Log("Key not found in iconDict: " + iconName);
        }
        if (targetIcon == null)
            return new GaugeAbstract(blankSprite, value);
        return new GaugeAbstract(targetIcon, value);
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
            outputGaugeData.Add(CreatePair("Food", ((float)qs[0])*100f));
            outputGaugeData.Add(CreatePair("Bed", ((float)qs[1])*100f));
            outputGaugeData.Add(CreatePair("Work", ((float)qs[2])*100f));

            SetLayout(inputGaugeData, outputGaugeData);
        }
        else if (actionIndex == 1) // *** Action
        {
            List<GaugeAbstract> inputGaugeData = new List<GaugeAbstract>();
            inputGaugeData.Add(CreatePair("Water", ((float)states[0])*100f));
            inputGaugeData.Add(CreatePair("Growth", ((float)states[1])*100f));

            List<GaugeAbstract> outputGaugeData = new List<GaugeAbstract>();
            outputGaugeData.Add(CreatePair("Water", ((float)qs[0])*100f));
            outputGaugeData.Add(CreatePair("Harvest", ((float)qs[1])*100f));
            outputGaugeData.Add(CreatePair("Idle", ((float)qs[2])*100f));

            SetLayout(inputGaugeData, outputGaugeData);
        }
        ////////////////////////////////////////////////////////////////////
        // title.text = actionIndex == 0 ? "Move state" : "Action state"; //
        //                                                                //
        // List<double> preValueStates = new List<double>();              //
        // List<double> preValueQS = new List<double>();                  //
        //                                                                //
        // foreach (double v in states)                                   //
        // {                                                              //
        //     if (actionIndex == 0)                                      //
        //         preValueStates.Add((v + 0.5f) * 100);                  //
        //     else                                                       //
        //         preValueStates.Add(v * 100);                           //
        // }                                                              //
        //                                                                //
        // foreach (double v in qs)                                       //
        //     preValueQS.Add(v * 100);                                   //
        ////////////////////////////////////////////////////////////////////



        //inputPanel.SetAllGauges(actionIndex, preValueStates);
        //outputPanel.SetAllGauges(actionIndex, preValueQS);
    }

    void Start()
    {
        foreach (FakeIconDict entry in iconDictionary)
        {
            iconDict[entry.iconKey] = entry.iconSprite;
        }
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
