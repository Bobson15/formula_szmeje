using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlls : MonoBehaviour, ISideFreeDetector
{
    private CarState carState = CarState.Stop;
    private PlayerInput playerInput;
    private Movement playerMovement;
    private Rigidbody rb;
    private OvertakeDetector leftOvertakeDetector, rightOvertakeDetector;
    private float baseGateDiffrence = 0;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<Movement>();
        rb = GetComponent<Rigidbody>();
        leftOvertakeDetector = transform.Find("overtake_left_detector").GetComponent<OvertakeDetector>();
        rightOvertakeDetector = transform.Find("overtake_right_detector").GetComponent<OvertakeDetector>();
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
    public bool isLeftSideFree(GameObject overtakingCar)
    {
        return leftOvertakeDetector.canOvertake(overtakingCar);
    }
    public bool isRightSideFree(GameObject overtakingCar)
    {
        return rightOvertakeDetector.canOvertake(overtakingCar);
    }
    private enum CarState
    {
        Backward = -1,
        Stop = 0,
        Forward = 1
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("AiGate"))
        {
            AiGate currentAiGate = collider.gameObject.GetComponent<AiGate>();
            baseGateDiffrence = Mathf.Abs(Mathf.DeltaAngle(currentAiGate.getRotation(), currentAiGate.nextGate.getRotation()));
        }
    }
    public float getBaseGateDiffrence()
    {
        return baseGateDiffrence;
    }
}
