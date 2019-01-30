using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crop", menuName = "Crop")]
public class CropAssets : ScriptableObject
{
    [Header("Best water quantity at each state (1 - 5)")]
    public int bestWaterQuantityAtPlanted;
    public int bestWaterQuantityAtGrowing;

    [Header("Paremeters")]
    public float durationToDone;
    public float percentOfPenalty;
    public float maximumCost;

    [Header("Sprite for each state")]
    public Sprite seedSprite;
    public Sprite growingSprite;
    public Sprite doneSprite;
}
