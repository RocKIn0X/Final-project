using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AnalyticsAPI.TrackEvent("Red");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
