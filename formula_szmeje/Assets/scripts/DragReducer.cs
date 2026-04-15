using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragReducer : MonoBehaviour
{
    private int detectedCars = 0;
    private Rigidbody rb;
    void Start()
    {
        rb = gameObject.transform.parent.GetComponent<Rigidbody>();

        Collider collider = GetComponent<Collider>();

        Collider[] hits = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Car"))
            {
                detectedCars++;
            }
        }
        if (detectedCars > 0)
        {
            rb.drag = 0.09f;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            detectedCars++;
            if(detectedCars == 1)
            {
                rb.drag = 0.09f;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            detectedCars--;
            if (detectedCars < 0)
            {
                detectedCars = 0;
            }
            if (detectedCars == 0)
            {
                rb.drag = 0.09f;
            }
        }
    }
}
