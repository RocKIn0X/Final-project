using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChartBar : MonoBehaviour
{
    private RectTransform barRect;
    private Image barImage;

    public float barValue = 0f;

    public void SetHeight(float height)
    {
        if (barRect == null)
            barRect = GetComponent<RectTransform>();
        barRect.sizeDelta = new Vector2 (barRect.sizeDelta.x, height);
    }

    public void SetColor(Color barColor)
    {
        if (barImage == null)
            barImage = GetComponent<Image>();
        barImage.color = barColor;
    }
}
