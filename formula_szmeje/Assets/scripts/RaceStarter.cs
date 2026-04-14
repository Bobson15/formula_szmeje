using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceStarter : MonoBehaviour
{
    private GameObject playerNameDeliverObj;
    private Movement playerMovement;
    private int lights = 0;
    public GameObject startingLights;
    void Start()
    {
        playerNameDeliverObj = GameObject.FindWithTag("PlayerNameDeliver");
        playerMovement = GameObject.FindWithTag("Player").GetComponent<Movement>();        
        if (playerNameDeliverObj != null && playerNameDeliverObj.GetComponent<PlayerNameDeliver>().gamemode == Gamemode.Race)
        {
            StartCoroutine(StartRace());
        }
        else
        {
            foreach(GameObject ai in GameObject.FindGameObjectsWithTag("AI"))
            {
                Destroy(ai);
            }
            Destroy(startingLights);
            playerMovement.start();
        }
    }

    IEnumerator StartRace()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1f);
            startingLights.transform.GetChild(i).GetComponent<Image>().color = Color.red;
        }
        yield return new WaitForSeconds(Random.Range(0.2f, 2f));
        for (int i = 0; i < 5; i++)
        {
            startingLights.transform.GetChild(i).GetComponent<Image>().color = Color.gray;
        }
        playerMovement.start();
        GameObject.FindWithTag("Player").GetComponent<LapTimer>().TriggerLapStart();
        foreach (GameObject ai in GameObject.FindGameObjectsWithTag("AI"))
        {
            ai.GetComponent<Movement>().start();
        }
        yield return new WaitForSeconds(1f);
        Destroy(startingLights);
    }
}
