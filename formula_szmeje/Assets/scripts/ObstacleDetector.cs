using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    private int barierContacts = 0;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.root.CompareTag("bariers"))
        {
            barierContacts++;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.transform.root.CompareTag("bariers"))
        {
            barierContacts--;
        }
    }
    public bool isObstacleDetected()
    {
        return barierContacts > 0;
    }
    public void getInfo()
    {
        Debug.Log(gameObject.name + " " + barierContacts);
    }
}
