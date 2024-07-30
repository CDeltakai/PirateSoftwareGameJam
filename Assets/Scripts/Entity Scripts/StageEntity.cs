using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(EntityStatusManager))]
public class StageEntity : MonoBehaviour
{
#region Events and Delegates

    public delegate void HPChangedEventHandler(int oldValue, int newValue);
    public event HPChangedEventHandler OnHPChanged;
    public event Action OnHPModified;//No parameters, just a notification
    public event Action OnKilled;

#endregion


    [SerializeField] protected Transform worldTransform;
    public Transform WorldTransform => worldTransform;
    [SerializeField] protected Transform _spriteCenterPoint;
    public Transform SpriteCenterPoint => _spriteCenterPoint;


    public Vector3Int tilePosition;
    public Ease defaultMoveEase = Ease.InOutSine;



[Header("Entity Stats")]
    [SerializeField, Min(0)] int _currentHP = 2;

    public int CurrentHP {
        get{return _currentHP;}
        set
        {
            if(_currentHP != value)
            {
                int oldValue = _currentHP;
                _currentHP = value;
                OnHPChanged?.Invoke(oldValue, _currentHP);
                OnHPModified?.Invoke();
            }
        }
    }

    public bool IsAlive => _currentHP > 0;
    public bool invincible = false;

//References
    StageManager _stageManager;
    EntityStatusManager _statusManager;


    void InitStartingMethods()
    {
        _stageManager = StageManager.Instance;      
        _statusManager = GetComponent<EntityStatusManager>();  
        InitializePosition();
    }

    protected virtual void Start()
    {
        InitStartingMethods();
    }

    void Update()
    {
        
    }

    void InitializePosition()
    {
        int checkRadius = 2;

        GroundTileData initialPosition = _stageManager.FindClosestValidTile(worldTransform.position, checkRadius);
        if(initialPosition == null) 
        {
            Debug.LogError("No valid tile found for entity: " + gameObject.name + " at position: " + worldTransform.position + " within a radius of " + checkRadius);
            return; 
        }

        tilePosition = initialPosition.localCoordinates;
        _stageManager.SetTileEntity(this, tilePosition);
    }


    public void TweenMove(int x, int y)
    {
        Vector3Int destination = new();
        destination = new Vector3Int(tilePosition.x + x, tilePosition.y + y, 0);

        if(!_stageManager.CheckValidTile(destination)) { return; }

        _stageManager.SetTileEntity(null, tilePosition);
        tilePosition.Set(destination.x, destination.y, 0);
        _stageManager.SetTileEntity(this, destination);

        worldTransform.DOMove(destination, 0.1f).SetEase(defaultMoveEase);
        tilePosition = destination;
    }

    //Overload for Vector3Int - Moves directly to the destination
    public void TweenMove(Vector3Int destination)
    {
        if(!_stageManager.CheckValidTile(destination)) { return; }
        _stageManager.SetTileEntity(null, tilePosition);
        tilePosition.Set(destination.x, destination.y, 0);
        _stageManager.SetTileEntity(this, destination);

        worldTransform.DOMove(destination, 0.1f).SetEase(defaultMoveEase);
        tilePosition = destination;
    }

    public void TweenMove(Vector2Int direction)
    {
        TweenMove(direction.x, direction.y);
    }

    public void TweenMove(Vector2 direction)
    {
        TweenMove(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));
    }

    public void SetLocation(Vector3Int location)
    {
        if(!_stageManager.CheckValidTile(location)) { return; }

        _stageManager.SetTileEntity(null, tilePosition);
        tilePosition = location;
        _stageManager.SetTileEntity(this, tilePosition);

        worldTransform.position = location;
    }

    public void SetLocation(Vector2Int location)
    {
        SetLocation(new Vector3Int(location.x, location.y, 0));
    }

    public void EnqueueWorldAction(int priority, TurnActionHandler action, Func<bool> canExecute)
    {
        TurnAction turnAction = new(priority, action, canExecute);
        TurnManager.Instance.AddTurnAction(turnAction);
    }

    public void HurtEntity(DamagePayload damagePayload)
    {
        if(invincible) { return; }

        foreach(var statusEffect in damagePayload.statusEffects)
        {
            _statusManager.ApplyStatusEffect(statusEffect);
        }

        CurrentHP -= damagePayload.damage;
        if(!IsAlive)
        {
            DestroyEntity();
        }
    }


    public virtual void DestroyEntity(Action preDestroyAction = null)
    {
        preDestroyAction?.Invoke();

        _stageManager.SetTileEntity(null, tilePosition);

        OnKilled?.Invoke();

        Destroy(gameObject);
    }

}
