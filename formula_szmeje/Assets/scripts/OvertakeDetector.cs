using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class OvertakeDetector : MonoBehaviour
{
    private int detectedBariers = 0, detectedCars = 0;
    private Dictionary<GameObject, int> detectedCarsDict = new Dictionary<GameObject, int>();
    public LayerMask trackLayer;
    public float rayLength = 5f;
    private Terrain terrain;
    public int raycastPosition = 0;
    void Start()
    {
        terrain = GameObject.Find("Monza").GetComponent<Terrain>();
        Collider collider = GetComponent<Collider>();

        Collider[] hits = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents);

        foreach (var hit in hits)
        {
            if (hit.transform.root.CompareTag("bariers"))
            {
                detectedBariers++;
            }
            else if (hit.CompareTag("Car"))
            {
                GameObject rb = hit.transform.parent.gameObject;
                if (detectedCarsDict.ContainsKey(rb))
                {
                    detectedCarsDict[rb]++;
                }
                else
                {
                    detectedCars++;
                    detectedCarsDict.Add(rb, 1);
                }
            }
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.root.CompareTag("bariers"))
        {
            detectedBariers++;
        }
        else if (collider.CompareTag("Car"))
        {
            GameObject rb = collider.transform.parent.gameObject;
            if (detectedCarsDict.ContainsKey(rb))
            {
                detectedCarsDict[rb]++;
            }
            else
            {
                detectedCars++;
                detectedCarsDict.Add(rb, 1);
            }
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if ( collider.transform.root.CompareTag("bariers"))
        {
            if(--detectedBariers < 0)
            {
                detectedBariers = 0;
            }
        }
        else if (collider.CompareTag("Car") && detectedCarsDict.ContainsKey(collider.transform.parent.gameObject))
        {
            GameObject car = collider.transform.parent.gameObject;
            detectedCarsDict[car]--;
            if (detectedCarsDict[car] == 0)
            {
                detectedCarsDict.Remove(car);
                if (--detectedCars < 0)
                {
                    detectedCars = 0;
                }
            }
        }
    }
    public bool canOvertake(GameObject overtakingCar)
    {
        if (detectedBariers > 0) {
            return false;
        }
        if(detectedCars >=2 || (detectedCars == 1 && !detectedCarsDict.ContainsKey(overtakingCar)))
        {
            return false;
        }
        bool groundDetected = true;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.forward * 7f + transform.right * raycastPosition * 1.8f, Vector3.down, out hit, rayLength, trackLayer))
        {
            float terrainHeight = terrain.SampleHeight(hit.point);
            if (hit.point.y > terrainHeight)
            {
                groundDetected = false;
            }
        }
        return !groundDetected;
    }
}
