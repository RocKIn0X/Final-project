using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) InputProcess();
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

    private void InputProcess()
    {
        LayerMask layerMask = LayerMask.GetMask("Tile");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000, layerMask);
        foreach (RaycastHit hit in hits) Debug.Log(hit.collider.name);
    }
}
