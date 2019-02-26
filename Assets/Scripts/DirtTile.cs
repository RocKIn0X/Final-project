using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtTile : MonoBehaviour
{
    public bool isTestingCrop; // for testing crop logic by clicking
    public Crop crop;
    public SpriteRenderer overlay;

    // Start is called before the first frame update
    void Start()
    {
        if (isTestingCrop)
        {
            crop = new Crop(crop.asset);
            overlay.sprite = crop.GetSprite();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (crop.HasCrop())
        {
            if (crop.GetCropState() == CropState.Seed && crop.GetWaterState() == WaterState.Watered)
            {
                crop.TransitionState();
                UpdateCropSprite();
            }
            else if (crop.GetCropState() == CropState.Growing && crop.GetWaterState() == WaterState.Watered)
            {
                bool isDone = crop.IsCropDone(Time.deltaTime);
                if (isDone)
                {
                    crop.TransitionState();
                    UpdateCropSprite();
                }
                else
                {
                    // Debug.Log("Time remain: " + crop.GetTimeRemaining());
                }
            }
            else if (crop.GetCropState() == CropState.Done)
            {
                if (crop.GetCost() >= 0f)
                {
                    crop.Devalue(Time.deltaTime);
                }
            }
        }
    }

    // plant seed
    void PlantSeed (Crop c)
    {
        if (crop.HasCrop())
        {
            Debug.Log("This tile is not available now.");
            return;
        }
        Debug.Log("Planting " + c.GetName());
        crop = c;

        UpdateCropSprite();
    }

    // water crop
    void Water (int w)
    {
        if (!crop.HasCrop())
        {
            Debug.Log("No crop here!");
            return;
        }
        else
        {
            if (crop.GetWaterState() == WaterState.Dry && crop.GetCropState() != CropState.Done)
            {
                crop.WaterCrop(w);
            }
        }
    }

    // harvest
    public float HarvestCrop ()
    {
        if (!crop.HasCrop())
        {
            return 0;
        }

        if (crop.GetCropState() != CropState.Done)
        {
            return 0;
        }

        float cost = crop.GetCost();
        Debug.Log(crop.GetName() + " cost: " + cost);

        crop = new Crop(null);
        UpdateCropSprite();
        //Player.instance.AddCoin((int)cost);

        return cost;
    }

    void UpdateCropSprite ()
    {
        overlay.sprite = crop.GetSprite();
    }

    private void OnMouseDown()
    {
        if (isTestingCrop)
        {
            if (crop.GetCropState() == CropState.Seed && crop.GetWaterState() == WaterState.Dry)
            {
                Water(Random.Range(1, 5));
            }
            else if (crop.GetCropState() == CropState.Growing && crop.GetWaterState() == WaterState.Dry)
            {
                Water(Random.Range(1, 5));
            }
            else if (crop.GetCropState() == CropState.Done)
            {
                float cost = HarvestCrop();
            }
        }
    }
}
