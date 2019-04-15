using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkTile : Tile
{
    public bool isWatered;
    public GameObject overlayObj;
    public Crop crop;


    private void OnEnable()
    {
        GameManager.SecondEvent += CropGrowth;
    }

    private void OnDisable()
    {
        GameManager.SecondEvent -= CropGrowth;
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

    public override void ActionResult(int index, MonsterInteraction m)
    {
        if (index == 0)
        {
            // Idle
            Debug.Log("Idle");
        }
        else if (index == 1)
        {
            // Harvest
            HarvestHere(m);
        }
        else if (index == 2)
        {
            // Water
            WaterHere(m);
        }
    }

    public override void EatHere(MonsterInteraction m)
    {
        Debug.Log("Eat at Work tile");
    }
    public override void HarvestHere(MonsterInteraction m)
    {
        Debug.Log("Harvest at Work tile");
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
        //crop.WaterCrop(Random.Range(1, 11));
    }

    private void AddCrop(GameObject _crop_obj)
    {
        this.overlayObj = Instantiate(_crop_obj, this.transform);
        crop = this.overlayObj.GetComponent<Crop>();
    }

    private void HarvestCrop()
    {
        if (this.overlayObj != null && crop != null)
        {
            PlayerManager.Instance.AddMoney(crop.CalculateCost());
            //Destroy(this.overlayObj);
            crop = null;
            this.overlayObj.GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    private void CropGrowth ()
    {
        //Debug.Log("Crop growth at " + Time.time);
        if (this.overlayObj != null && crop != null)
        {
            crop.CropGrowth();
            overlayObj.GetComponent<SpriteRenderer>().sprite = crop.GetSprite();
        }
    }
    
    public void PlantFromPlayer(CropAssets cropAsset)
    {
        if (!crop.HasCrop()) crop = new Crop(cropAsset);
        overlayObj.GetComponent<SpriteRenderer>().sprite = crop.GetSprite();
    }

    public List<double> GetCropInfo ()
    {
        return crop.GetInfo();
    }
}
