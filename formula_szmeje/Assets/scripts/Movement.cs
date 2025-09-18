using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    private const float topSpeed = 330f;
    private const float accelerationTime = 1f;
    private const float breakTime = 1.8f;
    private const float turnSpeed = 2f;
    private const float maxTurnAngle = 30f;
    private const float minTurnAngle = 22f;
    private const float reverseSpeed = 50f;
    private const float downforceCoefficient = 10f;
    public Transform tireFrontL;
    public Transform tireFrontR;
    public Transform contr;
    private Rigidbody rb;
    private PlayerInput playerInput;
    private int gear = 1;
    private float currentSpeed = 0f;
    private float changingGearTime = 0f;
    private bool isBreaking = false;
    private bool isBreakingR = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        rb.centerOfMass = new Vector3(0, -1, 0);
    }

    void Update()
    {


        if (playerInput.actions["Throttle"].IsPressed())
        {
            isBreaking = false;
            if (currentSpeed >= 0f)
            {
                if (changingGearTime <= 0 && !isBreakingR)
                {
                    Accelerate();
                }
            }
            else
            {
                Break();
                isBreakingR = true;
            }
        }
        else if (playerInput.actions["Brake"].IsPressed())
        {
            isBreakingR = false;
            if (currentSpeed > 0f)
            {
                Break();
                isBreaking = true;
            }
            else if (!isBreaking)
            {
                Reverse();
            }
        }
        else
        {
            isBreaking = false;
            isBreakingR = false;
            Decelerate();
        }
        if (rb.velocity.magnitude * 3.6f > 100 + 33 * (gear - 1) && gear < 8)
        {
            gear++;
            changingGearTime = 0.05f;
        }
        else if (rb.velocity.magnitude * 3.6f < 90 + 33 * (gear - 2) && gear > 1)
        {
            gear--;
            changingGearTime = 0.05f;
        }
        else if (changingGearTime > 0)
        {
            changingGearTime -= Time.deltaTime;
        }
        Turn();
    }
    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(0, Mathf.Pow(Mathf.Abs(rb.velocity.magnitude) * 3.6f, 2) * -downforceCoefficient * (rb.mass/10000), 0), ForceMode.Force);
    }

    void Accelerate()
    {
        float targetSpeed;
        targetSpeed = (110f + 33f * gear) * (1f - 0.15f * 0) / 3.6f;
        if (rb.velocity.magnitude * 3.6f < topSpeed)
        {
            if (gear > 1)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / ((gear-1) * accelerationTime * 0.5f));
            }
            else
            {
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / accelerationTime);
            }
        }
        Vector3 forwardVelocity = -transform.forward * currentSpeed;
        rb.velocity = new Vector3(forwardVelocity.x, rb.velocity.y, forwardVelocity.z);
    }
    void Break()
    {
        if (currentSpeed > 0)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, -20, Time.deltaTime / (breakTime));
            if (currentSpeed < 0)
            {
                currentSpeed = 0;
            }
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 20, Time.deltaTime / (breakTime * ((rb.velocity.magnitude * 3.6f) / topSpeed)));
            if (currentSpeed > 0)
            {
                currentSpeed = 0;
            }
        }
        Vector3 forwardVelocity = -transform.forward * currentSpeed;
        rb.velocity = new Vector3(forwardVelocity.x, rb.velocity.y, forwardVelocity.z);
    }

    void Reverse()
    {
        float targetSpeed;
        if (0 == 0)
        {
            targetSpeed = -reverseSpeed / 3.6f;
        }
        else
        {
            targetSpeed = -reverseSpeed * 0.8f / 3.6f;
        }
        if (rb.velocity.magnitude < (-targetSpeed) / 2)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / accelerationTime);
        }
        Vector3 reverseVelocity = -transform.forward * currentSpeed;
        rb.velocity = new Vector3(reverseVelocity.x, rb.velocity.y, reverseVelocity.z);
    }

    void Decelerate()
    {
        if (currentSpeed > 0)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, -20, Time.deltaTime / (accelerationTime * 6));
            if (currentSpeed < 0)
            {
                currentSpeed = 0;
            }
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 20, Time.deltaTime / (accelerationTime * 6));
            if (currentSpeed > 0)
            {
                currentSpeed = 0;
            }
        }
        Vector3 forwardVelocity = -transform.forward * currentSpeed;
        rb.velocity = new Vector3(forwardVelocity.x, rb.velocity.y, forwardVelocity.z);
    }

    void Turn()
    {
        float turnAngle = 0;
        if (playerInput.actions["TurnLeft"].IsPressed() && !playerInput.actions["TurnRight"].IsPressed())
        {
            turnAngle = -1 * ((maxTurnAngle - minTurnAngle) * ((topSpeed - rb.velocity.magnitude * 3.6f) / topSpeed) + minTurnAngle);
            AnimateWheels(-1);
        }
        else if (!playerInput.actions["TurnLeft"].IsPressed() && playerInput.actions["TurnRight"].IsPressed())
        {
            turnAngle = 1 * ((maxTurnAngle - minTurnAngle) * ((topSpeed - rb.velocity.magnitude * 3.6f) / topSpeed) + minTurnAngle);
            AnimateWheels(1);
        }
        else if (playerInput.actions["Turn"].IsPressed()) {
            turnAngle = playerInput.actions["Turn"].ReadValue<Vector2>().x * ((maxTurnAngle - minTurnAngle) * ((topSpeed - rb.velocity.magnitude * 3.6f) / topSpeed) + minTurnAngle);
            AnimateWheels(playerInput.actions["Turn"].ReadValue<Vector2>().x);
        }
        else
        {
            AnimateWheels(0);
        }
        if (Mathf.Abs(currentSpeed) >= 2)
        {
            Quaternion turnRotation = Quaternion.Euler(0, turnAngle * Time.deltaTime * turnSpeed, 0);
            if (Input.GetKey(KeyCode.S) && !isBreaking)
            {
                turnRotation = Quaternion.Euler(0, -turnAngle * Time.deltaTime * turnSpeed, 0);
            }
            rb.MoveRotation(rb.rotation * turnRotation);

            Vector3 forwardVelocity = -transform.forward * currentSpeed;
            rb.velocity = new Vector3(forwardVelocity.x, rb.velocity.y, forwardVelocity.z);
            }
    }

    void AnimateWheels(float angle)
    {
        float turnAngle = angle * ((maxTurnAngle - minTurnAngle) * ((topSpeed - rb.velocity.magnitude * 3.6f) / topSpeed) + minTurnAngle);

        if (tireFrontL != null && tireFrontR != null)
        {
            Quaternion wheelTurn = Quaternion.Euler(0, turnAngle, 0);
            tireFrontL.localRotation = wheelTurn;
            tireFrontR.localRotation = wheelTurn;
        }

        if (contr != null)
        {
            float steeringAngle = angle * 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, steeringAngle);
            contr.localRotation = Quaternion.Lerp(contr.localRotation, targetRotation, Time.deltaTime * 5f);
        }

        float wheelSpinSpeed = Mathf.Abs(currentSpeed) * 5f;

        if (tireFrontL != null)
            tireFrontL.Rotate(Vector3.right, wheelSpinSpeed * Time.deltaTime);
        if (tireFrontR != null)
            tireFrontR.Rotate(Vector3.right, wheelSpinSpeed * Time.deltaTime);
    }
}