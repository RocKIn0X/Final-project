using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconLibrary : MonoBehaviour
{
    private bool isInitialised = false;

    private static bool DEBUG_MODE = true ;
    private static string DEBUG_NAME = "IconLibrary" ;

    private static void _Log(string text)
    {
        if (DEBUG_MODE == true)
        {
            Debug.Log("[" + DEBUG_NAME + "] " + text) ;
        }
    }

    [System.Serializable]
    public struct FakeIconDict
    {
        public string iconKey;
        public Sprite iconSprite;
    };
    public List<FakeIconDict> iconDictionary;
    public Dictionary<string, Sprite> iconDict = new Dictionary<string, Sprite>();

    public Sprite fallbackIcon;

    public Sprite GetIcon(string key)
    {
        if (isInitialised == false)
        {
            InitLibrary();
        }
        if (key == null || key == "")
        {
            _Log("Got empty key");
            return fallbackIcon;
        }
        Sprite resultIcon = null;
        try
        {
            resultIcon = iconDict[key];
        }
        catch
        {
            _Log("Icon not found: " + key);
        }
        if (resultIcon != null)
            return resultIcon;
        else
            return fallbackIcon;
    }

    void InitLibrary()
    {
        foreach (FakeIconDict entry in iconDictionary)
        {
            iconDict[entry.iconKey] = entry.iconSprite;
        }
        isInitialised = true;
    }

    void Start()
    {
        InitLibrary();
    }
}
