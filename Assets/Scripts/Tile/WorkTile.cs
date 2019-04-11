using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkTile : Tile
{
    public TypeTile typeTile = TypeTile.WorkTile;
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
        Debug.Log("Crop growth at " + Time.time);
        if (this.overlayObj != null && crop != null)
        {
            crop.CropGrowth();
            overlayObj.GetComponent<SpriteRenderer>().sprite = crop.GetSprite();
        }
    }
}
