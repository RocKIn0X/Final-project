using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public string playerName = "Testing01";
    public float playerMoney;
    public Image cursorImage;
    public CropAssets cropAsset_selectToPlant;

    [SerializeField] TextMeshProUGUI playerMoneyText;
    [Header("Inventory")]

    public List<InventoryItem> inventoryItemList = new List<InventoryItem>();
    public Dictionary<CropAssets, int> cropAmountList = new Dictionary<CropAssets, int>();

    private StatCollector statCollector;

    private TrainningPopup popup;

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
    private void Update()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
                {
                    if (!EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId)) InputProcess();
                }
                break;

            case RuntimePlatform.LinuxEditor:
            case RuntimePlatform.WindowsEditor:
                if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0)) InputProcess();
                break;
        }
    }

    public void AddMoney(float amount)
    {
        playerMoney += amount;
        SetMoney();

        if (statCollector == null)
            statCollector = (StatCollector)FindObjectOfType(typeof(StatCollector));
        statCollector.FinanceEarn(amount);
    }
    public bool ConsumeMoney(float amount)
    {
        if (playerMoney - amount < 0) return false;
        playerMoney -= amount;
        SetMoney();

        if (statCollector == null)
            statCollector = (StatCollector)FindObjectOfType(typeof(StatCollector));
        statCollector.FinanceSpend(amount);
        return true;
    }
    public void SetMoney()
    {
        playerMoneyText.text = "$" + playerMoney.ToString();
        DataManager.Instance.SaveData();
    }
    public void SetInventory()
    {
        int i = 0;
        foreach (KeyValuePair<CropAssets, int> cropAsset in cropAmountList)
        {
            if (cropAsset.Value > 0)
            {
                inventoryItemList[i].gameObject.SetActive(true);
                inventoryItemList[i++].SetInventory(cropAsset.Key, cropAsset.Value);
            }
        }
        for (int j = i; j < inventoryItemList.Capacity; j++)
        {
            inventoryItemList[j].gameObject.SetActive(false);
        }
        DataManager.Instance.SaveData();
    }
    private void InputProcess()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask layerMask = LayerMask.GetMask("Monster");
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000, layerMask);
        foreach (RaycastHit hit in hits)
        {
            ActionManager.instance.CallTrainningPopup();
        }
    }

    public void InitialPlayerData()
    {
        PlayerData playerData = DataManager.Instance.current_playerData;
        playerName = playerData.playerName;
        playerMoney = playerData.playerMoney;
        DataManager.Instance.ConvertLoadingData();
        SetInventory();
        SetMoney();
    }
}
