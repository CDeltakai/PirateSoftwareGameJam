using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour
{
    PlayerController playerController;


    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveUp(CallbackContext context)
    {
        if(context.performed)
        {
            Vector2Int moveDirection = new(0, 1);
            playerController.TweenMove(moveDirection);
        }
    }

    public void MoveDown(CallbackContext context)
    {
        if(context.performed)
        {
            Vector2Int moveDirection = new(0, -1);
            playerController.TweenMove(moveDirection);
        }
    }

    public void MoveLeft(CallbackContext context)
    {
        if(context.performed)
        {
            Vector2Int moveDirection = new(-1, 0);
            playerController.TweenMove(moveDirection);
        }
    }

    public void MoveRight(CallbackContext context)
    {
        if(context.performed)
        {
            Vector2Int moveDirection = new(1, 0);
            playerController.TweenMove(moveDirection);
        }
    }

}
