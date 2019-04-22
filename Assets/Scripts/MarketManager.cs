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
    [SerializeField] UI_BuyPanel buyPanel;
    public CropAssets buyingCropAssets;
    public int buyingQuantity = 0;
    public float totalCost = 0;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void PrepareToBuy(CropAssets _buyingCropAssets)
    {
        buyingCropAssets = _buyingCropAssets;
        buyPanel.DisplayItem(buyingCropAssets.cropSprite,
                             buyingCropAssets.buyingCost * buyingCropAssets.priceMultiplier);
    }

    public void BuyItem()
    {
        if (PlayerManager.Instance.ConsumeMoney(totalCost))
        {
            if (PlayerManager.Instance.cropAmountList.ContainsKey(buyingCropAssets)) PlayerManager.Instance.cropAmountList[buyingCropAssets] += buyingQuantity;
            else PlayerManager.Instance.cropAmountList.Add(buyingCropAssets, buyingQuantity);
            PlayerManager.Instance.SetInventory();
        }
        buyPanel.SetCanvasGroup(isOn: false);
    }
}
