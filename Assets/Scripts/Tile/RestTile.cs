using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestTile : Tile
{
    public override Vector3 pos
    {
        get
        {
            return transform.position;
        }
    }

    public override GameObject go
    {
        get
        {
            return gameObject;
        }
    }

    public override TypeTile typeTile
    {
        get
        {
            return TypeTile.RestTile;
        }
    }

    public override List<double> info
    {
        get
        {
            return null;
        }
    }

    public override void ActionResult(int index, MonsterInteraction m)
    {
        throw new System.NotImplementedException();
    }

    public override void EatHere(MonsterInteraction m)
    {
        Debug.Log("Eat at Rest tile");
    }
    public override void HarvestHere(MonsterInteraction m)
    {
        Debug.Log("Harvest at Rest tile");
    }
    public override void PlantHere(MonsterInteraction m)
    {
        Debug.Log("Plant at Rest tile");
    }
    public override void SleepHere(MonsterInteraction m)
    {
        Debug.Log("Sleep at Rest tile");
    }
    public override void WaterHere(MonsterInteraction m)
    {
        Debug.Log("Water at Rest tile");
    }
}
