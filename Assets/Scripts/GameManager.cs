using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float dayTimer;
    private int weekCount;

    void Start()
    {
        dayTimer = 0f;
        weekCount = 0;
    }

    void Update()
    {
        dayTimer += Time.deltaTime;
        if ((dayTimer/10) > weekCount)
        {
            weekCount++;
            Debug.Log(weekCount);
        }
    }
}
