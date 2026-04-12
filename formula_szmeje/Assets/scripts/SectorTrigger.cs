using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorTrigger : MonoBehaviour
{
    public int sectorNumber;

    private void OnTriggerEnter(Collider other)
    {
        ILapCounter timer = other.transform.parent.GetComponent<ILapCounter>();

        if (timer == null) return;
        if (other.transform.parent.CompareTag("AI"))
        {
            Debug.Log("przed");
        }
        if (sectorNumber == 0 && (other.transform.parent.CompareTag("AI") || !((LapTimer)timer).isTimerBlocked()))
        {
            if (other.transform.parent.CompareTag("AI"))
            {
                Debug.Log("po");
            }
            timer.TriggerLapStart();
        }
        else
            timer.TriggerSector(sectorNumber);
    }
    
}