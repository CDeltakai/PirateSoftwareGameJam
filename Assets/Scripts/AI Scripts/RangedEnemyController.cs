using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : MonoBehaviour
{

    public enum State
    {
        Idle,
        Moving,
        Attacking
    }

    [SerializeField] StageEntity stageEntity;
    public PlayerController player;
    StageManager stageManager;
    TurnManager turnManager;

    [SerializeField] Vector3Int targetTile;

    [Header("Attack Indicator")]
    [SerializeField] GameObject attackIndicator;
    [SerializeField] LineRenderer attackLine;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 30f;
    [SerializeField] Transform firePoint;

    [SerializeField] DamagePayload damagePayload;

    [Header("AI Settings")]
    [SerializeField] CircleCollider2D attackRange; // If the player is within this range, the enemy will attack
    
    // Enemy will first check if there are obstacles between it and the player by firing a raycast to the player
    // If there are no obstacles, the enemy will attack the player
    [SerializeField] LayerMask obstacleLayer; 
    [SerializeField] State _currentState = State.Idle;
    [SerializeField] int cooldownTurns = 0;
    [SerializeField] bool preparingAttack = false;


    [Header("Debug")]
    [SerializeField] bool forcePrepareAttack = false;

    Vector3Int CurrentTile => stageEntity.tilePosition;

    void Start()
    {
        stageManager = StageManager.Instance;
        turnManager = TurnManager.Instance;

        if(!player)
        {
            player = GameManager.Instance.PlayerRef;
        }


        turnManager.OnNextTurn += EnqueueAction;
    }

    void Update()
    {
        if(forcePrepareAttack)
        {
            forcePrepareAttack = false;
            PrepareAttack();
        }

    }


    void EnqueueAction()
    {

        TurnAction action = new(0, CheckState, () => stageEntity.IsAlive);
        turnManager.AddTurnAction(action);
        

    }


    void CheckState()
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

        if(ScanForPlayer())
        {
            _currentState = State.Attacking;
        }else
        {
            _currentState = State.Idle;
        }

        PerformAction();
    }

    void PerformAction()
    {
        switch(_currentState)
        {
            case State.Idle:
                MoveRandomly();
                break;
            case State.Moving:
                break;
            case State.Attacking:
                Attack();
                break;
        }
    }

    bool ScanForPlayer()
    {
        Vector3Int playerTile = player.tilePosition;

        if (Vector3Int.Distance(playerTile, stageEntity.tilePosition) <= attackRange.radius)
        {
            // Perform raycast to check for obstacles
            Vector3 direction = player.transform.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, direction.magnitude, obstacleLayer);

            if (hit.collider == null)
            {
                return true;
            }
        }
    

        return false;
    }

    void Attack()
    {
        if(!preparingAttack)
        {
            PrepareAttack();
        }else
        {
            FireBullet((firePoint.position - attackIndicator.transform.position).normalized);

            attackIndicator.SetActive(false);
            attackLine.gameObject.SetActive(false);

            cooldownTurns = 2;

            StageEntity targetEntity = stageManager.GetGroundTileData(targetTile).entity;
            if(targetEntity)
            {
                if(targetEntity.CompareTag(TagNames.Player.ToString()))
                {
                    targetEntity.HurtEntity(damagePayload);
                }
            }

            preparingAttack = false;

        }
    }

    void PrepareAttack()
    {
        preparingAttack = true;
        Vector3Int playerTile = player.tilePosition;

        attackIndicator.transform.position = player.tilePosition;
        attackIndicator.SetActive(true);

        attackLine.SetPosition(0, firePoint.position);
        attackLine.SetPosition(1, attackIndicator.transform.position);
        attackLine.gameObject.SetActive(true);

        targetTile = playerTile;
        
    }

    void FireBullet(Vector3 direction)
    {
        print("Firing bullet");
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(Vector3.forward, direction));
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.targetPosition = targetTile;
        bulletScript.speed = bulletSpeed;
        
    }

    private void MoveRandomly()
    {
        Vector3Int randomTile = FindRandomAdjacentTile();
        if (stageManager.CheckValidTile(randomTile))
        {
            stageEntity.TweenMove(randomTile);
        }

    }

    private Vector3Int FindRandomAdjacentTile()
    {
        List<Vector3Int> adjacentTiles = new List<Vector3Int>
        {
            CurrentTile + Vector3Int.up,
            CurrentTile + Vector3Int.down,
            CurrentTile + Vector3Int.left,
            CurrentTile + Vector3Int.right
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
        return CurrentTile;
    }

}
