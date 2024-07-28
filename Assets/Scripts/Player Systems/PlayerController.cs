using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : StageEntity
{
    [SerializeField] int _maxHP = 20;
    public int MaxHP => _maxHP;

    [SerializeField] PlayerSpellManager _spellManager;

    protected override void Start()
    {
        base.Start();
        if(!_spellManager)
        {
            _spellManager = GetComponent<PlayerSpellManager>();
            if(!_spellManager)
            {
                Debug.LogError("PlayerSpellManager not found on PlayerController");
            }
        }
    }

    public void ConfirmSpell(CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log("Spell confirmed");
        }
    }

    public void CancelSpell(CallbackContext context)
    {
        if(context.performed)
        {
            _spellManager.ClearSpellMix();
        }
    }

    public void PerformPlayerAction(int priority, TurnActionHandler action, Func<bool> canExecute)
    {
        TurnAction turnAction = new(priority, action, canExecute);
        TurnManager.Instance.ExecutePlayerAction(turnAction);
    }


}
