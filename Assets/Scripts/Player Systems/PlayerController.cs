using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : StageEntity
{
    [SerializeField] int _maxHP = 20;


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
}
