using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideDetector : MonoBehaviour
{
    private int leftDetections = 0;
    private int rightDetections = 0;
    void Start()
    {
        Collider collider = GetComponent<Collider>();

        Collider[] hits = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents);

        foreach (var hit in hits)
        {
            string hitName = hit.transform.name;
            if (hitName.StartsWith("SCutL"))
            {
                leftDetections++;
            }
            else if (hitName.StartsWith("SCutR"))
            {
                rightDetections++;
            }
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        string colliderName = collider.transform.name;
        if (colliderName.StartsWith("SCutL"))
        {
            leftDetections++;
        }
        else if (colliderName.StartsWith("SCutR"))
        {
            rightDetections++;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        string colliderName = collider.transform.name;
        if (colliderName.StartsWith("SCutL"))
        {
            leftDetections--;
        }
        else if (colliderName.StartsWith("SCutR"))
        {
            rightDetections--;
        }
    }
    public bool isLeftDetected()
    {
        return leftDetections > 0;
    }
    public bool isRightDetected()
    {
        return rightDetections > 0;
    }
    public void info()
    {
        Debug.Log(gameObject.name + " " + leftDetections + " " + rightDetections);
    }
}
