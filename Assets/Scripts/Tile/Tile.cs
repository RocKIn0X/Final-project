using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public enum TypeTile
    {
        FoodTile, RestTile, WorkTile
    }
    public abstract Vector3 pos { get; }
    public abstract void SleepHere();
    public abstract void EatHere();
    public abstract void WaterHere();
    public abstract void HarvestHere();
    public abstract void PlantHere();
}
