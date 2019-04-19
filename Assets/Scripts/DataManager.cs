using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("DataManager").AddComponent<DataManager>();
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

    public PlayerData current_playerData = new PlayerData();
    public Dictionary<string, PlayerData> playerData_dic = new Dictionary<string, PlayerData>();

    const string folderName = "BinaryCharacterData";
    const string fileExtension = ".data";
   [SerializeField] List<CropAssets> allCropAssets = new List<CropAssets>();
    Dictionary<string, CropAssets> cropAssetsNameDic = new Dictionary<string, CropAssets>();

    void Awake()
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
        InittialData();
    }

    void InittialData()
    {
        foreach (CropAssets cropAsset in allCropAssets)
        {
            cropAssetsNameDic.Add(cropAsset.name, cropAsset);
        }
        LoadData();
    }

    public void SaveData()
    {
        SavePlayerData();
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        string dataPath = Path.Combine(folderPath, current_playerData.playerName + fileExtension);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, current_playerData);
        }
    }

    public void LoadData()
    {
        string[] filePaths = GetFilePaths();
        foreach (string filePath in filePaths)
        {
            PlayerData _playerData = LoadPlayerData(filePath);
            playerData_dic.Add(_playerData.playerName, _playerData);
        }
    }

    public PlayerData LoadPlayerData(string path)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(path, FileMode.Open))
        {
            return (PlayerData)binaryFormatter.Deserialize(fileStream);
        }
    }

    public string[] GetFilePaths()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);
        return Directory.GetFiles(folderPath, "*"+fileExtension);
    }

    public void SavePlayerData()
    {
        PlayerPrefs.SetString("RecentPlayer", current_playerData.playerName);
        current_playerData.playerMoney = PlayerManager.Instance.playerMoney;
        ConvertSavingData();
    }

    private void ConvertSavingData()
    {
        current_playerData.cropAmountNameList.Clear();
        foreach (KeyValuePair<CropAssets, int> cropAsset in PlayerManager.Instance.cropAmountList)
        {
            current_playerData.cropAmountNameList.Add(cropAsset.Key.name, cropAsset.Value);
        }
    }

    private void ConvertLoadingData()
    {
        foreach (KeyValuePair<string, int> cropAsset in current_playerData.cropAmountNameList)
        {
            PlayerManager.Instance.cropAmountList.Add(this.cropAssetsNameDic[cropAsset.Key], cropAsset.Value);
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public string playerName = "";
    public float playerMoney = 0;
    public Dictionary<string, int> cropAmountNameList = new Dictionary<string, int>();
}
