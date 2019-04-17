using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarController : MonoBehaviour
{
    public TextMeshProUGUI valueText;

    private Slider actionBar;

    // Start is called before the first frame update
    void Start()
    {
        actionBar = GetComponent<Slider>();
    }

    public void SetActionBar (float value)
    {
        actionBar.value = value + 0.5f;
        valueText.text = value.ToString("F4");
    }
}
