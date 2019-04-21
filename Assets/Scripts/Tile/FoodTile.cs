using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodTile : Tile
{
    public StatusAdjustValue behaviorBook;

    private void Start()
    {
        InitBehaviorBook();
    }

    private void InitBehaviorBook()
    {
        behaviorBook.InitBook();
    }

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

    public override List<double> info
    {
        get
        {
            return null;
        }
    }

    public override void ActionResult(int index, MonsterInteraction m)
    {
        if (index == 0)
        {
            EatHere(m);
        }

        float hungerAmount = behaviorBook.behaviorDictionary[index].hungerAmount;
        float tirenessAmount = behaviorBook.behaviorDictionary[index].tirenessAmount;
        float emotionAmount = behaviorBook.behaviorDictionary[index].emotionAmount;
        m.SetStatus(hungerAmount, tirenessAmount, emotionAmount);
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
