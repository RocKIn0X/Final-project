using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugePanel : MonoBehaviour
{
    [System.Serializable]
    public struct GaugeDictionary
    {
        public string name;
        public List<string> gaugeName;
    };

    public List<GaugeDictionary> gaugeBooks = new List<GaugeDictionary>();
    public List<GameObject> gaugesHolder = new List<GameObject>();

    public void SetAllGauges (int index, List<double> values)
    {
        for (int i = 0; i < gaugesHolder.Count; i++)
        {
            if (i < values.Count)
            {
                gaugesHolder[i].SetActive(true);
                gaugesHolder[i].GetComponent<GaugeAndKey>().SetGaugeAndKey(gaugeBooks[index].gaugeName[i], values[i]);
            }  
            else
                gaugesHolder[i].SetActive(false);
        }
    }
}
