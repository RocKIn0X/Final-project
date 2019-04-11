using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crop", menuName = "Crop")]
public class CropAssets : ScriptableObject
{
    [Header("Growth parameter")]
    public float minGrowthRate;
    public float maxGrowthRate;
    public float waterToGrowth;

    [Header("Price")]
    public float maximumCost;

    [Header("Sprite for each state")]
    public Sprite seedSprite;
    public Sprite growingSprite;
    public Sprite doneSprite;
}
