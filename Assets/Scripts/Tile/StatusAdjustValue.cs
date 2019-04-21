using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusAdjustValue
{
    [System.Serializable]
    public struct StatusAmount
    {
        public float hungerAmount;
        public float tirenessAmount;
        public float emotionAmount;
    }

    [System.Serializable]
    public struct StatusAndActionIndex
    {
        public string behavior;
        public int actionIndex;
        public StatusAmount statusAmount;
    }

    public List<StatusAndActionIndex> behaviorDict = new List<StatusAndActionIndex>();
    public Dictionary<int, StatusAmount> behaviorDictionary = new Dictionary<int, StatusAmount>();

    public void InitBook()
    {
        foreach (StatusAndActionIndex s in behaviorDict)
        {
            behaviorDictionary.Add(s.actionIndex, s.statusAmount);
        }
    }
}
