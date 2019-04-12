using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileManager : MonoBehaviour
{
    private static TileManager instance;
    public static TileManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("TileManager").AddComponent<TileManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }

        private set
        {
            if (instance != null && instance != value)
            {
                Destroy(instance.gameObject);
            }
            instance = value;
        }
    }

    public Tile[] tiles_arr;
    public WorkTile[] workTile_arr;
    public RestTile[] restTile_arr;
    public FoodTile[] foodTile_arr;

    private int prevWorkIndex = -1;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        tiles_arr = Object.FindObjectsOfType<Tile>();
        workTile_arr = Object.FindObjectsOfType<WorkTile>();
        restTile_arr = Object.FindObjectsOfType<RestTile>();
        foodTile_arr = Object.FindObjectsOfType<FoodTile>();
    }

    public Tile[] tiles
    {
        get
        {
            return tiles_arr;
        }
    }
    public WorkTile[] workTiles
    {
        get
        {
            return workTile_arr;
        }
    }
    public FoodTile[] foodTiles
    {
        get
        {
            return foodTile_arr;
        }
    }

    public RestTile[] restTiles
    {
        get
        {
            return restTile_arr;
        }
    }

    public Tile GetTile (int index)
    {
        if (index == 0) return GetWorkTile();
        else if (index == 1) return GetFoodTile();
        else if (index == 2) return GetRestTile();

        return null;
    }

    private Tile GetWorkTile ()
    {
        int index = 0;

        if (prevWorkIndex == -1)
        {
            index = Random.Range(0, workTiles.Length);
        }            
        else
        {
            do
            {
                index = Random.Range(0, workTiles.Length);
            }
            while (index == prevWorkIndex);
        }

        prevWorkIndex = index;

        return workTiles[index];
    }

    private Tile GetFoodTile ()
    {
        return foodTiles[0];
    }

    private Tile GetRestTile ()
    {
        return restTiles[0];
    }

    private void InputProcess()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000);
        foreach (RaycastHit hit in hits) Debug.Log(hit.collider.name);
    }
}
