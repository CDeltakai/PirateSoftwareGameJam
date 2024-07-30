using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MeleeEnemyController : MonoBehaviour
{
    public enum State
    {
        Idle,
        Moving,
        Attacking
    }

    [SerializeField] StageEntity stageEntity;

    public Transform playerTransform;

    private Vector3Int currentTile;
    private Vector3Int targetTile;
    private Queue<Vector3Int> path;
    private StageManager stageManager;

    [SerializeField] State _currentState = State.Idle;

    [Header("Debug")]
    [SerializeField] bool testPathfinding = false; 
    [SerializeField] bool forceMoveAlongPath = false;
    [SerializeField] int pathCount = 0;

    void Start()
    {
        stageManager = StageManager.Instance;

        currentTile = stageManager.GroundTilemap.WorldToCell(transform.position);
        targetTile = stageManager.GroundTilemap.WorldToCell(playerTransform.position);
        path = new Queue<Vector3Int>();
    }

    void Update()
    {
        if (testPathfinding)
        {
            testPathfinding = false;
            CheckState();
        }

        if (forceMoveAlongPath)
        {
            forceMoveAlongPath = false;
            MoveAlongPath();
        }

    }

    void CheckState()
    {
        Pathfind();
        if (path.Count > 0 && path.Peek() == currentTile)
        {
            path.Dequeue(); // Remove the first tile from the path, as it is the current tile
        }

        if (IsAdjacent(currentTile, stageManager.GroundTilemap.WorldToCell(playerTransform.position)))
        {
            _currentState = State.Attacking;
        }
        else if (path.Count > 0)
        {
            _currentState = State.Moving;
        }
        else
        {
            _currentState = State.Idle;
        }

        PerformAction();
    }

    bool IsAdjacent(Vector3Int tile1, Vector3Int tile2)
    {
        int distanceX = Mathf.Abs(tile1.x - tile2.x);
        int distanceY = Mathf.Abs(tile1.y - tile2.y);

        return (distanceX == 1 && distanceY == 0) || (distanceX == 0 && distanceY == 1);
    }

    void PerformAction()
    {
        switch (_currentState)
        {
            case State.Idle:
                break;
            case State.Moving:
                MoveAlongPath();
                break;
            case State.Attacking:
                Attack();
                break;
        }
    }

    void Attack()
    {
        // Implement attack logic here
    }

    void Pathfind()
    {
        targetTile = FindValidAdjacentTile(stageManager.GroundTilemap.WorldToCell(playerTransform.position));
        if (path.Count == 0 || path.Peek() != targetTile)
        {
            FindPathToTarget();
        }
    }

    private void FindPathToTarget()
    {
        currentTile = stageEntity.tilePosition;
        targetTile = FindReachableAdjacentTile(stageManager.GroundTilemap.WorldToCell(playerTransform.position));


        Debug.Log($"Finding path from {currentTile} to {targetTile}");

        List<Vector3Int> newPath = Pathfinding.FindPath(currentTile, targetTile, stageManager);
        if (newPath != null)
        {
            path = new Queue<Vector3Int>(newPath);
            Debug.Log("Path found. Path count: " + path.Count);
        }
        else
        {
            Debug.Log("No path found.");
        }
        pathCount = path.Count;
    }

    private Vector3Int FindValidAdjacentTile(Vector3Int playerTile)
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>
        {
            playerTile + Vector3Int.up,
            playerTile + Vector3Int.down,
            playerTile + Vector3Int.left,
            playerTile + Vector3Int.right
        };

        foreach (var tile in adjacentTiles)
        {
            if (stageManager.CheckValidTile(tile))
            {
                return tile;
            }
        }

        // If no valid adjacent tile is found, return the player's tile as a fallback
        return playerTile;
    }

    private Vector3Int FindReachableAdjacentTile(Vector3Int playerTile)
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>
        {
            playerTile + Vector3Int.up,
            playerTile + Vector3Int.down,
            playerTile + Vector3Int.left,
            playerTile + Vector3Int.right
        };

        foreach (var tile in adjacentTiles)
        {
            if (stageManager.CheckValidTile(tile))
            {
                List<Vector3Int> testPath = Pathfinding.FindPath(currentTile, tile, stageManager);
                if (testPath != null && testPath.Count > 0)
                {
                    return tile;
                }
            }
        }

        // If no reachable adjacent tile is found, return the player's tile as a fallback
        return playerTile;
    }


    private void MoveAlongPath()
    {
        if (path.Count > 0)
        {
            Vector3Int nextTile = path.Dequeue();
            if (stageManager.CheckValidTile(nextTile))
            {
                stageEntity.TweenMove(nextTile);
                currentTile = nextTile;
            }
        }
    }
}