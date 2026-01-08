using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiGate : MonoBehaviour
{
    public AiGate nextGate;
    public float maxSpeed;
    private Transform transform;
    private void Start()
    {
        transform = GetComponent<Transform>();
    }
    public float getRotation()
    {
        return transform.eulerAngles.y;
    }

}
