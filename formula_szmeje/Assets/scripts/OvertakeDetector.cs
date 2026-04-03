using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvertakeDetector : MonoBehaviour
{
    private int detectedProblems = 0;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Car")|| collider.transform.root.CompareTag("bariers"))
        {
            detectedProblems++;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Car") || collider.transform.root.CompareTag("bariers"))
        {
            detectedProblems = Math.Max(detectedProblems - 1, 0);
        }
    }
    public bool canOvertake()
    {
        return detectedProblems == 0;
    }
}
