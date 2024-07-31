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

    [SerializeField] ControlState controlState = ControlState.Movement;

    [SerializeField] float actionLockoutTime = 0.1f;
    [SerializeField] bool actionLocked = false;

    [SerializeField] int restoreHPOnKill = 3;
    EventBinding<NPCKilledEvent> npcKilledEventBinding;

    public void RestoreHP(int amount)
    {
        CurrentHP += amount;
        if(CurrentHP > _maxHP)
        {
            CurrentHP = _maxHP;
        }
    }

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

    void OnEnable()
    {
        npcKilledEventBinding = new EventBinding<NPCKilledEvent>(() => RestoreHP(restoreHPOnKill));
        EventBus<NPCKilledEvent>.Register(npcKilledEventBinding);        
    }

    void ActionLockout()
    {
        actionLocked = true;
        StartCoroutine(UnlockAction());
    }

    IEnumerator UnlockAction()
    {
        yield return new WaitForSeconds(actionLockoutTime);
        actionLocked = false;
    }

    public void ConfirmSpell(CallbackContext context)
    {
        if(TurnManager.TurnInProgress){return;}
        if(actionLocked){return;}

        if(controlState != ControlState.Movement)
        {
            return;
        }

        if(context.performed)
        {
            if(_spellManager.ConfirmSpellMix())
            {
                Debug.Log("Spell confirmed");
                ActionLockout();
                SwitchControlState(ControlState.Aiming);
            }
        }
    }

    public void CancelSpell(CallbackContext context)
    {
        if(TurnManager.TurnInProgress){return;}


        if(context.performed)
        {
            SwitchControlState(ControlState.Movement);  
            _spellManager.ClearSpellMix();
        }
    }

    public void DeploySpell(CallbackContext context)
    {
        if(TurnManager.TurnInProgress){return;}


        if(actionLocked)
        {
            return;
        }

        if(controlState != ControlState.Aiming || !_cursorController.CursorActive)
        {
            return;
        }

        if(context.performed)
        {

            void ExecuteSpell()
            {
                if(_spellManager.CastSpell())
                {
                    SwitchControlState(ControlState.Movement);
                    ActionLockout();
                }
            }

            PerformPlayerAction(0, ExecuteSpell, () => IsAlive, () => !_spellManager.CastingInProgress);

        }
    }



    public void PerformPlayerAction(int priority, TurnActionHandler action, Func<bool> canExecute, Func<bool> delayCondition = null)
    {
        TurnAction turnAction = new(priority, action, canExecute);
        TurnManager.Instance.ExecutePlayerAction(turnAction, delayCondition);
    }

    public void SwitchControlState(ControlState state)
    {
        switch (state)
        {
            case ControlState.Movement:
                controlState = ControlState.Movement;
                _playerMovement.movementEnabled = true;
                _cursorController.SetCursor(false);
                break;
            case ControlState.Aiming:
                controlState = ControlState.Aiming;
                _playerMovement.movementEnabled = false;
                _cursorController.SetCursor(true);
                break;
            default:
                break;
        }
    }

}
