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

    public PlayerController player;

    private Vector3Int currentTile;
    private Vector3Int targetTile;
    private Queue<Vector3Int> path;
    private StageManager stageManager;

    [SerializeField] Vector3Int attackingTile;
    [SerializeField] GameObject attackIndicator;

    TurnManager turnManager;

    [SerializeField] State _currentState = State.Idle;
    [SerializeField] DamagePayload damagePayload;
    
    [Header("Attacking Debug")]
    [SerializeField] bool preparingAttack = false;

    [Header("Debug")]
    [SerializeField] bool testPathfinding = false; 
    [SerializeField] bool forceMoveAlongPath = false;
    [SerializeField] int pathCount = 0;

    [SerializeField] int cooldownTurns = 0;

    void Start()
    {
        stageManager = StageManager.Instance;
        turnManager = TurnManager.Instance;

        currentTile = stageManager.GroundTilemap.WorldToCell(transform.position);
        targetTile = stageManager.GroundTilemap.WorldToCell(player.tilePosition);
        path = new Queue<Vector3Int>();

        turnManager.OnNextTurn += EnqueueAction;
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

    void EnqueueAction()
    {
        TurnAction action = new(1, CheckState, () => stageEntity.IsAlive);
        turnManager.AddTurnAction(action);
    }

    public void CheckState()
    {
        if(cooldownTurns > 0)
        {
            cooldownTurns--;
            return;
        }

        if(preparingAttack)
        {
            _currentState = State.Attacking;
            PerformAction();
            return;
        }

        Pathfind();
        if (path.Count > 0 && path.Peek() == currentTile)
        {
            path.Dequeue(); // Remove the first tile from the path, as it is the current tile
        }

        if (IsAdjacent(currentTile, stageManager.GroundTilemap.WorldToCell(player.tilePosition)))
        {
            attackingTile = stageManager.GroundTilemap.WorldToCell(player.tilePosition);
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
            MoveRandomly();
                break;
            case State.Moving:
                MoveAlongPath();
                break;
            case State.Attacking:
                Attack(attackingTile);
                break;
        }
    }

    void PrepareAttack(Vector3Int targetTile)
    {
        preparingAttack = true;
        attackIndicator.transform.position = targetTile;
        StartCoroutine(FlashIndicator());
    }

    IEnumerator FlashIndicator()
    {
        while(preparingAttack)
        {
            attackIndicator.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            attackIndicator.SetActive(false);
            yield return new WaitForSeconds(0.25f);
        }
    }

    void Attack(Vector3Int targetTile)
    {
        if(!preparingAttack)
        {
            PrepareAttack(targetTile);
        }else
        {
            attackIndicator.SetActive(false);
            StageEntity target = stageManager.GetGroundTileData(targetTile).entity;
            if(target && target.CompareTag(TagNames.Player.ToString()))
            {
                target.HurtEntity(damagePayload);
            }
            preparingAttack = false;
            cooldownTurns = 1;
        }
    }

    void Pathfind()
    {
        targetTile = FindValidAdjacentTile(stageManager.GroundTilemap.WorldToCell(player.tilePosition));
        if (path.Count == 0 || path.Peek() != targetTile)
        {
            FindPathToTarget();
        }
    }

    private void FindPathToTarget()
    {
        currentTile = stageEntity.tilePosition;
        targetTile = FindReachableAdjacentTile(stageManager.GroundTilemap.WorldToCell(player.tilePosition));


        //Debug.Log($"Finding path from {currentTile} to {targetTile}");

        List<Vector3Int> newPath = Pathfinding.FindPath(currentTile, targetTile, stageManager);
        if (newPath != null)
        {
            path = new Queue<Vector3Int>(newPath);
            //Debug.Log("Path found. Path count: " + path.Count);
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

            if(!IsAdjacent(currentTile, nextTile))
            {
                Debug.LogWarning("Next tile in path is not adjacent to current tile.");
                return;
            }

            if (stageManager.CheckValidTile(nextTile))
            {
                stageEntity.TweenMove(nextTile);
                currentTile = nextTile;
            }
        }
    }

    private void MoveRandomly()
    {
        Vector3Int randomTile = FindRandomAdjacentTile();
        if (stageManager.CheckValidTile(randomTile))
        {
            stageEntity.TweenMove(randomTile);
            currentTile = randomTile;
        }

    }

    private Vector3Int FindRandomAdjacentTile()
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>
        {
            currentTile + Vector3Int.up,
            currentTile + Vector3Int.down,
            currentTile + Vector3Int.left,
            currentTile + Vector3Int.right
        };

        // Shuffle the list to get a random valid tile
        for (int i = 0; i < adjacentTiles.Count; i++)
        {
            Vector3Int temp = adjacentTiles[i];
            int randomIndex = Random.Range(i, adjacentTiles.Count);
            adjacentTiles[i] = adjacentTiles[randomIndex];
            adjacentTiles[randomIndex] = temp;
        }

        foreach (var tile in adjacentTiles)
        {
            if (stageManager.CheckValidTile(tile))
            {
                return tile;
            }
        }

        // If no valid adjacent tile is found, return the current tile
        return currentTile;
    }

}