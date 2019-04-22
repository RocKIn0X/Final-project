using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkTile : Tile
{
    private UI_TextNotif notifManager;

    public bool isWatered;
    public GameObject overlayObj;
    public Crop crop;

    public float waterAmount;
    public float waterDecay;

    public StatusAdjustValue behaviorBook;

    private void OnEnable()
    {
        GameManager.SecondEvent += CropGrowth;
        GameManager.SecondEvent += DryWaterInTile;
    }

    private void OnDisable()
    {
        GameManager.SecondEvent -= CropGrowth;
        GameManager.SecondEvent -= DryWaterInTile;
    }

    private void Start()
    {
        InitBehaviorBook();
    }

    private void InitBehaviorBook()
    {
        behaviorBook.InitBook();
    }

    public override Vector3 pos
    {
        get
        {
            return transform.position;
        }
    }

    public override GameObject go
    {
        get
        {
            return gameObject;
        }
    }

    public override TypeTile typeTile
    {
        get
        {
            return TypeTile.WorkTile;
        }
    }

    public override List<double> info
    {
        get
        {
            return GetInfo();
        }
    }

    public override void ActionResult(int index, MonsterInteraction m)
    {
        switch(index)
        {
            case (0):
                Debug.Log("Idle");
                break;
            case (1):
                HarvestHere(m);
                break;
            case (2):
                WaterHere(m);
                break;
            case (3):
                EatHere(m);
                break;
            case (4):
                SleepHere(m);
                break;
            default:
                Debug.Log("Index ERROR");
                break;
        }

        float hungerAmount = behaviorBook.behaviorDictionary[index].hungerAmount;
        float tirenessAmount = behaviorBook.behaviorDictionary[index].tirenessAmount;
        float emotionAmount = behaviorBook.behaviorDictionary[index].emotionAmount;
        m.SetStatus(hungerAmount, tirenessAmount, emotionAmount);
    }

    public override void EatHere(MonsterInteraction m)
    {
        Debug.Log("Eat at Work tile");
        EatCrop();
    }
    public override void HarvestHere(MonsterInteraction m)
    {
        Debug.Log("Harvest at Work tile");

        HarvestCrop();
    }
    public override void PlantHere(MonsterInteraction m)
    {
        Debug.Log("Plant at Work tile");
    }
    public override void SleepHere(MonsterInteraction m)
    {
        Debug.Log("Sleep at Work tile");
    }
    public override void WaterHere(MonsterInteraction m)
    {
        Debug.Log("Water at Work tile");

        waterAmount += m.waterAmount;
        if (crop != null)
            crop.WaterCrop(waterAmount);
    }

    private void AddCrop(GameObject _crop_obj)
    {
        this.overlayObj = Instantiate(_crop_obj, this.transform);
        crop = this.overlayObj.GetComponent<Crop>();
    }

    private void HarvestCrop()
    {
        if (this.overlayObj != null && crop != null && crop.HasCrop())
        {
            float moneyReceive = crop.CalculateCost();
            if (notifManager == null)
                notifManager = (UI_TextNotif)FindObjectOfType(typeof(UI_TextNotif));
            notifManager.Notify("Got $ " + moneyReceive + "from selling ", crop.asset.cropSprite);
            PlayerManager.Instance.AddMoney(moneyReceive);
            //Destroy(this.overlayObj);
            crop = new Crop(null);
            this.overlayObj.GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    private void EatCrop()
    {
        if (this.overlayObj != null && crop != null && crop.HasCrop())
        {
            crop = new Crop(null);
            this.overlayObj.GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    private void DryWaterInTile ()
    {
        if (crop != null && !crop.HasCrop())
            waterAmount = Mathf.Clamp(waterAmount - waterDecay, 0f, 1f);
    }

    private List<double> GetInfo()
    {
        Debug.Log("Get info");
        List<double> info = new List<double>();

        if (crop.HasCrop())
        {
            info.Add(crop.GetGrowthRate());
            info.Add(waterAmount);
        }
        else
        {
            info.Add(0);
            info.Add(0);
        }

        return info;
    }

    private void CropGrowth ()
    {
        //Debug.Log("Crop growth at " + Time.time);
        if (this.overlayObj != null && crop != null)
        {
            crop.CropGrowth(ref waterAmount);
            overlayObj.GetComponent<SpriteRenderer>().sprite = crop.GetSprite();
        }
    }

    public void PlantFromPlayer(CropAssets cropAsset)
    {
        if (PlayerManager.Instance.cropAmountList.ContainsKey(cropAsset)
            && PlayerManager.Instance.cropAmountList[cropAsset] > 0
            && !crop.HasCrop())
        {
            crop = new Crop(cropAsset);
            overlayObj.GetComponent<SpriteRenderer>().sprite = crop.GetSprite();
            PlayerManager.Instance.cropAmountList[cropAsset] -= 1;
            PlayerManager.Instance.SetInventory();
        }
    }
}
