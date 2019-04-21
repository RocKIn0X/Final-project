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

    public Image targetImage;
    public Image actionImage;

    public Sprite foodSprite;
    public Sprite bedSprite;
    public Sprite noPlantSprite;
    public Sprite blankSprite;

    // Auto-resolves GameObjects
    private StatCollector statCollector;
    private IconLibrary iconLib;
    private TileManager tileManager;

    public bool trainable = false;

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
        targetImage.sprite = targetSprite != null ? targetSprite : targetImage.sprite;
        actionImage.sprite = actionSprite != null ? actionSprite : actionImage.sprite;

        if (inputGauges == null && outputGauges == null)
        {
            trainable = false;
                return;
            }

            trainable = true;
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

    private Sprite GetTileSprite()
    {
        if (tileManager == null)
            tileManager = (TileManager)FindObjectOfType(typeof(TileManager));
        Tile targetTile = tileManager.tileTarget;

        switch (targetTile.typeTile)
        {
            case TypeTile.FoodTile :
                return foodSprite;
            case TypeTile.RestTile :
                return bedSprite;
            case TypeTile.WorkTile :
                Crop tileCrop = targetTile.gameObject.GetComponent<WorkTile>().crop;
                return tileCrop.GetSprite() == null ? noPlantSprite : tileCrop.GetSprite() ;
            default :
                return null;
        }
    }

    public void ActivatePopup (int actionIndex, List<double> states, List<double> qs)
    {
        Sprite targetSprite = GetTileSprite();
        if (actionIndex == 0) // *** Move
        {
            List<string> key = new List<string>();
            key.Add("Hunger");
            key.Add("Energy");
            key.Add("Mood");

            List<GaugeAbstract> inputGaugeData = new List<GaugeAbstract>();
            int index = 0;
            foreach (double value in states)
            {
                inputGaugeData.Add(CreatePair(key[index], ((float)states[index] + 0.5f)*100f));
                index = index + 1;
            }

            key.Clear();

            key.Add("Work");
            key.Add("Eat");
            key.Add("Sleep");

            if (iconLib == null)
                iconLib = (IconLibrary)FindObjectOfType(typeof(IconLibrary));

            List<GaugeAbstract> outputGaugeData = new List<GaugeAbstract>();
            int maxValueIndex = 0;
            index = 0;
            foreach (double value in qs)
            {
                outputGaugeData.Add(CreatePair(key[index], ((float)qs[index])*100f));
                if (qs[index] > qs[maxValueIndex])
                    maxValueIndex = index;
                index = index + 1;
            }
            Sprite actionSprite = iconLib.GetIcon(key[maxValueIndex]);
            SetLayout(inputGaugeData, outputGaugeData, targetSprite, actionSprite);
        }
        else if (actionIndex == 1) // *** Action
        {
            List<string> key = new List<string>();
            key.Add("Growth");
            key.Add("Water");

            List<GaugeAbstract> inputGaugeData = new List<GaugeAbstract>();
            int index = 0;
            foreach (double value in states)
            {
                inputGaugeData.Add(CreatePair(key[index], ((float)states[index])*100f));
                index = index + 1;
            }

            key.Clear();

            key.Add("Idle");
            key.Add("Harvest");
            key.Add("Water");

            if (iconLib == null)
                iconLib = (IconLibrary)FindObjectOfType(typeof(IconLibrary));

            List<GaugeAbstract> outputGaugeData = new List<GaugeAbstract>();
            int maxValueIndex = 0;
            index = 0;
            foreach (double value in qs)
            {
                outputGaugeData.Add(CreatePair(key[index], ((float)qs[index])*100f));
                if (qs[index] > qs[maxValueIndex])
                    maxValueIndex = index;
                index = index + 1;
            }
            Sprite actionSprite = iconLib.GetIcon(key[maxValueIndex]);
            SetLayout(inputGaugeData, outputGaugeData, targetSprite, actionSprite);
        }
    }

    public void ActivateNoTrainPopup()
    {
        List<GaugeAbstract> inputGaugeData = new List<GaugeAbstract>();

        List<GaugeAbstract> outputGaugeData = new List<GaugeAbstract>();
        SetLayout(inputGaugeData, outputGaugeData);
    }

    public void ClickPraise ()
    {
        if (trainable == false)
            return;

        if (statCollector == null)
            statCollector = (StatCollector)FindObjectOfType(typeof(StatCollector));
        statCollector.TrainReward();

        ActionManager.instance.praise();
    }

    public void ClickPunish ()
    {
        if (trainable == false)
            return;

        if (statCollector == null)
            statCollector = (StatCollector)FindObjectOfType(typeof(StatCollector));
        statCollector.TrainPunish();

        ActionManager.instance.punish();
    }

    public void ClosePanel ()
    {
        ActionManager.instance.SetTrainingPopup(false);
    }
}
