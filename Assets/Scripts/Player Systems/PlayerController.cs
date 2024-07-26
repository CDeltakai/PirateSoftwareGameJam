using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : StageEntity
{



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
