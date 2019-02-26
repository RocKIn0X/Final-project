using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileState
    {
      Plane, Tilled, Planted
    };

    public TileState tileState;

    void Start()
    {

    }

    public void PlantOnTile()
    {
        switch (tileState)
        {
            case TileState.Plane:
            case TileState.Planted:
                break;
            case TileState.Tilled:
                tileState = TileState.Planted;
                break;
        }
    }

    public void Till()
    {
        switch (tileState)
        {
            case TileState.Plane:
                tileState = TileState.Tilled;
                break;
            case TileState.Tilled:
            case TileState.Planted:
                break;
        }
    }
}
