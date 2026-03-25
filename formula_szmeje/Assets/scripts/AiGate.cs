using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiGate : MonoBehaviour
{
    public AiGate nextGate;
    public float maxSpeed;
    public Side side;
    public float getRotation()
    {
        return transform.eulerAngles.y;
    }
    public Vector3 getPossition()
    {
        return transform.position;
    }

    public enum Side
    {
        none,
        left,
        right
    }
}
