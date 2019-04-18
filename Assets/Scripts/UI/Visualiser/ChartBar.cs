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
        barRect.sizeDelta = new Vector2 (barRect.sizeDelta.x, height/2f);
    }

    public void SetPositive(bool isPositive)
    {
        if (barRect == null)
            barRect = GetComponent<RectTransform>();
        if (isPositive == true)
        {
            barRect.pivot = new Vector2 (0.5f, 0f);
            barRect.localScale = new Vector3 (1f, 1f, 1f);
        }
        else
        {
            barRect.pivot = new Vector2 (0.5f, 0f);
            barRect.localScale = new Vector3 (1f, -1f, 1f);
        }
    }

    public void SetColor(Color barColor)
    {
        if (barImage == null)
            barImage = GetComponent<Image>();
        barImage.color = barColor;
    }
}
