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

    void Update()
    {
        float angle = Mathf.DeltaAngle(transform.eulerAngles.y, currentAiGate.nextGate.getRotation());
        float gateDifference = Mathf.Abs(angle);
        bool leftObstacleDetected = leftObstacleDetector.isObstacleDetected();
        bool rightObstacleDetected = rightObstacleDetector.isObstacleDetected();
        bool leftGroundDetected = leftGroundDetector.isGroundDetected();
        bool rightGroundDetected = rightGroundDetector.isGroundDetected();
        bool onLeftSide = leftSideDetector.isLeftDetected();
        bool onRightSide = rightSideDetector.isRightDetected();
        if (avoiding > 0f)
        {
            avoiding -= Time.deltaTime;
        }
        if (!readyForNextGate && avoiding <= 0)
        {
            if (gateDifference < 5 && currentAiGate.side == AiGate.Side.left && !onLeftSide && !leftObstacleDetected && !leftGroundDetected)
            {
                turn = -0.025f;
            }
            else if (gateDifference < 5 && currentAiGate.side == AiGate.Side.right && !onRightSide && !rightObstacleDetected && !rightGroundDetected)
            {
                turn = 0.025f;
            }
            else if (gateDifference > 1)
            {
                if (angle > 0)
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
                turn = 0f;
                readyForNextGate = true;
            }
        }
        if ((leftObstacleDetected || leftGroundDetected) && !(rightObstacleDetected || rightGroundDetected))
        {
            if (!readyForNextGate && turn > 0f)
            {
                turn /= -10;
            }
            else if (readyForNextGate)
            {
                turn = 0.25f;
                avoiding = 0.25f;
            }
        }
        else if ((rightObstacleDetected || rightGroundDetected) && !(leftObstacleDetected || leftGroundDetected))
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
    }
    void FixedUpdate() 
    {
        float speedKmh = rb.velocity.magnitude * 3.6f;
        aiMovement.Turn(false, false, turn);
        if (speedKmh < currentAiGate.maxSpeed)
        {
            if (!aiMovement.isChangingGear())
            {
                aiMovement.Accelerate(false);
            }
        }
        else if(speedKmh > currentAiGate.maxSpeed+2)
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
