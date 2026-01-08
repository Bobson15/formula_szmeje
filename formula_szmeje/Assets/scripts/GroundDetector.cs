using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    private bool groundDetected = false;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.root.CompareTag("Ground"))
        {
            groundDetected = true;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.transform.root.CompareTag("Ground"))
        {
            groundDetected = false;
        }
    }
    public bool isGroundDetected()
    {
        return groundDetected;
    }
    public void info()
    {
        Debug.Log(gameObject.name + " " + groundDetected);
    }
}
