﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileClass : MonoBehaviour
{
    public enum TypeTile
    {
        FoodTile, RestTile, WorkTile
    }

    public TypeTile typeTile;
    public abstract void SleepHere();
    public abstract void EatHere();
    public abstract void WaterHere();
    public abstract void HarvestHere();
    public abstract void PlantHere();
}
