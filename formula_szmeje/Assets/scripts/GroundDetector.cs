using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    private bool groundDetected = false;
    private int trackDetections = 0;
    private void OnTriggerEnter(Collider collider)
    {
        try
        {
            if (collider.transform.root.CompareTag("Ground"))
            {
                groundDetected = true;
            }
            else if (collider.transform.parent.CompareTag("Track"))
            {
                trackDetections++;
            }
        }
        catch
        {

        }
    }
    private void OnTriggerExit(Collider collider)
    {
        try
        {
            if (collider.transform.root.CompareTag("Ground"))
            {
                groundDetected = false;
            }
            else if (collider.transform.parent.CompareTag("Track"))
            {
                trackDetections--;
            }
        }
        catch
        {

        }
    }
    public bool isGroundDetected()
    {
        return groundDetected && trackDetections == 0;
    }
    public void info()
    {
        Debug.Log(gameObject.name + " " + groundDetected + " " + trackDetections);
    }
}
