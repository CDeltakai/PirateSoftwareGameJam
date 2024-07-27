using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GroundTileData
{
    //The coordinates of this tile on the tilemap
    internal Vector3Int localCoordinates{get;set;}
    //The position of this tile on the world
    internal Vector3 worldPosition{get;set;}
    //The entity that is currently occupying this tile
    internal StageEntity entity{get;set;}

}
