using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private float dayTimer;
    private int weekCount;
    [SerializeField] TextMeshProUGUI weekText;

    void Start()
    {
        dayTimer = 0f;
        weekCount = 0;
        SetWeekText();
    }

    void Update()
    {
        dayTimer += Time.deltaTime;
        if ((dayTimer/10) > weekCount)
        {
            weekCount++;
            SetWeekText();
        }
    }

    void SetWeekText()
    {
        weekText.text = "WEEK : " + weekCount.ToString();
    }

}
