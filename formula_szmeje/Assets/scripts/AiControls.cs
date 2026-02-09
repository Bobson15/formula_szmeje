using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AiControls : MonoBehaviour
{
    private Movement aiMovement;
    private Rigidbody rb;
    private Transform transform;
    public AiGate currentAiGate;
    private ObstacleDetector leftObstacleDetector, rightObstacleDetector;
    private GroundDetector leftGroundDetector, rightGroundDetector;
    private SideDetector leftSideDetector, rightSideDetector;
    private float turn = 0f;
    private bool readyForNextGate = false;
    private float avoiding = 0f;
    void Start()
    {
        aiMovement = GetComponent<Movement>();
        rb = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
        leftObstacleDetector = transform.Find("obstacle_left_detector").GetComponent<ObstacleDetector>();
        rightObstacleDetector = transform.Find("obstacle_right_detector").GetComponent<ObstacleDetector>();
        leftGroundDetector = transform.Find("ground_left_detector").GetComponent<GroundDetector>();
        rightGroundDetector = transform.Find("ground_right_detector").GetComponent<GroundDetector>();
        leftSideDetector = transform.Find("side_left_detector").GetComponent<SideDetector>();
        rightSideDetector = transform.Find("side_right_detector").GetComponent<SideDetector>();
    }

    void FixedUpdate()
    {
        leftSideDetector.info();
        rightSideDetector.info();
        float gateDifference = Mathf.Abs(currentAiGate.nextGate.getRotation() - transform.eulerAngles.y);
        if(avoiding > 0f)
        {
            avoiding -= Time.fixedDeltaTime;
        }
        if (!readyForNextGate && avoiding <= 0)
        {
            if (gateDifference < 5 && currentAiGate.side == AiGate.Side.left && !leftSideDetector.isLeftDetected() && !leftObstacleDetector.isObstacleDetected() && !leftGroundDetector.isGroundDetected())
            {
                turn = -0.025f;
            }
            else if (gateDifference < 5 && currentAiGate.side == AiGate.Side.right && !rightSideDetector.isRightDetected() && !rightObstacleDetector.isObstacleDetected() && !rightGroundDetector.isGroundDetected())
            {
                turn = 0.025f;
            }
            else if (gateDifference > 1)
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
            if (!readyForNextGate && turn > 0f)
            {
                turn /=-10;
            }
            else if (readyForNextGate)
            {
                turn = 0.25f;
                avoiding = 0.25f;
            }
        }
        else if ((rightObstacleDetector.isObstacleDetected() || rightGroundDetector.isGroundDetected()) && !(leftObstacleDetector.isObstacleDetected() || leftGroundDetector.isGroundDetected()))
        {
            if (!readyForNextGate && turn < 0f)
            {
                turn /= -10;
            }
            else if (readyForNextGate)
            {
                turn = -0.25f;
                avoiding = 0.25f;
            }
        }
        else if (gateDifference > 1 && readyForNextGate)
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
            avoiding = 0f;
        }
    }
}
