using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorTrigger : MonoBehaviour
{
    public int sectorNumber;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        LapTimer timer = FindObjectOfType<LapTimer>();
        if (timer == null) return;

        if (sectorNumber == 0)
            timer.TriggerLapStart();
        else
            timer.TriggerSector(sectorNumber);

        Debug.Log("Wjechano w sektor: " + sectorNumber);    
    }
    
}