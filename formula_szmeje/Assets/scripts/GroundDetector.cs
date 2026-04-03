using System;
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
    private bool canCut = false;

    void Start()
    {
        terrain = GameObject.Find("Monza").GetComponent<Terrain>(); ;
    }

    void FixedUpdate()
    {
        bool tempGroundDetected = true;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, trackLayer))
        {
            float terrainHeight = terrain.SampleHeight(hit.point);
            if (hit.point.y > terrainHeight)
            {
                tempGroundDetected = false;
            }
        }
        groundDetected = tempGroundDetected;
    }

    public bool isGroundDetected()
    {
        return groundDetected;
    }
    public Vector3 getPossition()
    {
        return transform.position;
    }
    public void info()
    {
        Debug.Log(gameObject.name + " " + groundDetected);
    }
    public void changePossition(bool canCut)
    {
        if (this.canCut && !canCut)
        {
            this.canCut = false;
            transform.localPosition = new Vector3((transform.localPosition.x / Mathf.Abs(transform.localPosition.x)) * 0.8f, transform.localPosition.y, transform.localPosition.z);
        }
        else if (!this.canCut && canCut)
        {
            this.canCut = true;
            transform.localPosition = new Vector3((transform.localPosition.x / Mathf.Abs(transform.localPosition.x)) * 0.4f, transform.localPosition.y, transform.localPosition.z);
        }
    }
}
