using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDetector : MonoBehaviour
{
    private int detectedCars = 0;
    private List<Rigidbody> detectedCarsList = new List<Rigidbody>() { };
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("AI") || collider.CompareTag("Player"))
        {
            detectedCars++;
            detectedCarsList.Add(collider.GetComponent<Rigidbody>());
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("AI") || collider.CompareTag("Player"))
        {
            detectedCars--;
            foreach(Rigidbody rb  in detectedCarsList)
            {
                if(rb == collider.GetComponent<Rigidbody>())
                {
                    detectedCarsList.Remove(rb);
                    break;
                }
            }
        }
    }
    public bool isCarDetected()
    {
        return detectedCars > 0;
    }
    public float getDetectedCarSpeed()
    {
        float speed = 0;
        if (detectedCars > 0)
        {
            speed = detectedCarsList[0].velocity.magnitude * 3.6f;
            for (int i = 1; i < detectedCarsList.Count; i++) {
                speed = Mathf.Min(speed, detectedCarsList[i].velocity.magnitude * 3.6f);
            }
        }
        return speed;
    }
    public void getInfo()
    {
        Debug.Log(gameObject.name + " " + detectedCars);
    }
}
