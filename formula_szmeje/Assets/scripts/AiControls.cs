using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AiControls : MonoBehaviour
{
    private Movement aiMovement;
    private PlayerInput playerInput;
    private Rigidbody rb;
    private Transform transform;
    public AiGate currentAiGate;
    private ObstacleDetector leftObstacleDetector, rightObstacleDetector;
    private GroundDetector leftGroundDetector, rightGroundDetector;
    private float turn = 0f;
    private bool readyForNextGate = false;
    void Start()
    {
        aiMovement = GetComponent<Movement>();
        rb = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
        playerInput = GetComponent<PlayerInput>();
        leftObstacleDetector = transform.Find("obstacle_left_detector").GetComponent<ObstacleDetector>();
        rightObstacleDetector = transform.Find("obstacle_right_detector").GetComponent<ObstacleDetector>();
        leftGroundDetector = transform.Find("tire_front_L").GetComponent<GroundDetector>();
        rightGroundDetector = transform.Find("tire_front_R").GetComponent<GroundDetector>();
    }

    void FixedUpdate()
    {
        leftGroundDetector.info();
        rightGroundDetector.info();
        if (!readyForNextGate)
        {
            float gateDifference = Mathf.Abs(currentAiGate.nextGate.getRotation() - transform.eulerAngles.y);
            if (gateDifference > 2)
            {
                if (transform.eulerAngles.y < 180)
                {
                    if (currentAiGate.nextGate.getRotation() > transform.eulerAngles.y && currentAiGate.nextGate.getRotation() <= transform.eulerAngles.y + 180)
                    {
                        turn = 1f;
                    }
                    else
                    {
                        turn = -1f;
                    }
                }
                else
                {
                    if (currentAiGate.nextGate.getRotation() < transform.eulerAngles.y && currentAiGate.nextGate.getRotation() >= transform.eulerAngles.y - 180)
                    {
                        turn = -1f;
                    }
                    else
                    {
                        turn = 1f;
                    }
                }
            }
            else
            {
                turn = 0f;
                readyForNextGate = true;
            }
        }
        if ((leftObstacleDetector.isObstacleDetected() || leftGroundDetector.isGroundDetected()) && !(rightObstacleDetector.isObstacleDetected() || rightGroundDetector.isGroundDetected()))
        {
            if (!readyForNextGate && turn == -1f)
            {
                turn = 0f;
            }
            else if(readyForNextGate)
            {
                if (leftObstacleDetector.isObstacleDetected())
                {
                    turn = 0.25f;
                }
                else
                {
                    turn = 1f;
                }
            }
        }
        else if ((rightObstacleDetector.isObstacleDetected() || rightGroundDetector.isGroundDetected()) && !(leftObstacleDetector.isObstacleDetected() || leftGroundDetector.isGroundDetected()))
        {
            if (!readyForNextGate && turn == 1f)
            {
                turn = 0f;
            }
            else if (readyForNextGate)
            {
                if (rightObstacleDetector.isObstacleDetected())
                {
                    turn = -0.25f;
                }
                else
                {
                    turn = 1f;
                }
            }
        }
        else if(turn != 0f && readyForNextGate)
        {
            turn = 0f;
            readyForNextGate = false;
        }
            aiMovement.Turn(false, false, turn);
        if (rb.velocity.magnitude * 3.6f < currentAiGate.maxSpeed)
        {
            if (!aiMovement.isChangingGear())
            {
                aiMovement.Accelerate(false);
            }
        }
        else if(rb.velocity.magnitude * 3.6f > currentAiGate.maxSpeed+2)
        {
            aiMovement.Break(false, false, turn);
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
            readyForNextGate = false;
        }
    }
}
