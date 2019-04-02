using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkTile : Tile
{
    public TypeTile typeTile = TypeTile.WorkTile;
    public bool isWatered;
    public GameObject crop_obj;
    public Crop crop;
    

    public override Vector3 pos
    {
        get
        {
            return transform.position;
        }
    }
    public override void EatHere()
    {
       
    }
    public override void HarvestHere()
    {
       
    }
    public override void PlantHere()
    {
        
    }
    public override void SleepHere()
    {
        
    }
    public override void WaterHere()
    {
        crop.WaterCrop(Random.Range(1, 11));
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
            PlayerManager.Instance.AddMoney(crop.GetCost());
            Destroy(this.crop_obj);
            crop = null;
            this.crop_obj = null;
        }
    }
}
