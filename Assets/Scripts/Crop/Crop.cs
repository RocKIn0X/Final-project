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

[System.Serializable]
public class Crop
{
    // get value from crop asset
    private float minGrowthRate;
    private float maxGrowthRate;
    private float waterToGrowth;
    private float maximumCost;

    public CropAssets asset;
    public CropState state;

    [SerializeField]
    private float growthRate;
    [SerializeField]
    private float realCost;

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
        growthRate = 0f;
        realCost = 0f;
    }

    void GetDataFromAsset (CropAssets asset)
    {
        minGrowthRate = asset.minGrowthRate;
        maxGrowthRate = asset.maxGrowthRate;
        waterToGrowth = asset.waterToGrowth;
        maximumCost = asset.maximumSellingCost;
    }

    // transition state of the crop
    void TransitionState ()
    {
        switch (state)
        {
            case CropState.Seed:
                state = CropState.Growing;
                break;
            case CropState.Growing:
                state = CropState.Done;
                break;
            default:
                break;
        }
    }

    bool isChangeState()
    {
        if (state == CropState.Seed && growthRate > 0.3f) return true;
        else if (state == CropState.Growing && growthRate > 0.8f) return true;
        else return false;
    }

    // Run every 1 second (Link to TimeManager)
    public void CropGrowth(ref float waterAmount)
    {
        float waterConsumed = waterAmount >= waterToGrowth ? 1f : waterAmount / waterToGrowth;
        float growthValue = minGrowthRate + maxGrowthRate * waterConsumed;

        growthRate = Mathf.Clamp(growthRate + growthValue, 0f, 1f);
        waterAmount = Mathf.Clamp(waterAmount - waterToGrowth, 0f, 1f);
        realCost = maximumCost * growthRate;

        if (isChangeState())
        {
            TransitionState();
        }
    }

    // increase water guage
    public void WaterCrop(float amount)
    {

    }

    // calculate cost of the crop
    public float CalculateCost()
    {
        return realCost * asset.priceMultiplier;
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

    public double GetGrowthRate ()
    {
        if (HasCrop())
        {
            return (double)growthRate;
        }

        return 0;
    }
}
