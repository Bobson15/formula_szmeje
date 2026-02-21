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
        if (collider == null)
        {
            return;
        }
        Transform root = collider.transform.root;
        if (root != null && root.CompareTag("Ground"))
        {
            groundDetected = true;
            return;
        }
        Transform parent = collider.transform.parent;
        if (parent != null && parent.CompareTag("Track"))
        {
            trackDetections++;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider == null)
        {
            return;
        }
        Transform root = collider.transform.root;
        if (root != null && root.CompareTag("Ground"))
        {
            groundDetected = false;
            return;
        }
        Transform parent = collider.transform.parent;
        if (parent != null && parent.CompareTag("Track"))
        {
            trackDetections--;
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
