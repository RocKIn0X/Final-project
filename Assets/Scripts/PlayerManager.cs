using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] Dictionary<string, int> cropAmountList;

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

    public void AddMoney(float amount)
    {
        playerMoney += amount;
        playerMoneyText.text = "$" + playerMoney.ToString();
    }
}
