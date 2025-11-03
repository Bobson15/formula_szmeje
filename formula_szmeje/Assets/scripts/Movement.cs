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
    private const float horsePower = 4500f;
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
    }
    void Accelerate()
    {
        float targetPower = horsePower - (horsePower / 10) * (gear-1);
        tireBackLCollider.motorTorque = targetPower;
        tireBackRCollider.motorTorque = targetPower;
        if (tireFrontLCollider.sidewaysFriction.stiffness > 1.9f && !playerInput.actions["Brake"].IsPressed())
        {
            WheelFrictionCurve forward = tireFrontLCollider.forwardFriction;
            WheelFrictionCurve sideways = tireFrontLCollider.sidewaysFriction;
            forward.stiffness = Mathf.Min(forward.stiffness + (3f * Time.deltaTime), 2.2f);
            sideways.stiffness = Mathf.Max(sideways.stiffness - (3f * Time.deltaTime), 1.9f);
            tireFrontLCollider.forwardFriction = forward;
            tireFrontLCollider.sidewaysFriction = sideways;
            tireFrontRCollider.forwardFriction = forward;
            tireFrontRCollider.sidewaysFriction = sideways;
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
            if (tireFrontLCollider.sidewaysFriction.stiffness < 2.6f)
            {
                WheelFrictionCurve forward = tireFrontLCollider.forwardFriction;
                WheelFrictionCurve sideways = tireFrontLCollider.sidewaysFriction;
                if (rb.velocity.magnitude * 3.6f > 100)
                {
                    forward.stiffness = Mathf.Max(forward.stiffness - (3f * Time.deltaTime), 1.5f);
                }
                else if(forward.stiffness > 1.75f)
                {
                    forward.stiffness = Mathf.Max(forward.stiffness - (3f * Time.deltaTime), 1.75f);
                }
                else if (forward.stiffness < 1.75f)
                {
                    forward.stiffness = Mathf.Min(forward.stiffness + (3f * Time.deltaTime), 1.75f);
                }
                sideways.stiffness = Mathf.Min(sideways.stiffness + (3f * Time.deltaTime), 2.6f);
                tireFrontLCollider.forwardFriction = forward;
                tireFrontLCollider.sidewaysFriction = sideways;
                tireFrontRCollider.forwardFriction = forward;
                tireFrontRCollider.sidewaysFriction = sideways;
            }
        }
        else
        {
            tireBackLCollider.brakeTorque = brakePower * 0.4f;
            tireBackRCollider.brakeTorque = brakePower * 0.4f;
            tireFrontLCollider.brakeTorque = brakePower * 0.6f;
            tireFrontRCollider.brakeTorque = brakePower * 0.6f;
            if (tireFrontLCollider.sidewaysFriction.stiffness > 1.9f)
            {
                WheelFrictionCurve forward = tireFrontLCollider.forwardFriction;
                WheelFrictionCurve sideways = tireFrontLCollider.sidewaysFriction;
                forward.stiffness = Mathf.Min(forward.stiffness + (3 * Time.deltaTime), 2.2f);
                sideways.stiffness = Mathf.Max(sideways.stiffness - (3f * Time.deltaTime), 1.9f);
                tireFrontLCollider.forwardFriction = forward;
                tireFrontLCollider.sidewaysFriction = sideways;
                tireFrontRCollider.forwardFriction = forward;
                tireFrontRCollider.sidewaysFriction = sideways;
            }
        }
    }

    void Reverse()
    {
        tireBackLCollider.motorTorque = -horsePower/6;
        tireBackRCollider.motorTorque = -horsePower/6;
        if (tireFrontLCollider.sidewaysFriction.stiffness > 1.9f && !playerInput.actions["Throttle"].IsPressed())
        {
            WheelFrictionCurve forward = tireFrontLCollider.forwardFriction;
            WheelFrictionCurve sideways = tireFrontLCollider.sidewaysFriction;
            forward.stiffness = Mathf.Min(forward.stiffness + (3f * Time.deltaTime), 2.2f);
            sideways.stiffness = Mathf.Max(sideways.stiffness - (3f * Time.deltaTime), 1.9f);
            tireFrontLCollider.forwardFriction = forward;
            tireFrontLCollider.sidewaysFriction = sideways;
            tireFrontRCollider.forwardFriction = forward;
            tireFrontRCollider.sidewaysFriction = sideways;
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
        if (tireFrontLCollider.sidewaysFriction.stiffness > 1.9f)
        {
            WheelFrictionCurve forward = tireFrontLCollider.forwardFriction;
            WheelFrictionCurve sideways = tireFrontLCollider.sidewaysFriction;
            forward.stiffness = Mathf.Min(forward.stiffness + (3f * Time.deltaTime), 2.2f);
            sideways.stiffness = Mathf.Max(sideways.stiffness - (3f * Time.deltaTime), 1.9f);
            tireFrontLCollider.forwardFriction = forward;
            tireFrontLCollider.sidewaysFriction = sideways;
            tireFrontRCollider.forwardFriction = forward;
            tireFrontRCollider.sidewaysFriction = sideways;
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