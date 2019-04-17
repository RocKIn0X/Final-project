using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarketManager : MonoBehaviour
{

    private static MarketManager instance;
    public static MarketManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("MarketManager").AddComponent<MarketManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }

        private set
        {
            if (instance != null && instance != value)
            {
                Destroy(instance.gameObject);
            }
            instance = value;
        }
    }
    [SerializeField] CanvasGroup buyConfirmPopupGroup;
    [SerializeField] TextMeshProUGUI buyingItemAmountText;
    [SerializeField] TextMeshProUGUI totalCostText;
    public CropAssets buyingCropAssets;
    public int buyingItemAmount = 0;
    public float totalCost = 0;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void PrepareToBuy(CropAssets _buyingCropAssets)
    {
        buyingCropAssets = _buyingCropAssets;
        buyingItemAmount = 0;
        SetConfirmPopup(isOn: true);
        IncreaseBuyingAmount();
    }

    public void BuyItem()
    {
        if (PlayerManager.Instance.ConsumeMoney(totalCost))
        {
            if (PlayerManager.Instance.cropAmountList.ContainsKey(buyingCropAssets)) PlayerManager.Instance.cropAmountList[buyingCropAssets] += buyingItemAmount;
            else PlayerManager.Instance.cropAmountList.Add(buyingCropAssets, buyingItemAmount);
            PlayerManager.Instance.SetInventory();
        }
        SetConfirmPopup(isOn: false);
    }

    public void IncreaseBuyingAmount()
    {
        buyingItemAmount += 1;
        totalCost = buyingCropAssets.buyingCost * buyingItemAmount;
        SetConfirmPopupText();
    }

    public void DecreaseBuyingAmount()
    {
        buyingItemAmount = buyingItemAmount -= 1 > 1 ? buyingItemAmount - 1 : 1;
        totalCost = buyingCropAssets.buyingCost * buyingItemAmount;
        SetConfirmPopupText();
    }

    public void SetConfirmPopupText()
    {
        buyingItemAmountText.text = buyingItemAmount.ToString();
        totalCostText.text = totalCost.ToString();
    }

    public void SetConfirmPopup(bool isOn)
    {
        buyConfirmPopupGroup.alpha = isOn ? 1 : 0;
        buyConfirmPopupGroup.blocksRaycasts = isOn;
        buyConfirmPopupGroup.interactable = isOn;
    }
}
