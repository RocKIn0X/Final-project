using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDict : MonoBehaviour
{
    [System.Serializable]
    public struct ActionDictionary {
        public string actionKey;
        public Sprite actionTarget;
    } ;

    public List<ActionDictionary> ActionBook = new List <ActionDictionary> ();

    private static Dictionary<string, Sprite> ActualDict = new Dictionary <string, Sprite> ();

    void PopulateDict()
    {
        foreach (ActionDictionary value in ActionBook) {
            if (value.actionTarget != null) {
                ActualDict.Add(value.actionKey, value.actionTarget);
            }
        }
    }

    public static Sprite GetActionSprite(string key)
    {
        return ActionDict.ActualDict[key];
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
