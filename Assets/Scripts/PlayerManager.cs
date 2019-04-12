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
    [SerializeField] TextMeshProUGUI playerMoneyText;
    [Header("Inventory")]
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
        //playerMoney = PlayerPrefs.HasKey("PlayerMoney") ? PlayerPrefs.GetInt("PlayerMoney") : 0;
        playerMoneyText.text = "$" + playerMoney.ToString();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) InputProcess();
    }

    public void AddMoney(float amount)
    {
        playerMoney += amount;
        playerMoneyText.text = "$" + playerMoney.ToString();
    }

    public void SetInventory()
    {
        foreach (KeyValuePair<CropAssets, int> cropAsset in cropAmountList)
        {
            Debug.Log(cropAsset.Key + ":" + cropAsset.Value);
        }
    }

    private void InputProcess()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000);
        foreach (RaycastHit hit in hits) Debug.Log(hit.collider.name);
    }
}
