using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDetector : MonoBehaviour
{
    private int carContacts = 0;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("AI") || collider.CompareTag("Player"))
        {
            carContacts++;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("AI") || collider.CompareTag("Player"))
        {
            carContacts--;
        }
    }
    public bool isCarDetected()
    {
        return carContacts > 0;
    }
    public void getInfo()
    {
        Debug.Log(gameObject.name + " " + carContacts);
    }
}
