using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISideFreeDetector
{
    bool isLeftSideFree(GameObject overtakingCar);
    bool isRightSideFree(GameObject overtakingCar);
    float getBaseGateDiffrence();
}
