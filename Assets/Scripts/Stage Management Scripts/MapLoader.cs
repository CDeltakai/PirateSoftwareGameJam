using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    [SerializeField] GameObject mapPrefabToInstantiate;

    public MapLayoutController CurrentMap { get; private set; }

    void OnValidate()
    {
        if(mapPrefabToInstantiate && !mapPrefabToInstantiate.GetComponent<MapLayoutController>())
        {
            Debug.LogError("Map prefab must have a MapLayoutController component attached to it.");
        }
    }

    public void SetMapPrefab(GameObject mapPrefab)
    {
        if(!mapPrefab.GetComponent<MapLayoutController>())
        {
            Debug.LogError("Map prefab must have a MapLayoutController component attached to it.");
            return;
        }

        mapPrefabToInstantiate = mapPrefab;
    }

    /// <summary>
    /// Destroys the current map and instantiates the new map prefab defined in the MapLoader.
    /// WARNING: This will require the StageManager to reinitialize ground tile data and the player and NPC will also need to be reset.
    /// </summary>
    public void LoadMap()
    {
        CurrentMap = GetComponentInChildren<MapLayoutController>();
        if(CurrentMap)
        {
            Destroy(CurrentMap.gameObject);
        }

        CurrentMap = Instantiate(mapPrefabToInstantiate, transform).GetComponent<MapLayoutController>();
    }

}
