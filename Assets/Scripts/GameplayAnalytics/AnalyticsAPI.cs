using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsAPI : MonoBehaviour
{
    private static bool DEBUG_MODE = true ;
    private static string DEBUG_NAME = "AnalyticsAPI" ;
    private static void _Log(string text)
    {
        if (DEBUG_MODE == true)
        {
            Debug.Log("[" + DEBUG_NAME + "] " + text) ;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void TrackEvent(string catagory, string comment = "")
    {
        // Track something like "2019.01.11 15.40.33 : CloseStatWindow"
        // or "2019.12.31 23.59.55 : PlayEventNewYear CatStat12+32+44+22+55"
        string logText = System.DateTime.Now + " : " + catagory ;
        _Log(logText) ;
    }
}
