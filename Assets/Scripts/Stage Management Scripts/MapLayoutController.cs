using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapLayoutController : MonoBehaviour
{
    [SerializeField] GameObject _mapPrefab;
    public GameObject MapPrefab => _mapPrefab;

    [SerializeField] Tilemap _groundTilemap;
    public Tilemap GroundTilemap => _groundTilemap;
    [SerializeField] Tilemap _wallTilemap;    
    public Tilemap WallTilemap => _wallTilemap;

    #if UNITY_EDITOR
    // This method is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
    // This is used to ensure that the map prefab is always the same as the game object this script is attached to.
    void OnValidate()
    {
        if (PrefabUtility.IsPartOfPrefabAsset(this.gameObject))
        {
            _mapPrefab = this.gameObject;
        }
    }
    #endif    
}
