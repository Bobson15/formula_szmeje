using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDetector : MonoBehaviour
{
    private int detectedCars = 0;
    private Dictionary<Rigidbody, int> detectedCarsDict = new Dictionary<Rigidbody, int>();
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Car"))
        {
            Rigidbody rb = collider.GetComponentInParent<Rigidbody>();
            if (detectedCarsDict.ContainsKey(rb))
            {
                detectedCarsDict[rb]++;
            }
            else
            {
                detectedCars++;
                detectedCarsDict.Add(collider.GetComponentInParent<Rigidbody>(), 1);
            }
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Car") && detectedCarsDict.ContainsKey(collider.GetComponentInParent<Rigidbody>()))
        {
            Rigidbody rb = collider.GetComponentInParent<Rigidbody>();
            detectedCarsDict[rb]--;
            if (detectedCarsDict[rb] == 0) { 
                detectedCarsDict.Remove(rb);
                detectedCars--;
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
            var car = detectedCarsDict.GetEnumerator();
            car.MoveNext();
            speed = car.Current.Key.velocity.magnitude * 3.6f;
            while(car.MoveNext()) {
                speed = Mathf.Min(speed, car.Current.Key.velocity.magnitude * 3.6f);
            }
        }
        return speed;
    }
    public void getInfo()
    {
        Debug.Log(gameObject.name + " " + detectedCars);
    }
}
