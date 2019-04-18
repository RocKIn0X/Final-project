using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShopShowcase : MonoBehaviour
{
    public Sprite plantLocked;
    public CropAssets cropAsset;

    public Image displayImage;
    public Text displayText;
    public Button buyButton;

    public void UpdateDisplay()
    {
        if (cropAsset != null)
        {
            displayImage.sprite = cropAsset.cropSprite;
            displayText.text = cropAsset.name + "\n" + cropAsset.buyingCost.ToString();
            buyButton.interactable = true;
        }
        else
        {
            displayImage.sprite = plantLocked;
            displayText.text = "Lockberry";
            buyButton.interactable = false;
        }

    }

    public void ActivateBuying()
    {
        MarketManager.Instance.PrepareToBuy(cropAsset);
    }

}
