using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StageEntity : MonoBehaviour
{
#region Events and Delegates

    public delegate void HPChangedEventHandler(int oldValue, int newValue);
    public event HPChangedEventHandler OnHPChanged;

#endregion


    [SerializeField] protected Transform worldTransform;
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
            }
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    bool ValidateMove(Vector2Int direction)
    {
        return false;
    }

    public virtual void TweenMove(int x, int y)
    {
        Vector3Int destination = new();
        destination = new Vector3Int(tilePosition.x + x, tilePosition.y + y, 0);

        worldTransform.DOMove(destination, 0.1f).SetEase(defaultMoveEase);
        tilePosition = destination;
    }

    public virtual void TweenMove(Vector2Int direction)
    {
        TweenMove(direction.x, direction.y);
    }

    public void TweenMove(Vector2 direction)
    {
        TweenMove(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));
    }

}
