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
    public bool canCut = false;
    private Dictionary<GameObject, int> laps = new Dictionary<GameObject, int>();
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
    private void OnTriggerEnter(Collider collider)
    {
        GameObject playerNameDeliverObj = GameObject.FindWithTag("PlayerNameDeliver");
        if (playerNameDeliverObj != null && playerNameDeliverObj.GetComponent<PlayerNameDeliver>().gamemode == Gamemode.Race)
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
