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
        Debug.Log("Eat at Food tile");
    }
    public override void HarvestHere()
    {
        Debug.Log("Harvest at Food tile");
    }
    public override void PlantHere()
    {
        Debug.Log("Plant at Food tile");
    }
    public override void SleepHere()
    {
        Debug.Log("Sleep at Food tile");
    }
    public override void WaterHere()
    {
        Debug.Log("Water at Food tile");
    }
}
