using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public CropAssets buyingCropAssets;
    public int itemAmountToBuy = 0;
    public float totalCostToPay = 0;

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
        buyConfirmPopupGroup.alpha = 1;
        buyConfirmPopupGroup.blocksRaycasts = true;
        buyConfirmPopupGroup.interactable = true;
        itemAmountToBuy = 1;
        totalCostToPay = buyingCropAssets.buyingCost * itemAmountToBuy;
    }

    public void BuyItem()
    {
        if (PlayerManager.Instance.ConsumeMoney(totalCostToPay))
        {
            if (PlayerManager.Instance.cropAmountList.ContainsKey(buyingCropAssets)) PlayerManager.Instance.cropAmountList[buyingCropAssets] += itemAmountToBuy;
            else PlayerManager.Instance.cropAmountList.Add(buyingCropAssets, itemAmountToBuy);
            PlayerManager.Instance.SetInventory();
        }
    }
}
