using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiLapCounter : MonoBehaviour, ILapCounter
{
    private int lapCounter = 1;
    private int sector = 1;
    public int getLaps()
    {
        return lapCounter;
    }

    public void TriggerLapStart()
    {
        if (sector == 3)
        {
            lapCounter++;
            sector = 1;
        }
    }

    public void TriggerSector(int sectorNumber)
    {
        if(sectorNumber == 2 && sector == 1)
        {
            sector = 2;
        }
        else if (sectorNumber == 3 && sector == 2)
        {
            sector = 3;
        }
    }
}
