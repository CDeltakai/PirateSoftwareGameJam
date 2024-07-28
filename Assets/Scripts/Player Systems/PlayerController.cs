using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : StageEntity
{
    [SerializeField] int _maxHP = 20;
    public int MaxHP => _maxHP;

    public void SelectElement(int elementIndex)
    {
        Debug.Log("Selected element: " + elementIndex);
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
            Debug.Log("Spell cancelled");
        }
    }

    public void PerformPlayerAction(int priority, TurnActionHandler action, Func<bool> canExecute)
    {
        TurnAction turnAction = new(priority, action, canExecute);
        TurnManager.Instance.ExecutePlayerAction(turnAction);
    }


}
