using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class movement : MonoBehaviour
{
    public float horsepower = 1000f;
    public float topSpeed = 350f;
    public float accelerationTime = 2.5f;
    public float turnSpeed = 2f;
    public float maxTurnAngle = 30f;
    public float reverseSpeed = 100f;

    public Transform tireFrontL;
    public Transform tireFrontR;
    public Transform contr;
    public float wheelTurnAngle = 10f;

    private Rigidbody rb;
    private float currentSpeed = 0f;
    private float horizontalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -1, 0);
    }

    void Update()
    {
        // Input
        horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.W))
        {
            Accelerate();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Reverse();
        }
        else
        {
            Decelerate();
        }

        Turn(horizontalInput);
        AnimateWheels();
    }

    void Accelerate()
    {
        float targetSpeed = topSpeed / 3.6f;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / accelerationTime);
        Vector3 forwardVelocity = -transform.forward * currentSpeed;
        rb.velocity = new Vector3(forwardVelocity.x, rb.velocity.y, forwardVelocity.z);
    }

    void Reverse()
    {
        float targetSpeed = -reverseSpeed / 3.6f;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / accelerationTime);
        Vector3 reverseVelocity = -transform.forward * currentSpeed;
        rb.velocity = new Vector3(reverseVelocity.x, rb.velocity.y, reverseVelocity.z);
    }

    void Decelerate()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime / accelerationTime);
        Vector3 forwardVelocity = -transform.forward * currentSpeed;
        rb.velocity = new Vector3(forwardVelocity.x, rb.velocity.y, forwardVelocity.z);
    }

    void Turn(float horizontalInput)
    {
        float turnAngle = horizontalInput * maxTurnAngle;
        Quaternion turnRotation = Quaternion.Euler(0, turnAngle * Time.deltaTime * turnSpeed, 0);
        rb.MoveRotation(rb.rotation * turnRotation);

        Vector3 forwardVelocity = -transform.forward * currentSpeed;
        rb.velocity = new Vector3(forwardVelocity.x, rb.velocity.y, forwardVelocity.z);
    }

    void AnimateWheels()
    {
        float turnAngle = horizontalInput * maxTurnAngle;

        if (tireFrontL != null && tireFrontR != null)
        {
            Quaternion wheelTurn = Quaternion.Euler(0, turnAngle, 0);
            tireFrontL.localRotation = wheelTurn;
            tireFrontR.localRotation = wheelTurn;
        }

        if (contr != null)
        {
            float steeringAngle = horizontalInput * 90f;
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