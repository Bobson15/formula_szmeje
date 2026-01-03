using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AiControls : MonoBehaviour
{
    private Movement aiMovement;
    private PlayerInput playerInput;
    private Rigidbody rb;
    public AiGate currentAiGate;
    void Start()
    {
        aiMovement = GetComponent<Movement>();
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }

    void FixedUpdate()
    {
        //w przysz³oœci dodaæ skrêcanie ai na razie rêczne
        aiMovement.Turn(playerInput.actions["TurnLeft"].IsPressed(), playerInput.actions["TurnRight"].IsPressed(), playerInput.actions["Turn"].ReadValue<Vector2>().x);
        if (rb.velocity.magnitude * 3.6f < currentAiGate.maxSpeed)
        {
            if (!aiMovement.isChangingGear())
            {
                aiMovement.Accelerate(false);
            }
        }
        else if(rb.velocity.magnitude * 3.6f > currentAiGate.maxSpeed+2)
        {
            aiMovement.Break(playerInput.actions["TurnLeft"].IsPressed(), playerInput.actions["TurnRight"].IsPressed(), playerInput.actions["Turn"].ReadValue<Vector2>().x);
        }
        else
        {
            aiMovement.Decelerate();
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("AiGate"))
        {
            currentAiGate = collider.gameObject.GetComponent<AiGate>();
        }
    }
}
