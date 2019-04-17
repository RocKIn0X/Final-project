using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuyPanel : MonoBehaviour
{
    private int currentQuantity;
    private float currentPrice;
    public Image itemDisplay;
    public Text itemQuantity;
    public Text itemPrice;

    public void updateQuantity(bool isAdding)
    {
        if (isAdding == true)
        {
            DisplayItem(itemDisplay.sprite, currentPrice, currentQuantity + 1);
        }
        else if (isAdding == false && currentQuantity > 1)
        {
            DisplayItem(itemDisplay.sprite, currentPrice, currentQuantity - 1);
        }
    }

    public void DisplayItem(Sprite plantIcon, float plantCost, int plantQuantity = 1)
    {
        currentQuantity = plantQuantity;
        currentPrice = plantCost;
        itemDisplay.sprite = plantIcon;
        itemQuantity.text = plantQuantity.ToString();
        int displayPrice = Mathf.RoundToInt(plantCost * plantQuantity);
        itemPrice.text = "$ " + displayPrice.ToString();
        this.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
