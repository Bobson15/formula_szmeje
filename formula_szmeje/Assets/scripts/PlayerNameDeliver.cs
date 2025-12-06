using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNameDeliver : MonoBehaviour
{
    public static PlayerNameDeliver Instance;
    public string playerName;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

[System.Serializable]
public class PlayerNameDeliverSerializer
{
    public string playerName;
    public float bestLapTime;
    public float bestFirstSectorTime;
    public float bestSecondSectorTime;
    public float bestThirdSectorTime;

    public PlayerNameDeliverSerializer(string playerName, float bestLapTime, float bestFirstSectorTime, float bestSecondSectorTime, float bestThirdSectorTime)
    {
        this.playerName = playerName;
        this.bestLapTime = bestLapTime;
        this.bestFirstSectorTime = bestFirstSectorTime;
        this.bestSecondSectorTime = bestSecondSectorTime;
        this.bestThirdSectorTime = bestThirdSectorTime;
    }
}

[System.Serializable]
public class PlayerNameDeliverSerializerList
{
    public List<PlayerNameDeliverSerializer> playersList = new List<PlayerNameDeliverSerializer>();
}
