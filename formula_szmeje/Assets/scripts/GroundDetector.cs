using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    public LayerMask trackLayer;
    public float rayLength = 5f;
    private Terrain terrain;
    private bool groundDetected = false;

    void Start()
    {
        terrain = GameObject.Find("Monza").GetComponent<Terrain>(); ;
    }

    void FixedUpdate()
    {
        groundDetected = true;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, trackLayer))
        {
            float terrainHeight = terrain.SampleHeight(hit.point);
            if (hit.point.y > terrainHeight)
            {
                groundDetected = false;
            }
        }

        Debug.DrawRay(transform.position, Vector3.down * rayLength, groundDetected ? Color.green : Color.red);
    }

    public bool isGroundDetected()
    {
        return groundDetected;
    }
    public void info()

    {
        Debug.Log(gameObject.name + " " + groundDetected);
    }
}
