using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ShopPanel : MonoBehaviour
{
    public UI_BuyPanel buyPanel;

    public void BuyingPlant(Sprite plantIcon, float plantCost)
    {
        buyPanel.DisplayItem(plantIcon, plantCost);
        Debug.Log("Showing plant");
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
