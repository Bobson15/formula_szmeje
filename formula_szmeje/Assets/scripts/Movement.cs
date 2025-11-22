using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class Movement : MonoBehaviour
{
    private const float topSpeed = 330f;
    private const float horsePower = 4800f;
    private const float brakePower = 12000f;
    private const float turnSpeed = 2f;
    private const float maxTurnAngle = 32f;
    private const float minTurnAngle = 21f;
    private const float downforceCoefficient = 20f;
    public Transform tireFrontL;
    public Transform tireFrontR;
    public Transform tireBackL;
    public Transform tireBackR;
    public WheelCollider tireFrontLCollider;
    public WheelCollider tireFrontRCollider;
    public WheelCollider tireBackLCollider;
    public WheelCollider tireBackRCollider;
    public Transform contr;
    private Rigidbody rb;
    private PlayerInput playerInput;
    private int gear = 1;
    private float changingGearTime = 0f;
    private CarState carState = CarState.Stop;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        rb.centerOfMass = new Vector3(0, -1, 0);
    }

    void FixedUpdate()
    {
        Turn();
        if (playerInput.actions["Throttle"].IsPressed())
        {
            if (carState != CarState.Backward)
            {
                if (changingGearTime <= 0f)
                {
                    Accelerate();
                }
                carState = CarState.Forward;
            }
            else
            {
                Break();
                if(rb.velocity.magnitude < 0.1)
                {
                    carState = CarState.Stop;
                }
            }
        }
        if (playerInput.actions["Brake"].IsPressed())
        {
            if (carState == CarState.Forward)
            {
                Break();
                if (rb.velocity.magnitude < 0.1)
                {
                    carState = CarState.Stop;
                }
            }
            else
            {
                Reverse();
                carState = CarState.Backward;
            }
        }
        if(!playerInput.actions["Throttle"].IsPressed()&&!playerInput.actions["Brake"].IsPressed())
        {
            Decelerate();
            if (rb.velocity.magnitude < 0.1)
            {
                carState = CarState.Stop;
            }
        }
        if (rb.velocity.magnitude * 3.6f > 100 + 35 * (gear - 1) && gear < 8)
        {
            gear++;
            changingGearTime = 0.05f;
        }
        else if (rb.velocity.magnitude * 3.6f < 90 + 35 * (gear - 2) && gear > 1)
        {
            gear--;
            changingGearTime = 0.05f;
        }
        else if (changingGearTime > 0)
        {
            changingGearTime -= Time.deltaTime;
        }
        rb.AddForce(new Vector3(0, Mathf.Pow(Mathf.Abs(rb.velocity.magnitude) * 3.6f, 2) * -downforceCoefficient * 0.01f, 0), ForceMode.Force);
        WheelHit frontLeftHit;
        WheelFrictionCurve forwardFrontLeft = tireFrontLCollider.forwardFriction;
        WheelFrictionCurve forwardFrontRight = tireFrontRCollider.forwardFriction;
        WheelFrictionCurve forwardBackLeft = tireBackLCollider.forwardFriction;
        WheelFrictionCurve forwardBackRight = tireBackRCollider.forwardFriction;
        WheelFrictionCurve forwardOfTrack = tireFrontLCollider.forwardFriction;
        forwardOfTrack.stiffness = 1.3f;
        if (tireFrontLCollider.GetGroundHit(out frontLeftHit))
        {
            if (frontLeftHit.collider.gameObject.CompareTag("Ground"))
            {
                tireFrontLCollider.forwardFriction = forwardOfTrack;
            }
            else if(forwardFrontLeft.stiffness == forwardOfTrack.stiffness)
            {
                forwardFrontLeft.stiffness = 2.2f;
                tireFrontLCollider.forwardFriction = forwardFrontLeft;
            }
        }
        WheelHit frontRightHit;
        if (tireFrontRCollider.GetGroundHit(out frontRightHit))
        {
            if (frontRightHit.collider.gameObject.CompareTag("Ground"))
            {
                tireFrontRCollider.forwardFriction = forwardOfTrack;
            }
            else if(forwardFrontRight.stiffness == forwardOfTrack.stiffness)
            {
                forwardFrontRight.stiffness = 2.2f;
                tireFrontRCollider.forwardFriction = forwardFrontRight;
            }
        }
        WheelHit backLeftHit;
        if (tireBackLCollider.GetGroundHit(out backLeftHit))
        {
            if (backLeftHit.collider.gameObject.CompareTag("Ground"))
            {
                tireBackLCollider.forwardFriction = forwardOfTrack;
            }
            else if(forwardBackLeft.stiffness == forwardOfTrack.stiffness)
            {
                forwardBackLeft.stiffness = 2.2f;
                tireBackLCollider.forwardFriction = forwardBackLeft;
            }
        }
        WheelHit backRightHit;
        if (tireBackRCollider.GetGroundHit(out backRightHit))
        {
            if (backRightHit.collider.gameObject.CompareTag("Ground"))
            {
                tireBackRCollider.forwardFriction = forwardOfTrack;
            }
            else if (forwardBackRight.stiffness == forwardOfTrack.stiffness)
            {
                forwardBackRight.stiffness = 2.2f;
                tireBackRCollider.forwardFriction = forwardBackRight;
            }
        }
    }
    void Accelerate()
    {
        float targetPower = horsePower - (horsePower / 10) * (gear-1);
        tireBackLCollider.motorTorque = targetPower;
        tireBackRCollider.motorTorque = targetPower;
        if ((tireFrontLCollider.sidewaysFriction.stiffness > 1.9f || tireFrontRCollider.sidewaysFriction.stiffness > 1.9f) && !playerInput.actions["Brake"].IsPressed())
        {
            WheelFrictionCurve forwardLeft = tireFrontLCollider.forwardFriction;
            WheelFrictionCurve forwardRight = tireFrontRCollider.forwardFriction;
            WheelFrictionCurve sidewaysLeft = tireFrontLCollider.sidewaysFriction;
            WheelFrictionCurve sidewaysRight = tireFrontRCollider.sidewaysFriction;
            forwardLeft.stiffness = Mathf.Min(forwardLeft.stiffness + (3f * Time.deltaTime), 2.2f);
            forwardRight.stiffness = Mathf.Min(forwardRight.stiffness + (3f * Time.deltaTime), 2.2f);
            sidewaysLeft.stiffness = Mathf.Max(sidewaysLeft.stiffness - (3f * Time.deltaTime), 1.9f);
            sidewaysRight.stiffness = Mathf.Max(sidewaysRight.stiffness - (3f * Time.deltaTime), 1.9f);
            tireFrontLCollider.forwardFriction = forwardLeft;
            tireFrontLCollider.sidewaysFriction = sidewaysLeft;
            tireFrontRCollider.forwardFriction = forwardRight;
            tireFrontRCollider.sidewaysFriction = sidewaysRight;
        }
    }
    void Break()
    {
        if ((playerInput.actions["TurnLeft"].IsPressed() && !playerInput.actions["TurnRight"].IsPressed()) || (!playerInput.actions["TurnLeft"].IsPressed() && playerInput.actions["TurnRight"].IsPressed()) || Mathf.Abs(playerInput.actions["Turn"].ReadValue<Vector2>().x) > 0.1f)
        {
            tireBackLCollider.brakeTorque = brakePower * 0.2f;
            tireBackRCollider.brakeTorque = brakePower * 0.2f;
            tireFrontLCollider.brakeTorque = brakePower * 0.3f;
            tireFrontRCollider.brakeTorque = brakePower * 0.3f;
            if (tireFrontLCollider.sidewaysFriction.stiffness < 2.6f || tireFrontRCollider.sidewaysFriction.stiffness < 2.6f)
            {
                WheelFrictionCurve forwardLeft = tireFrontLCollider.forwardFriction;
                WheelFrictionCurve forwardRight = tireFrontRCollider.forwardFriction;
                WheelFrictionCurve sidewaysLeft = tireFrontLCollider.sidewaysFriction;
                WheelFrictionCurve sidewaysRight = tireFrontRCollider.sidewaysFriction;
                if (rb.velocity.magnitude * 3.6f > 100)
                {
                    forwardLeft.stiffness = Mathf.Max(forwardLeft.stiffness - (3f * Time.deltaTime), 1.5f);
                    forwardRight.stiffness = Mathf.Max(forwardRight.stiffness - (3f * Time.deltaTime), 1.5f);
                }
                else
                {
                    if (forwardLeft.stiffness > 1.75f)
                    {
                        forwardLeft.stiffness = Mathf.Max(forwardLeft.stiffness - (3f * Time.deltaTime), 1.75f);
                    }
                    else if (forwardLeft.stiffness < 1.75f)
                    {
                        forwardLeft.stiffness = Mathf.Min(forwardLeft.stiffness + (3f * Time.deltaTime), 1.75f);
                    }
                    if (forwardRight.stiffness > 1.75f)
                    {
                        forwardRight.stiffness = Mathf.Max(forwardRight.stiffness - (3f * Time.deltaTime), 1.75f);
                    }
                    else if (forwardRight.stiffness < 1.75f)
                    {
                        forwardRight.stiffness = Mathf.Min(forwardRight.stiffness + (3f * Time.deltaTime), 1.75f);
                    }
                }
                sidewaysLeft.stiffness = Mathf.Min(sidewaysLeft.stiffness + (3f * Time.deltaTime), 2.6f);
                sidewaysRight.stiffness = Mathf.Min(sidewaysRight.stiffness + (3f * Time.deltaTime), 2.6f);
                tireFrontLCollider.forwardFriction = forwardLeft;
                tireFrontLCollider.sidewaysFriction = sidewaysLeft;
                tireFrontRCollider.forwardFriction = forwardRight;
                tireFrontRCollider.sidewaysFriction = sidewaysRight;
            }
        }
        else
        {
            tireBackLCollider.brakeTorque = brakePower * 0.4f;
            tireBackRCollider.brakeTorque = brakePower * 0.4f;
            tireFrontLCollider.brakeTorque = brakePower * 0.6f;
            tireFrontRCollider.brakeTorque = brakePower * 0.6f;
            if (tireFrontLCollider.sidewaysFriction.stiffness > 1.9f || tireFrontRCollider.sidewaysFriction.stiffness > 1.9f)
            {
                WheelFrictionCurve forwardLeft = tireFrontLCollider.forwardFriction;
                WheelFrictionCurve forwardRight = tireFrontRCollider.forwardFriction;
                WheelFrictionCurve sidewaysLeft = tireFrontLCollider.sidewaysFriction;
                WheelFrictionCurve sidewaysRight = tireFrontRCollider.sidewaysFriction;
                forwardLeft.stiffness = Mathf.Min(forwardLeft.stiffness + (3f * Time.deltaTime), 2.2f);
                forwardRight.stiffness = Mathf.Min(forwardRight.stiffness + (3f * Time.deltaTime), 2.2f);
                sidewaysLeft.stiffness = Mathf.Max(sidewaysLeft.stiffness - (3f * Time.deltaTime), 1.9f);
                sidewaysRight.stiffness = Mathf.Max(sidewaysRight.stiffness - (3f * Time.deltaTime), 1.9f);
                tireFrontLCollider.forwardFriction = forwardLeft;
                tireFrontLCollider.sidewaysFriction = sidewaysLeft;
                tireFrontRCollider.forwardFriction = forwardRight;
                tireFrontRCollider.sidewaysFriction = sidewaysRight;
            }
        }
    }

    void Reverse()
    {
        tireBackLCollider.motorTorque = -horsePower/6;
        tireBackRCollider.motorTorque = -horsePower/6;
        if ((tireFrontLCollider.sidewaysFriction.stiffness > 1.9f || tireFrontRCollider.sidewaysFriction.stiffness > 1.9f) && !playerInput.actions["Throttle"].IsPressed())
        {
            WheelFrictionCurve forwardLeft = tireFrontLCollider.forwardFriction;
            WheelFrictionCurve forwardRight = tireFrontRCollider.forwardFriction;
            WheelFrictionCurve sidewaysLeft = tireFrontLCollider.sidewaysFriction;
            WheelFrictionCurve sidewaysRight = tireFrontRCollider.sidewaysFriction;
            forwardLeft.stiffness = Mathf.Min(forwardLeft.stiffness + (3f * Time.deltaTime), 2.2f);
            forwardRight.stiffness = Mathf.Min(forwardRight.stiffness + (3f * Time.deltaTime), 2.2f);
            sidewaysLeft.stiffness = Mathf.Max(sidewaysLeft.stiffness - (3f * Time.deltaTime), 1.9f);
            sidewaysRight.stiffness = Mathf.Max(sidewaysRight.stiffness - (3f * Time.deltaTime), 1.9f);
            tireFrontLCollider.forwardFriction = forwardLeft;
            tireFrontLCollider.sidewaysFriction = sidewaysLeft;
            tireFrontRCollider.forwardFriction = forwardRight;
            tireFrontRCollider.sidewaysFriction = sidewaysRight;
        }
    }

    void Decelerate()
    {
        tireBackLCollider.motorTorque = 0;
        tireBackRCollider.motorTorque = 0;
        tireBackLCollider.brakeTorque = 0;
        tireBackRCollider.brakeTorque = 0;
        tireFrontLCollider.brakeTorque = 0;
        tireFrontRCollider.brakeTorque = 0;
        if (tireFrontLCollider.sidewaysFriction.stiffness > 1.9f || tireFrontRCollider.sidewaysFriction.stiffness > 1.9f)
        {
            WheelFrictionCurve forwardLeft = tireFrontLCollider.forwardFriction;
            WheelFrictionCurve forwardRight = tireFrontRCollider.forwardFriction;
            WheelFrictionCurve sidewaysLeft = tireFrontLCollider.sidewaysFriction;
            WheelFrictionCurve sidewaysRight = tireFrontRCollider.sidewaysFriction;
            forwardLeft.stiffness = Mathf.Min(forwardLeft.stiffness + (3f * Time.deltaTime), 2.2f);
            forwardRight.stiffness = Mathf.Min(forwardRight.stiffness + (3f * Time.deltaTime), 2.2f);
            sidewaysLeft.stiffness = Mathf.Max(sidewaysLeft.stiffness - (3f * Time.deltaTime), 1.9f);
            sidewaysRight.stiffness = Mathf.Max(sidewaysRight.stiffness - (3f * Time.deltaTime), 1.9f);
            tireFrontLCollider.forwardFriction = forwardLeft;
            tireFrontLCollider.sidewaysFriction = sidewaysLeft;
            tireFrontRCollider.forwardFriction = forwardRight;
            tireFrontRCollider.sidewaysFriction = sidewaysRight;
        }
    }

    void Turn()
    {
        float turnAngle = 0;
        float stearingWheelTurnAngle = 0;
        if (playerInput.actions["TurnLeft"].IsPressed() && !playerInput.actions["TurnRight"].IsPressed())
        {
            turnAngle = -1 * ((maxTurnAngle - minTurnAngle) * ((topSpeed - rb.velocity.magnitude * 3.6f) / topSpeed) + minTurnAngle);
            stearingWheelTurnAngle = -1;
        }
        else if (!playerInput.actions["TurnLeft"].IsPressed() && playerInput.actions["TurnRight"].IsPressed())
        {
            turnAngle = 1 * ((maxTurnAngle - minTurnAngle) * ((topSpeed - rb.velocity.magnitude * 3.6f) / topSpeed) + minTurnAngle);
            stearingWheelTurnAngle = 1;
        }
        else if (playerInput.actions["Turn"].IsPressed()) {
            turnAngle = playerInput.actions["Turn"].ReadValue<Vector2>().x * ((maxTurnAngle - minTurnAngle) * ((topSpeed - rb.velocity.magnitude * 3.6f) / topSpeed) + minTurnAngle);
            stearingWheelTurnAngle = playerInput.actions["Turn"].ReadValue<Vector2>().x;
        }
        tireFrontLCollider.steerAngle = turnAngle;
        tireFrontRCollider.steerAngle = turnAngle;
        tireFrontLCollider.GetWorldPose(out Vector3 tireFrontLPosition, out Quaternion tireFrontLRotation);
        tireFrontL.position = tireFrontLPosition;
        tireFrontL.rotation = tireFrontLRotation;
        tireFrontRCollider.GetWorldPose(out Vector3 tireFrontRPosition, out Quaternion tireFrontRRotation);
        tireFrontR.position = tireFrontRPosition;
        tireFrontR.rotation = tireFrontRRotation;
        tireBackLCollider.GetWorldPose(out Vector3 tireBackLPosition, out Quaternion tireBackLRotation);
        tireBackL.position = tireBackLPosition;
        tireBackL.rotation = tireBackLRotation;
        tireBackRCollider.GetWorldPose(out Vector3 tireBackRPosition, out Quaternion tireBackRRotation);
        tireBackR.position = tireBackRPosition;
        tireBackR.rotation = tireBackRRotation;

        Quaternion targetRotation = Quaternion.Euler(0, 0, -stearingWheelTurnAngle * 90f);
        contr.localRotation = Quaternion.Lerp(contr.localRotation, targetRotation, Time.deltaTime * (turnSpeed * 4));
    }
    private enum CarState
    {
        Backward = -1,
        Stop = 0,
        Forward = 1
    }
}