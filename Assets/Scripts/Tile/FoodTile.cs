using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodTile : Tile
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
            return TypeTile.FoodTile;
        }
    }

    public override void ActionResult(int index, MonsterInteraction m)
    {
        throw new System.NotImplementedException();
    }

    public override void EatHere(MonsterInteraction m)
    {
        Debug.Log("Eat at Food tile");
    }
    public override void HarvestHere(MonsterInteraction m)
    {
        Debug.Log("Harvest at Food tile");
    }
    public override void PlantHere(MonsterInteraction m)
    {
        Debug.Log("Plant at Food tile");
    }
    public override void SleepHere(MonsterInteraction m)
    {
        Debug.Log("Sleep at Food tile");
    }
    public override void WaterHere(MonsterInteraction m)
    {
        Debug.Log("Water at Food tile");
    }
}
