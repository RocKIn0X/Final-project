using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugePanel : MonoBehaviour
{
    [System.Serializable]
    public struct GaugeData
    {
        public string gaugeName;
        public Sprite gaugeIcon;
    };

    [System.Serializable]
    public struct GaugeDictionary
    {
        public string name;
        public List<GaugeData> gaugeData;
    };

    public List<GaugeDictionary> gaugeBooks = new List<GaugeDictionary>();
    public List<UI_GaugeObject> gaugesHolder = new List<UI_GaugeObject>();

    public void SetAllGauges (int layoutType, List<double> values)
    {
        Debug.Log("Value: " + values.Count);

        for (int i = 0; i < gaugesHolder.Count; i++)
        {
            if (i < values.Count)
            {
                gaugesHolder[i].needIcon = gaugeBooks[layoutType].gaugeData[i].gaugeIcon;
                gaugesHolder[i].RefreshGauge();
                gaugesHolder[i].SetGaugePercent((float)values[i]);
                gaugesHolder[i].gameObject.SetActive(true);
            }
            else
                gaugesHolder[i].gameObject.SetActive(false);
        }
    }
}
