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
        if (sectorNumber == 0 && (other.transform.parent.CompareTag("AI") || !((LapTimer)timer).isTimerBlocked()))
        {
            timer.TriggerLapStart();
        }
        else
            timer.TriggerSector(sectorNumber);
    }
    
}