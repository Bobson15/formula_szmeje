using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILapCounter
{
    public int getLaps();
    public void TriggerLapStart();
    public void TriggerSector(int sectorNumber);
}
