using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestTile : Tile
{
    public TypeTile typeTile = TypeTile.RestTile;
    public override Vector3 pos
    {
        get
        {
            return transform.position;
        }
    }
    public override void EatHere()
    {
        Debug.Log("Eat at Rest tile");
    }
    public override void HarvestHere()
    {
        Debug.Log("Harvest at Rest tile");
    }
    public override void PlantHere()
    {
        Debug.Log("Plant at Rest tile");
    }
    public override void SleepHere()
    {
        Debug.Log("Sleep at Rest tile");
    }
    public override void WaterHere()
    {
        Debug.Log("Water at Rest tile");
    }
}
