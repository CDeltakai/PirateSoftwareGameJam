using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(MapLoader))]
public class StageManager : MonoBehaviour
{
    //Non-persistent singleton
    private static StageManager _instance;
    public static StageManager Instance { get { return _instance; } }

    public event Action OnFinishInitializingTiles;

    MapLoader mapLoader;


    [SerializeField] Tilemap _groundTilemap;
    public Tilemap GroundTilemap => _groundTilemap;
    [SerializeField] Tilemap _wallTilemap;
    public Tilemap WallTilemap => _wallTilemap;
    KDTree2D tileKDTree;

    public Dictionary<Vector3, GroundTileData> groundTileDictionary {get; private set;}
    //Seperate list of ground tiles (uses the ground tiles defined within groundTileDictionary) for when an indexer is required
    //in certain operations, like selecting a random tile on the board.
    public List<GroundTileData> groundTileList {get; private set;}

[Header("Debugging")]
    [Tooltip("If true, the map will be initialized from the first existing MapLayoutController found in the children of the Stage Manager" +
    "and the MapLoader will not be used. Otherwise, the MapLoader will be used to load the map.")]
    [SerializeField] bool _useExistingMap = false;
    [SerializeField] bool _groundTilesInitialized = false;
    public bool GroundTilesInitialized => _groundTilesInitialized;

    void Awake()
    {
        _instance = this;
        mapLoader = GetComponent<MapLoader>();
        _groundTilesInitialized = false;

        if(_useExistingMap)
        {
            MapLayoutController map = GetComponentInChildren<MapLayoutController>();
            if(!map)
            {
                Debug.LogError("No MapLayoutController found in the children of the Stage Manager. Please add a Map Layout Prefab to the children of StageManager in scene.");
                return;
            }
            SetTilemaps(map);
        }else
        {
            mapLoader.LoadMap();
            SetTilemaps(mapLoader.CurrentMap);
        }

        InitGroundTileData();
    }

    void Start()
    {
        
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void SetTilemaps(MapLayoutController map)
    {
        if(!map.GroundTilemap || !map.WallTilemap)
        {
            Debug.LogError("Map Layout Prefab must have both a Ground Tilemap and a Wall Tilemap.");
            return;
        }

        _groundTilemap = map.GroundTilemap;
        _wallTilemap = map.WallTilemap;

    }



    //Initializes GroundTileData by iterating through all non-empty tiles of the GroundTileMap.
    void InitGroundTileData()
    {
        //Initializing dictionary and list of groundTiles
        groundTileDictionary = new Dictionary<Vector3, GroundTileData>();
        groundTileList = new List<GroundTileData>();


        //Going through all the tile positions within the groundTileMap and initializing a GroundTileData object at each tile position
        foreach(Vector3Int tilePosition in _groundTilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int localCoords = new Vector3Int(tilePosition.x, tilePosition.y, 0);

            //If the tile position does not have a tile, continue to the next position
            if(!_groundTilemap.HasTile(localCoords)) {continue;}

            //Initialize a new GroundTileData object with variables set according to the tilePosition
            GroundTileData groundTile = new GroundTileData
            {
                localCoordinates = localCoords,
                worldPosition = _groundTilemap.CellToWorld(localCoords),
                entity = null
            };

            //Adds the new groundTile to the dictionary, allowing for quick access to the data of a tile just by giving a Vector3 as a key.
            groundTileDictionary.Add(groundTile.worldPosition, groundTile);

            //Add the same ground tile to the groundTileList, which allows for O(1) operations when using an indexer,
            //useful for random selection operations
            groundTileList.Add(groundTile);
        }

        // Initialize the KDTree2D with the keys from the dictionary
        List<Vector3> keys = new List<Vector3>(groundTileDictionary.Keys);
        tileKDTree = new KDTree2D(keys);

        _groundTilesInitialized = true;
        OnFinishInitializingTiles?.Invoke();
    }

/// <summary>
/// Returns the GroundTileData object at the given tilePosition. Returns null if the tilePosition is not a key in the dictionary.
/// </summary>
/// <param name="tilePosition"></param>
/// <returns></returns>
    public GroundTileData GetGroundTileData(Vector3Int tilePosition)
    {
        if(!groundTileDictionary.ContainsKey(tilePosition)){ return null; }
        return groundTileDictionary[_groundTilemap.CellToWorld(tilePosition)];
    }

/// <summary>
/// Finds the closest ground tile to the given inputPosition using the KDTree2D.
/// </summary>
/// <param name="inputPosition"></param>
/// <returns></returns>
    public GroundTileData FindClosestGroundTile(Vector3 inputPosition)
    {
        Vector3 closestPosition = tileKDTree.FindNearest(inputPosition);
        return groundTileDictionary[closestPosition];        
    }

    public GroundTileData FindClosestGroundTile(Vector2 inputPosition)
    {
        return FindClosestGroundTile(new Vector3(inputPosition.x, inputPosition.y, 0));
    }

/// <summary>
/// Finds the closest valid ground tile to the given inputPosition within a given radius.
/// </summary>
/// <param name="inputPosition"></param>
/// <param name="radius"></param>
/// <returns></returns>
    public GroundTileData FindClosestValidTile(Vector3 inputPosition, int radius)
    {
        GroundTileData closestTile = FindClosestGroundTile(inputPosition);
        if(CheckValidTile(closestTile.localCoordinates)){ return closestTile; }

        //If the closest tile is not valid, search for the closest valid tile within a radius
        Vector3[] nearbyTiles = tileKDTree.FindNearby(inputPosition, radius);
        foreach(Vector3 tile in nearbyTiles)
        {
            GroundTileData groundTile = groundTileDictionary[tile];
            if(CheckValidTile(groundTile.localCoordinates)){ return groundTile; }
        }

        return null;
    }


    //Checks if the given coordinates are valid based on a number conditions. Returns true if conditions are passed,
    //false otherwise.
    public bool CheckValidTile(Vector3Int tileToCheck)
    {
        GroundTileData groundTile = GetGroundTileData(tileToCheck);

        //Check conditions of the groundTile on the tileToCheck
        if(groundTile == null){ return false; }

        //Is the tileToCheck the coordinates of a wall tile? 
        if(_wallTilemap.HasTile(tileToCheck)){ return false; }

        //Is the tile to check empty air?
        if(!_groundTilemap.HasTile(tileToCheck)){ return false; }

        //Is the groundTile at the tileToCheck empty of other entities?
        if(groundTile.entity != null){ return false; }

        return true;
    }

    public bool CheckValidTile(Vector2Int tileToCheck)
    {
        return CheckValidTile(new Vector3Int(tileToCheck.x, tileToCheck.y, 0));
    }

    /// <summary>
    /// Sets the entity for the ground tile at the given tilePosition
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="tilePosition"></param>
    public void SetTileEntity(StageEntity entity, Vector3Int tilePosition)
    {
        if(GetGroundTileData(tilePosition) != null)
        {
            GetGroundTileData(tilePosition).entity = entity;
        }
    }


}
