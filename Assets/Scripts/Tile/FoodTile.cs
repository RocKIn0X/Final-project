using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodTile : Tile
{
    public TypeTile typeTile = TypeTile.FoodTile;
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

    }
}
