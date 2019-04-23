using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestTile : Tile
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
        switch (index)
        {
            case (0):
                Debug.Log("Idle");
                break;
            case (1):
                HarvestHere(m);
                break;
            case (2):
                WaterHere(m);
                break;
            case (3):
                EatHere(m);
                break;
            case (4):
                SleepHere(m);
                break;
            default:
                Debug.Log("Index ERROR");
                break;
        }

        float hungerAmount = behaviorBook.behaviorDictionary[index].hungerAmount;
        float tirenessAmount = behaviorBook.behaviorDictionary[index].tirenessAmount;
        float emotionAmount = behaviorBook.behaviorDictionary[index].emotionAmount;
        m.SetStatus(hungerAmount, tirenessAmount, emotionAmount);
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
