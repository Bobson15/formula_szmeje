using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDetector : MonoBehaviour
{
    private int detectedCars = 0;
    private Dictionary<GameObject, int> detectedCarsDict = new Dictionary<GameObject, int>();
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Car"))
        {
            GameObject rb = collider.transform.parent.gameObject;
            if (detectedCarsDict.ContainsKey(rb))
            {
                detectedCarsDict[rb]++;
            }
            else
            {
                detectedCars++;
                detectedCarsDict.Add(collider.transform.parent.gameObject, 1);
            }
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Car") && detectedCarsDict.ContainsKey(collider.transform.parent.gameObject))
        {
            GameObject car = collider.transform.parent.gameObject;
            detectedCarsDict[car]--;
            if (detectedCarsDict[car] == 0) { 
                detectedCarsDict.Remove(car);
                detectedCars--;
            }
        }
    }
    public bool isCarDetected()
    {
        return detectedCars > 0;
    }
    public GameObject getClosestCar(Vector3 position)
    {
        if (detectedCars == 0)
        {
            return null;
        }
        else
        {
            var car = detectedCarsDict.GetEnumerator();
            car.MoveNext();
            GameObject closestCar = car.Current.Key;
            Vector3 clocestCarPosition = closestCar.transform.position;
            clocestCarPosition.y = 0;
            position.y = 0;
            while (car.MoveNext())
            {
                Vector3 carPosition = car.Current.Key.transform.position;
                carPosition.y = 0;
                if (Vector3.Distance(position, clocestCarPosition) > Vector3.Distance(position, carPosition)){
                    closestCar = car.Current.Key;
                    clocestCarPosition = carPosition;
                }
            }
            return closestCar;
        }
    }
    public void getInfo()
    {
        Debug.Log(gameObject.name + " " + detectedCars);
    }
}
