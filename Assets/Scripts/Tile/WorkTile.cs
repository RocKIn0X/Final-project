using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkTile : Tile
{
    public TypeTile typeTile = TypeTile.WorkTile;
    public bool isWatered;
    public GameObject crop_obj;
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
    public override void EatHere()
    {
        Debug.Log("Eat at Work tile");
    }
    public override void HarvestHere()
    {
        Debug.Log("Harvest at Work tile");
    }
    public override void PlantHere()
    {
        Debug.Log("Plant at Work tile");
    }
    public override void SleepHere()
    {
        Debug.Log("Sleep at Work tile");
    }
    public override void WaterHere()
    {
        Debug.Log("Water at Work tile");
        //crop.WaterCrop(Random.Range(1, 11));
    }

    private void AddCrop(GameObject _crop_obj)
    {
        this.crop_obj = Instantiate(_crop_obj, this.transform);
        crop = this.crop_obj.GetComponent<Crop>();
    }

    private void RemoveCrop()
    {
        if (this.crop_obj != null && crop != null)
        {
            PlayerManager.Instance.AddMoney(crop.CalculateCost());
            Destroy(this.crop_obj);
            crop = null;
            this.crop_obj = null;
        }
    }

    private void CropGrowth ()
    {
        //Debug.Log("Crop growth at " + Time.time);
        if (this.crop_obj != null && crop != null)
        {
            crop.CropGrowth();
        }
    }
}
