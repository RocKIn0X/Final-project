using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDict : MonoBehaviour
{
    [System.Serializable]
    public struct ActionDictionary {
        public string actionKey;
        public int actionIndex;
        public Sprite actionTarget;
    } ;

    public List<ActionDictionary> ActionBook = new List <ActionDictionary> ();

    //private static Dictionary<string, Sprite> ActualDict = new Dictionary <string, Sprite> ();
    private static Dictionary<int, Sprite> ActualDict = new Dictionary<int, Sprite>();
    private static Dictionary<int, string> NameDict = new Dictionary<int, string>();

    void PopulateDict()
    {
        foreach (ActionDictionary value in ActionBook) {
            if (value.actionTarget != null) {
                //ActualDict.Add(value.actionKey, value.actionTarget);
                ActualDict.Add(value.actionIndex, value.actionTarget);
                NameDict.Add(value.actionIndex, value.actionKey);
            }
        }
    }

    /*
    public static Sprite GetActionSprite(string key)
    {
        return ActionDict.ActualDict[key];
    }
    */

    public static Sprite GetActionSprite(int index)
    {
        return ActionDict.ActualDict[index];
    }

    public static string GetActionName(int index)
    {
        return ActionDict.NameDict[index];
    }

    // Start is called before the first frame update
    void Start()
    {
        PopulateDict();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
