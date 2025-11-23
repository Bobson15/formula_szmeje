using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlls : MonoBehaviour
{
    private CarState carState = CarState.Stop;
    private PlayerInput playerInput;
    private Movement playerMovement;
    private Rigidbody rb;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<Movement>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        playerMovement.Turn(playerInput.actions["TurnLeft"].IsPressed(), playerInput.actions["TurnRight"].IsPressed(), playerInput.actions["Turn"].ReadValue<Vector2>().x);
        if (playerInput.actions["Throttle"].IsPressed())
        {
            if (carState != CarState.Backward)
            {
                if (!playerMovement.isChangingGear())
                {
                    playerMovement.Accelerate(playerInput.actions["Brake"].IsPressed());
                }
                carState = CarState.Forward;
            }
            else
            {
                playerMovement.Break(playerInput.actions["TurnLeft"].IsPressed(), playerInput.actions["TurnRight"].IsPressed(), playerInput.actions["Turn"].ReadValue<Vector2>().x);
                if (rb.velocity.magnitude < 0.1)
                {
                    carState = CarState.Stop;
                }
            }
        }
        if (playerInput.actions["Brake"].IsPressed())
        {
            if (carState == CarState.Forward)
            {
                playerMovement.Break(playerInput.actions["TurnLeft"].IsPressed(), playerInput.actions["TurnRight"].IsPressed(), playerInput.actions["Turn"].ReadValue<Vector2>().x);
                if (rb.velocity.magnitude < 0.1)
                {
                    carState = CarState.Stop;
                }
            }
            else
            {
                playerMovement.Reverse(playerInput.actions["Throttle"].IsPressed());
                carState = CarState.Backward;
            }
        }
        if (!playerInput.actions["Throttle"].IsPressed() && !playerInput.actions["Brake"].IsPressed())
        {
            playerMovement.Decelerate();
            if (rb.velocity.magnitude < 0.1)
            {
                carState = CarState.Stop;
            }
        }
    }
    private enum CarState
    {
        Backward = -1,
        Stop = 0,
        Forward = 1
    }
}
