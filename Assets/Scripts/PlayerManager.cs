using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("PlayerManager").AddComponent<PlayerManager>();
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

    public float playerMoney;
    public CropAssets cropAsset_selectToPlant;
    [SerializeField] TextMeshProUGUI playerMoneyText;
    [Header("Inventory")]
    public List<InventoryItem> inventoryItemList = new List<InventoryItem>();
    public Dictionary<CropAssets, int> cropAmountList = new Dictionary<CropAssets, int>();

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
        playerMoneyText.text = "$" + playerMoney.ToString();
        SetInventory();
    }

    public void AddMoney(float amount)
    {
        playerMoney += amount;
        SetMoney();
    }

    public bool ConsumeMoney(float amount)
    {
        if (playerMoney - amount < 0) return false;
        playerMoney -= amount;
        SetMoney();
        return true;
    }

    public void SetMoney()
    {
        playerMoneyText.text = "$" + playerMoney.ToString();
    }

    public void SetInventory()
    {
        int i = 0;
        foreach (KeyValuePair<CropAssets, int> cropAsset in cropAmountList)
        {
            inventoryItemList[i].gameObject.SetActive(true);
            inventoryItemList[i++].SetInventory(cropAsset.Key, cropAsset.Value);
        }
        for (int j = i; j < inventoryItemList.Capacity; j++)
        {
            inventoryItemList[i].gameObject.SetActive(false);
        }
    }
}
