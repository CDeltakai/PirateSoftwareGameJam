using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour
{
    PlayerController playerController;

    [SerializeField] float actionLockoutTime = 0.1f;

    bool isPerformingAction = false;

    Coroutine CR_actionLockout;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    void TriggerActionLockout()
    {
        CR_actionLockout = StartCoroutine(ActionLockout(actionLockoutTime));
    }
    IEnumerator ActionLockout(float time)
    {
        isPerformingAction = true;
        yield return new WaitForSecondsRealtime(time);
        isPerformingAction = false;
        CR_actionLockout = null;
    }

    public void MoveUp(CallbackContext context)
    {
        if(isPerformingAction){return;}

        if(context.performed)
        {
            TriggerActionLockout();
            Vector2Int moveDirection = new(0, 1);
            playerController.PerformPlayerAction(0, () => playerController.TweenMove(moveDirection), () => playerController.IsAlive);
        }
    }

    public void MoveDown(CallbackContext context)
    {
        if(isPerformingAction){return;}

        if(context.performed)
        {
            TriggerActionLockout();

            Vector2Int moveDirection = new(0, -1);
            playerController.PerformPlayerAction(0, () => playerController.TweenMove(moveDirection), () => playerController.IsAlive);
        }
    }

    public void MoveLeft(CallbackContext context)
    {
        if(isPerformingAction){return;}

        if(context.performed)
        {
            TriggerActionLockout();

            Vector2Int moveDirection = new(-1, 0);
            playerController.PerformPlayerAction(0, () => playerController.TweenMove(moveDirection), () => playerController.IsAlive);
        }
    }

    public void MoveRight(CallbackContext context)
    {
        if(isPerformingAction){return;}

        if(context.performed)
        {
            TriggerActionLockout();

            Vector2Int moveDirection = new(1, 0);
            playerController.PerformPlayerAction(0, () => playerController.TweenMove(moveDirection), () => playerController.IsAlive);
        }
    }

}
