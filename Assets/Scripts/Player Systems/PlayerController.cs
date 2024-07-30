using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : StageEntity
{
    public enum ControlState
    {
        Movement,
        Aiming, //Whilst in aiming state movement should be disabled and virtual cursor should be enabled
    }


    [SerializeField] int _maxHP = 20;
    public int MaxHP => _maxHP;

    [SerializeField] PlayerSpellManager _spellManager;
    [SerializeField] PlayerMovement _playerMovement;
    [SerializeField] VirtualCursorController _cursorController;

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

    public void DeploySpell(CallbackContext context)
    {
        if(context.performed)
        {
            _spellManager.CastSpell();
        }
    }

    public void PerformPlayerAction(int priority, TurnActionHandler action, Func<bool> canExecute)
    {
        TurnAction turnAction = new(priority, action, canExecute);
        TurnManager.Instance.ExecutePlayerAction(turnAction);
    }

    public void SwitchControlState(ControlState state)
    {
        switch (state)
        {
            case ControlState.Movement:
                _playerMovement.movementEnabled = true;
                _cursorController.CursorActive = false;
                break;
            case ControlState.Aiming:
                _playerMovement.movementEnabled = false;
                _cursorController.CursorActive = true;
                break;
            default:
                break;
        }
    }

}
