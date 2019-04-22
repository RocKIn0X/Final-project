using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuyPanel : MonoBehaviour
{
    [SerializeField]CanvasGroup buyPanel_canvasGroup;
    private int currentQuantity;
    private float currentPrice;
    private PlayerManager playerManager;
    private float currentMoney;
    public Image itemDisplay;
    public Text itemQuantity;
    public Text itemPrice;

    public Color priceEnoughColor;
    public Color priceOverColor;

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
        if (playerManager == null)
            playerManager = (PlayerManager)FindObjectOfType(typeof(PlayerManager));
        currentMoney = playerManager.playerMoney;
        itemPrice.text = "$ " + displayPrice.ToString();
        if (displayPrice > currentMoney)
            itemPrice.color = priceOverColor;
        else
            itemPrice.color = priceEnoughColor;
        MarketManager.Instance.buyingQuantity = currentQuantity;
        MarketManager.Instance.totalCost = displayPrice;
        SetCanvasGroup(isOn: true);
    }

    public void SetCanvasGroup(bool isOn)
    {
        buyPanel_canvasGroup.alpha = isOn ? 1 : 0;
        buyPanel_canvasGroup.blocksRaycasts = isOn;
        buyPanel_canvasGroup.interactable = isOn;
        if (isOn) buyPanel_canvasGroup.GetComponent<Animator>().SetTrigger("Active");
    }
}
