using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AiControls : MonoBehaviour, ISideFreeDetector
{
    private Movement aiMovement;
    private Rigidbody rb;
    private Transform transform;
    public AiGate currentAiGate;
    private ObstacleDetector leftObstacleDetector, rightObstacleDetector;
    private GroundDetector leftGroundDetector, rightGroundDetector;
    private SideDetector leftSideDetector, rightSideDetector;
    private CarDetector frontCarDetector, leftCarDetector, rightCarDetector, leftTurnCarDetector, rightTurnCarDetector;
    private OvertakeDetector leftOvertakeDetector, rightOvertakeDetector;
    private float turn = 0f;
    private float baseGateDifference = 0f;
    private bool readyForNextGate = false, goingToOvertake = false;
    private float avoiding = 0f, reversing = 0f;
    private float speedCorection = 0, angleCorection = 0;
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
        leftOvertakeDetector = transform.Find("overtake_left_detector").GetComponent<OvertakeDetector>();
        rightOvertakeDetector = transform.Find("overtake_right_detector").GetComponent<OvertakeDetector>();
    }

    void Update()
    {
        float angle = Mathf.DeltaAngle(transform.eulerAngles.y, currentAiGate.nextGate.getRotation());
        float gateDifference = Mathf.Abs(angle  + angleCorection);
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
        if(reversing > 0f)
        {
            reversing -= Time.deltaTime;
        }
        if (!readyForNextGate && avoiding <= 0)
        {
            if (currentAiGate.side == AiGate.Side.left && !onLeftSide && gateDifference < 5 && !leftObstacleDetected && !leftGroundDetected && !(rightGroundDetected && leftGroundDetected) && !leftCarDetected && !goingToOvertake && reversing <= 0)
            {
                turn = -0.2f;
            }
            else if (currentAiGate.side == AiGate.Side.right && !onRightSide && gateDifference < 5 && !rightObstacleDetected && !rightGroundDetected && !(leftGroundDetected && rightGroundDetected) && !rightCarDetected && !goingToOvertake && reversing <= 0)
            {
                turn = 0.2f;
            }
            else if (gateDifference > 1)
            {
                if (baseGateDifference <= 5)
                {
                    if (angle + angleCorection > 0 && (!rightCarDetected || leftObstacleDetected || leftGroundDetected))
                    {
                        turn = 1f;
                    }
                    else if (angle + angleCorection < 0 && (!leftCarDetected || rightObstacleDetected || rightGroundDetected))
                    {
                        turn = -1f;
                    }
                }
                else
                {
                    if (angle + angleCorection > 0 && (!rightCarDetected || leftObstacleDetected || leftGroundDetected))
                    {
                        turn = 1f;
                    }
                    else if (angle + angleCorection < 0 && (!leftCarDetected || rightObstacleDetected || rightGroundDetected))
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

        angleCorection = 0;

        if(turn < 0f && baseGateDifference > 5f && (rightCarDetected || rightGroundDetected))
        {
            speedCorection = -(currentAiGate.maxSpeed / 10);
        }
        else if(turn > 0f && baseGateDifference > 5f && (leftCarDetected || leftGroundDetected))
        {
            speedCorection = -(currentAiGate.maxSpeed / 10);
        }
        else
        {
            speedCorection = 0;
        }


        if ((leftObstacleDetector.isObstacleDetected() && rightObstacleDetector.isObstacleDetected()) || reversing > 0f)
        {
            turn = -turn;
        }
        else if (leftGroundDetected && rightGroundDetected)
        {
            Vector3 gate = currentAiGate.nextGate.getPossition(), left = leftGroundDetector.getPossition(), right = rightGroundDetector.getPossition();
            gate.y = 0;
            left.y = 0;
            right.y = 0;
            float distanceRight = Vector3.Distance(gate, right);
            float distanceLeft = Vector3.Distance(gate, left);
            if (Mathf.Abs(distanceRight - distanceLeft) > 2)
            {
                if (distanceRight > distanceLeft)
                {
                    angleCorection = -15f;
                }
                else
                {
                    angleCorection = 15f;
                }
            }
        }
        else if ((leftObstacleDetected || leftGroundDetected) && !(rightObstacleDetected && !rightGroundDetected) && turn <= 0f)
        {
            turn = 0.25f;
            if (leftGroundDetected)
            {
                turn = 0.5f;
            }
            if (readyForNextGate)
            {
                avoiding = 0.25f;
            }
        }
        else if ((rightObstacleDetected || rightGroundDetected) && !(leftObstacleDetected && !leftGroundDetected) && turn >= 0f)
        {
            turn = -0.25f;
            if (rightGroundDetected)
            {
                turn = -0.5f;
            }
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
        float maxSpeed = currentAiGate.maxSpeed;
        if (leftGroundDetector.isGroundDetected() && rightGroundDetector.isGroundDetected())
        {
            maxSpeed = Mathf.Min(maxSpeed, 100f);
        }
        bool needToBrake = false;
        GameObject closestCar = frontCarDetector.getClosestCar(gameObject.GetComponent<Transform>().position);
        if (closestCar != null) {
            Vector3 closestCarPosition = closestCar.GetComponent<Transform>().position, leftPosition = leftGroundDetector.getPossition(), rightPosition = rightGroundDetector.getPossition();
            closestCarPosition.y = 0;
            leftPosition.y = 0;
            rightPosition.y = 0;
            if (Vector3.Distance(closestCarPosition, rightPosition) <= Vector3.Distance(closestCarPosition, leftPosition) && closestCar.GetComponent<ISideFreeDetector>().getBaseGateDiffrence() < 5f && (goingToOvertake == true || speedKmh + speedCorection <= maxSpeed + 2))
            {
                if (closestCar.GetComponent<ISideFreeDetector>().isLeftSideFree(gameObject) && !leftGroundDetector.isGroundDetected())  
                {
                    turn = -0.2f;
                    goingToOvertake = true;
                    readyForNextGate = false;
                }
                else if(closestCar.GetComponent<ISideFreeDetector>().isRightSideFree(gameObject) && !rightGroundDetector.isGroundDetected())
                {
                    turn = 0.2f;
                    goingToOvertake = true;
                    readyForNextGate = false;
                }
                else
                {
                    needToBrake = (frontCarDetector.isCarDetected() && closestCar.GetComponent<Rigidbody>().velocity.magnitude * 3.6f < speedKmh);
                    goingToOvertake = false;
                }
            }
            else if (Vector3.Distance(closestCarPosition, rightPosition) > Vector3.Distance(closestCarPosition, leftPosition) && closestCar.GetComponent<ISideFreeDetector>().getBaseGateDiffrence() < 5f && (goingToOvertake == true || speedKmh + speedCorection <= maxSpeed + 2))
            {
                if (closestCar.GetComponent<ISideFreeDetector>().isRightSideFree(gameObject) && !rightGroundDetector.isGroundDetected())
                {
                    turn = 0.2f;
                    goingToOvertake = true;
                    readyForNextGate = false;
                }
                else if (closestCar.GetComponent<ISideFreeDetector>().isLeftSideFree(gameObject) && !leftGroundDetector.isGroundDetected())
                {
                    turn = -0.2f;
                    goingToOvertake = true;
                    readyForNextGate = false;
                }
                else
                {
                    needToBrake = (frontCarDetector.isCarDetected() && closestCar.GetComponent<Rigidbody>().velocity.magnitude * 3.6f < speedKmh);
                    goingToOvertake = false;
                }
            }
            else
            {
                needToBrake = (frontCarDetector.isCarDetected() && closestCar.GetComponent<Rigidbody>().velocity.magnitude * 3.6f < speedKmh);
                goingToOvertake = false;

            }
        }
        if (leftObstacleDetector.isObstacleDetected() && rightObstacleDetector.isObstacleDetected())
        {
            reversing = 1f;
        }

        if (reversing > 0f)
        {
            aiMovement.Reverse(false);
        }
        else if (speedKmh + speedCorection < maxSpeed && !needToBrake)
        {
            if (!aiMovement.isChangingGear())
            {
                aiMovement.Accelerate(false);
            }
        }
        else if(speedKmh + speedCorection > maxSpeed + 2 || needToBrake)
        {
            aiMovement.Break(false, false, turn);
        }
        else
        {
            aiMovement.Decelerate();
        }
        aiMovement.Turn(false, false, turn);
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("AiGate"))
        {
            currentAiGate = collider.gameObject.GetComponent<AiGate>();
            readyForNextGate = false;
            baseGateDifference = Mathf.Abs(Mathf.DeltaAngle(currentAiGate.getRotation(), currentAiGate.nextGate.getRotation()));
            avoiding = 0f;
            goingToOvertake = false;
            leftGroundDetector.changePossition(currentAiGate.cutState);
            rightGroundDetector.changePossition(currentAiGate.cutState);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("AI") || collision.gameObject.CompareTag("Player"))
        {
            readyForNextGate = false;
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
    public float getBaseGateDiffrence()
    {
        return baseGateDifference;
    }
}
