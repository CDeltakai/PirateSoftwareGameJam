using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStatusManager : MonoBehaviour
{
[Header("References")]
    [SerializeField] SpriteRenderer _spriteRenderer;
    TurnManager _turnManager;


[Header("Current Status Values")]
    [SerializeField] bool _stunned = false;
    public bool Stunned => _stunned;
    [SerializeField] int _remainingStunTurns = 0; 

    void Start()
    {
        _turnManager = TurnManager.Instance;
        _turnManager.OnNextTurn += TickStatusEffects;
    }

    public void ApplyStatusEffect(StatusEffect statusEffect)
    {
        switch(statusEffect.statusType)
        {
            case StatusType.Stunned:
                _stunned = true;
                _remainingStunTurns += (int)statusEffect.effectStrength;
                break;
        }
    }

    
    void TickStatusEffects()
    {
        if(_stunned)
        {
            _remainingStunTurns--;
            if(_remainingStunTurns <= 0)
            {
                _stunned = false;
            }
        }
    }    

    public void FlashRed()
    {
        StartCoroutine(FlashRedRoutine());
    }

    IEnumerator FlashRedRoutine()
    {
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        _spriteRenderer.color = Color.white;
    }
}
