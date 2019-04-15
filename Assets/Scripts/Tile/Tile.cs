using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeTile
{
    FoodTile, RestTile, WorkTile
}

public abstract class Tile : MonoBehaviour
{
    public abstract GameObject go { get; }
    public abstract TypeTile typeTile { get; }
    public abstract Vector3 pos { get; }
    public abstract void SleepHere(MonsterInteraction m);
    public abstract void EatHere(MonsterInteraction m);
    public abstract void WaterHere(MonsterInteraction m);
    public abstract void HarvestHere(MonsterInteraction m);
    public abstract void PlantHere(MonsterInteraction m);
    public abstract void ActionResult(int index, MonsterInteraction m);
}
