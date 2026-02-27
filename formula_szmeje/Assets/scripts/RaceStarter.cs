using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceStarter : MonoBehaviour
{
    private GameObject playerNameDeliverObj;
    void Start()
    {
        playerNameDeliverObj = GameObject.FindWithTag("PlayerNameDeliver");
        //Zmienić potem z == null na != null oraz || na &&
        if (playerNameDeliverObj == null || playerNameDeliverObj.GetComponent<PlayerNameDeliver>().gamemode == Gamemode.Race)
        {
            //procedura startu wyścigu
        }
        else
        {
            foreach(GameObject ai in GameObject.FindGameObjectsWithTag("AI"))
            {
                Debug.Log(ai.name);
                Destroy(ai);
            }
        }
    }
}
