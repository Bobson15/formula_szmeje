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
    private CarDetector frontCarDetector, leftCarDetector, rightCarDetector, leftTurnCarDetector, rightTurnCarDetector;
    private float turn = 0f;
    private float baseGateDifference = 0f;
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
        frontCarDetector = transform.Find("car_front_detector").GetComponent<CarDetector>();
        leftCarDetector = transform.Find("car_left_detector").GetComponent<CarDetector>();
        rightCarDetector = transform.Find("car_right_detector").GetComponent<CarDetector>();
        leftTurnCarDetector = transform.Find("car_left_turn_detector").GetComponent<CarDetector>();
        rightTurnCarDetector = transform.Find("car_right_turn_detector").GetComponent<CarDetector>();
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
        bool frontCarDetected = frontCarDetector.isCarDetected();
        bool leftCarDetected = leftCarDetector.isCarDetected();
        bool rightCarDetected = rightCarDetector.isCarDetected();
        bool leftTurnCarDetected = leftTurnCarDetector.isCarDetected();
        bool rightTurnCarDetected = rightTurnCarDetector.isCarDetected();
        float oversteer = aiMovement.getOversteer();
        if (avoiding > 0f)
        {
            avoiding -= Time.deltaTime;
        }
        if (!readyForNextGate && avoiding <= 0)
        {
            if (gateDifference < 5 && currentAiGate.side == AiGate.Side.left && !onLeftSide && !leftObstacleDetected && !leftGroundDetected && !leftCarDetected)
            {
                turn = -0.025f;
            }
            else if (gateDifference < 5 && currentAiGate.side == AiGate.Side.right && !onRightSide && !rightObstacleDetected && !rightGroundDetected && !rightCarDetected)
            {
                turn = 0.025f;
            }
            else if (gateDifference > 1)
            {
                if (baseGateDifference <= 5)
                {
                    if (angle > 0 && (!rightCarDetected || leftObstacleDetected || leftGroundDetected))
                    {
                        turn = 1f;
                    }
                    else if (angle < 0 && (!leftCarDetected || rightObstacleDetected || rightGroundDetected))
                    {
                        turn = -1f;
                    }
                }
                else
                {
                    if (angle > 0 && (!rightTurnCarDetected || leftObstacleDetected || leftGroundDetected))
                    {
                        turn = 1f;
                    }
                    else if (angle < 0 && (!leftTurnCarDetected || rightObstacleDetected || rightGroundDetected))
                    {
                        turn = -1f;
                    }
                }
            }
            else
            {
                turn = 0f;
                readyForNextGate = true;
            }
        }
        if ((leftObstacleDetected || leftGroundDetected) && !(rightObstacleDetected || rightGroundDetected) && turn <= 0f)
        {
            turn = 0.25f;
            if (readyForNextGate)
            {
                avoiding = 0.25f;
            }
        }
        else if ((rightObstacleDetected || rightGroundDetected) && !(leftObstacleDetected || leftGroundDetected) && turn >= 0f)
        {
            turn = -0.25f;
            if (readyForNextGate)
            {
                avoiding = 0.25f;
            }
        }
        else if (oversteer > 0.2f && ((angle > 0 && oversteer < 0 && !rightTurnCarDetected) || (angle < 0 && oversteer > 0 && !leftTurnCarDetected)))
        {
            turn = oversteer;
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

        bool needToBrake = (frontCarDetector.isCarDetected() && frontCarDetector.getDetectedCarSpeed() < speedKmh);

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
            baseGateDifference = Mathf.Abs(Mathf.DeltaAngle(currentAiGate.getRotation(), currentAiGate.nextGate.getRotation()));
            avoiding = 0f;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("bariers") || collision.gameObject.CompareTag("AI") || collision.gameObject.CompareTag("Player"))
        {
            readyForNextGate = false;
            Debug.Log("collision");
        }
    }
}
