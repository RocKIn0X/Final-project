using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDict : MonoBehaviour
{
    [System.Serializable]
    public struct ActionDictionary {
        public string actionKey;
        public int actionIndex;
    } ;

    public List<ActionDictionary> ActionBook = new List <ActionDictionary> ();

    private static Dictionary<int, string> NameDict = new Dictionary<int, string>();

    private static IconLibrary iconLib;

    void PopulateDict()
    {
        foreach (ActionDictionary value in ActionBook) {
            if (value.actionKey != null && value.actionIndex != null) {
                NameDict.Add(value.actionIndex, value.actionKey);
            }
        }
    }

    public static string GetActionName(int index)
    {
        return ActionDict.NameDict[index];
    }

    void Start()
    {
        PopulateDict();
    }
}
