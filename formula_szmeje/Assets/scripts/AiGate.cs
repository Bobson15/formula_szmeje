using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AiGate : MonoBehaviour
{
    public AiGate nextGate;
    public float maxSpeed;
    public Side side;
    public CutState cutState = CutState.none;
    private Dictionary<GameObject, int> laps = new Dictionary<GameObject, int>();
    private GameObject playerNameDeliverObj;
    private PlayerNameDeliver playerNameDeliver;
    private void Start()
    {
        playerNameDeliverObj = GameObject.FindWithTag("PlayerNameDeliver");
        if (playerNameDeliverObj != null)
        {
            playerNameDeliver = playerNameDeliverObj.GetComponent<PlayerNameDeliver>();
        }
    }
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
    public enum CutState
    {
        none,
        half,
        full
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (playerNameDeliverObj != null && playerNameDeliver.gamemode == Gamemode.Race)
        {
            GameObject parent = collider.transform.parent.gameObject;
            if (parent.CompareTag("AI"))
            {
                laps[parent] = parent.GetComponent<ILapCounter>().getLaps();
            }
            if (parent.CompareTag("Player"))
            {
                laps[parent] = parent.GetComponent<ILapCounter>().getLaps();
                int position = 0;
                foreach (var lap in laps)
                {
                    if (lap.Value >= parent.GetComponent<ILapCounter>().getLaps())
                    {
                        position++;
                    }
                }
                parent.GetComponent<LapTimer>().UpdatePosition(position);
            }
        }
    }
}
