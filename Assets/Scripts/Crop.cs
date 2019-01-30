using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Crop
{
    // get value from crop asset
    private int bestWaterQuantityAtPlanted;
    private int bestWaterQuantityAtGrowing;
    private float durationToDone;
    private float maximumCost;
    private float percentOfPenalty;

    private float realCost;
    private int waterQuantity;

    public CropAssets asset;
    public CropState state;

    public enum CropState
    {
        Planted,
        Growing,
        Done
    }

    public enum WaterState
    {
        Watered,
        Dry
    }


    public Crop (CropAssets a)
    {
        asset = a;
        GetDataFromAsset(a);
        state = CropState.Planted;
        realCost = 0f;
        waterQuantity = 0;
    }

    void GetDataFromAsset (CropAssets asset)
    {
        bestWaterQuantityAtPlanted = asset.bestWaterQuantityAtPlanted;
        bestWaterQuantityAtPlanted = asset.bestWaterQuantityAtGrowing;
        durationToDone = asset.durationToDone;
        maximumCost = asset.maximumCost;
        percentOfPenalty = asset.percentOfPenalty / 100f;
    }

    // transition state of the crop
    void TransitionState ()
    {
        switch (state)
        {
            case CropState.Planted:
                CalculateCost();
                state = CropState.Growing;
                waterQuantity = 0;
                break;
            case CropState.Growing:
                CalculateCost();
                waterQuantity = 0;
                break;
            default:
                break;
        }
    }

    // calculate cost of the crop
    void CalculateCost ()
    {
        switch (state)
        {
            case CropState.Planted:
                realCost += (waterQuantity / bestWaterQuantityAtPlanted) * percentOfPenalty * maximumCost;
                break;
            case CropState.Growing:
                realCost += (waterQuantity / bestWaterQuantityAtGrowing) * percentOfPenalty * maximumCost;
                break;
            default:
                break;
        }
    }

    // increase water quantity
    public void WaterCrop(int value)
    {
        switch (state)
        {
            case CropState.Planted:
                waterQuantity += value;
                TransitionState();
                break;
            case CropState.Growing:
                waterQuantity += value;
                break;
            default:
                break;
        }
    }

    public bool IsCropDone (float time)
    {
        if (GetWaterState() == WaterState.Watered)
        {
            durationToDone -= time;
        }

        if (durationToDone <= 0f)
        {
            state = CropState.Done;
            return true;
        }

        return false;
    }

    public float GetCost()
    {
        return realCost;
    }

    public Sprite GetSprite()
    {
        if (asset == null)
            return null;

        switch (state)
        {
            case CropState.Planted:
                return asset.seedSprite;
            case CropState.Growing:
                return asset.growingSprite;
            case CropState.Done:
                return asset.doneSprite;
            default:
                return asset.seedSprite;
        }
    }

    public WaterState GetWaterState()
    {
        if (waterQuantity <= 0)
        {
            return WaterState.Dry;
        }
        else
        {
            return WaterState.Watered;
        }
    }
}
