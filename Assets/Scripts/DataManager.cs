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

    const string folderName = "BinaryCharacterData";
    const string fileExtension = ".data";
   [SerializeField] List<CropAssets> allCropAssets = new List<CropAssets>();
    Dictionary<string, CropAssets> cropAssetsNameDic = new Dictionary<string, CropAssets>();
    public PlayerData playerData = new PlayerData();
    public List<string> filePaths;

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
        LoadData();
    }

    void InittialData()
    {
        foreach (CropAssets cropAsset in allCropAssets)
        {
            cropAssetsNameDic.Add(cropAsset.name, cropAsset);
        }
    }

    public void SaveData()
    {
        SavePlayerData();
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        string dataPath = Path.Combine(folderPath, PlayerManager.Instance.playerName + fileExtension);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, playerData);
        }
    }

    public void LoadData()
    {
        filePaths = new List<string>(GetFilePaths());

        //if (filePaths.Length > 0)
        //{
        //    playerData = LoadPlayerData(filePaths[0]);
        //    PlayerManager.Instance.playerMoney = playerData.playerMoney;
        //    ConvertLoadingData();
        //}
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

        return Directory.GetFiles(folderPath, fileExtension);
    }

    public void SavePlayerData()
    {
        playerData.playerMoney = PlayerManager.Instance.playerMoney;
        ConvertSavingData();
    }

    private void ConvertSavingData()
    {
        playerData.cropAmountNameList.Clear();
        foreach (KeyValuePair<CropAssets, int> cropAsset in PlayerManager.Instance.cropAmountList)
        {
            playerData.cropAmountNameList.Add(cropAsset.Key.name, cropAsset.Value);
        }
    }

    private void ConvertLoadingData()
    {
        foreach (KeyValuePair<string, int> cropAsset in playerData.cropAmountNameList)
        {
            PlayerManager.Instance.cropAmountList.Add(this.cropAssetsNameDic[cropAsset.Key], cropAsset.Value);
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public float playerMoney;
    public Dictionary<string, int> cropAmountNameList = new Dictionary<string, int>();
}
