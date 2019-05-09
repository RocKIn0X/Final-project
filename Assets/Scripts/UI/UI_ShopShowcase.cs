using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShopShowcase : MonoBehaviour
{
    [Header("Customise")]
    public Sprite plantLocked;

    [Header("PriceTextColor")]
    public Color overpriceColor;
    public Color normalColor;
    public Color saleColor;
    public Color lockColor;

    [Header("GameData")]
    public CropAssets cropAsset;

    [Header("GameObjectRef")]
    public Image displayImage;
    public Text displayText;
    public Button buyButton;

    public void UpdateDisplay()
    {
        if (cropAsset != null)
        {
            displayImage.sprite = cropAsset.cropSprite;
            float priceMultiplier = cropAsset.priceMultiplier;
            displayText.text = cropAsset.name + "\n" + (cropAsset.buyingCost * priceMultiplier).ToString();
            if (priceMultiplier > 1.0f)
                displayText.color = overpriceColor;
            else if (priceMultiplier < 1.0f)
                displayText.color = saleColor;
            else
                displayText.color = normalColor;
            buyButton.interactable = true;
        }
        else
        {
            displayImage.sprite = plantLocked;
            displayText.text = "Lockberry";
            displayText.color = lockColor;
            buyButton.interactable = false;
        }

    }

    public void ActivateBuying()
    {
        MarketManager.Instance.PrepareToBuy(cropAsset);
    }

}
