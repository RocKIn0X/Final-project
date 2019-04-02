using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum CropState
{
    Seed,
    Growing,
    Done
}

public enum WaterState
{
    Watered,
    Dry
}

public class Crop
{
    // get value from crop asset
    private float bestWaterQuantityAtPlanted;
    private float bestWaterQuantityAtGrowing;
    private float durationToDone;
    private float maximumCost;
    private float percentOfPenalty;

    private float devalueTime = 5f;
    private float maxWaterQuantity;
    private float realCost;
    private float waterAmount;

    public CropAssets asset;
    public CropState state;

    public Crop ()
    {
        InitializeValue();
    }

    public Crop (CropAssets a)
    {
        if (a != null)
        {
            asset = a;
            InitializeValue();
            GetDataFromAsset(a);
        }
        else
        {
            InitializeValue();
        }
    }

    void InitializeValue ()
    {
        state = CropState.Seed;
        maxWaterQuantity = 0f;
        realCost = 0f;
        waterAmount = 0;
    }

    void GetDataFromAsset (CropAssets asset)
    {
        bestWaterQuantityAtPlanted = asset.bestWaterQuantityAtPlanted;
        bestWaterQuantityAtGrowing = asset.bestWaterQuantityAtGrowing;
        durationToDone = asset.durationToDone;
        maximumCost = asset.maximumCost;
        maxWaterQuantity = bestWaterQuantityAtPlanted + bestWaterQuantityAtGrowing;
        percentOfPenalty = asset.percentOfPenalty / 100f;
    }

    // calculate cost of the crop
    void CalculateCost ()
    {
        float perfectCostAtState = 0;

        switch (state)
        {
            case CropState.Seed:
                perfectCostAtState = (bestWaterQuantityAtPlanted / maxWaterQuantity) * maximumCost;
                realCost += (1 - (Mathf.Abs(waterAmount - bestWaterQuantityAtPlanted) / 5)) * perfectCostAtState;
                Debug.Log("Real cost: " + realCost);
                break;
            case CropState.Growing:
                perfectCostAtState = (bestWaterQuantityAtGrowing / maxWaterQuantity) * maximumCost;
                realCost += (1 - (Mathf.Abs(waterAmount - bestWaterQuantityAtGrowing) / 5)) * perfectCostAtState;
                Debug.Log("Real cost: " + realCost);
                break;
            default:
                break;
        }

        if (realCost > maximumCost)
        {
            realCost = maximumCost;
        }

        Debug.Log(realCost + " / " + maximumCost);
    }

    // decrease cost after its state is done but monster doesn't harvest it.
    public void Devalue (float time)
    {
        devalueTime -= time;

        if (devalueTime < 0f)
        {
            Debug.Log("devalue crop cost!");
            realCost -= 2f;
            devalueTime = 5f;
        }
    }

    // transition state of the crop
    public void TransitionState ()
    {
        switch (state)
        {
            case CropState.Seed:
                CalculateCost();
                state = CropState.Growing;
                waterAmount = 0;
                break;
            case CropState.Growing:
                CalculateCost();
                state = CropState.Done;
                waterAmount = 0;
                break;
            default:
                break;
        }
    }

    // increase water quantity
    public void WaterCrop (int amount)
    {
        Debug.Log("Water quantity: " + amount);
        waterAmount += amount;

        if (waterAmount > 10)
        {
            waterAmount = 10;
        }
    }

    public bool HasCrop ()
    {
        if (asset == null)
        {
            return false;
        }
        else
        {
            return true;
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
            return true;
        }

        return false;
    }

    public float GetCost ()
    {
        if (realCost <= 2f)
        {
            return 2f;
        }

        return realCost;
    }

    public float GetTimeRemaining ()
    {
        return durationToDone;
    }

    public string GetName ()
    {
        if (asset == null)
        {
            return null;
        }

        return asset.name;
    }

    public Sprite GetSprite ()
    {
        if (asset == null)
            return null;

        switch (state)
        {
            case CropState.Seed:
                return asset.seedSprite;
            case CropState.Growing:
                return asset.growingSprite;
            case CropState.Done:
                return asset.doneSprite;
            default:
                return asset.seedSprite;
        }
    }

    public CropState GetCropState ()
    {
        return state;
    }

    public WaterState GetWaterState ()
    {
        if (waterAmount <= 0)
        {
            return WaterState.Dry;
        }
        else
        {
            return WaterState.Watered;
        }
    }
}
