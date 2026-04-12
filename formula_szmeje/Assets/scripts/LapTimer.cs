using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LapTimer : MonoBehaviour, ILapCounter
{
    [Header("Teksty UI")]
    public TMP_Text lapTimeText;
    public TMP_Text bestTimeText;
    public TMP_Text prevTimeText;
    public TMP_Text lapsCounterText;
    public TMP_Text possitionText;
    public TMP_Text invalidLapText;

    [Header("Panele Sektorów")]
    public GameObject sektor1Box;
    public GameObject sektor2Box;
    public GameObject sektor3Box;

    private Image sektor1Image;
    private Image sektor2Image;
    private Image sektor3Image;

    [Header("Kolory")]
    public Color kolorDomyslny = Color.gray;
    public Color kolorFiolet = new Color(0.5f, 0f, 0.5f);
    public Color kolorZielony = Color.green;
    public Color kolorZolty = Color.yellow;

    public PauseMenu pauseMenu;

    private float lapStartTime;
    private float sectorStartTime;

    private float sector1Time = 0f, sector2Time = 0f, sector3Time = 0f;
    private float bestSector1 = Mathf.Infinity, bestSector2 = Mathf.Infinity, bestSector3 = Mathf.Infinity;

    private float currentLapTime;
    private float lastLapTime;
    private float bestLapTime = Mathf.Infinity;

    private int currentSector = 0;
    private int lapCount = 1;
    private int position = 1;

    private bool lapStarted = false, invalidLap = false;

    private string playerName = "";
    private string lapTimesFilePath;
    private PlayerNameDeliverSerializerList lapTimes = new PlayerNameDeliverSerializerList();
    private GameObject playerNameDeliverObj;
    private bool blockTimer = false;

    private void Start()
    {
        sektor1Image = sektor1Box.GetComponent<Image>();
        sektor2Image = sektor2Box.GetComponent<Image>();
        sektor3Image = sektor3Box.GetComponent<Image>();
        lapTimeText.text = "Lap: 00:00.000";
        sektor1Image.color = kolorDomyslny;
        sektor2Image.color = kolorDomyslny;
        sektor3Image.color = kolorDomyslny;

        lapTimesFilePath = Application.persistentDataPath + "/lapTimes.json";
        if (File.Exists(lapTimesFilePath))
        {
            string json = File.ReadAllText(lapTimesFilePath);
            lapTimes = JsonUtility.FromJson<PlayerNameDeliverSerializerList>(json);
        }
        playerNameDeliverObj = GameObject.FindWithTag("PlayerNameDeliver");
        if (playerNameDeliverObj != null) {
            if(playerNameDeliverObj.GetComponent<PlayerNameDeliver>().gamemode == Gamemode.HotLap) {
                playerName = playerNameDeliverObj.GetComponent<PlayerNameDeliver>().playerName;
                for (int i = 0; i < lapTimes.playersList.Count; i++) {
                    if (lapTimes.playersList[i].playerName == playerName) {
                        bestLapTime = lapTimes.playersList[i].bestLapTime;
                        bestSector1 = lapTimes.playersList[i].bestFirstSectorTime;
                        bestSector2 = lapTimes.playersList[i].bestSecondSectorTime;
                        bestSector3 = lapTimes.playersList[i].bestThirdSectorTime;
                        bestTimeText.text = "Best: " + FormatTime(bestLapTime);
                        break;
                    }
                }
                possitionText.text = "";
            }
            else if(playerNameDeliverObj.GetComponent<PlayerNameDeliver>().gamemode == Gamemode.Race)
            {
                blockTimer = true;
                lapsCounterText.text = "Laps: 1/5";
                foreach (GameObject ai in GameObject.FindGameObjectsWithTag("AI"))
                {
                    position++;
                }
                possitionText.text = "Position: " + position;
            }
        }
        else
        {
            possitionText.text = "";
        }
    }

    private void Update()
    {
        if (lapStarted)
        {
            currentLapTime = Time.time - lapStartTime;
            lapTimeText.text = "Lap: " + FormatTime(currentLapTime);
        }
    }

    public void TriggerLapStart()
    {
        blockTimer = false;
        float now = Time.time;

        if (!lapStarted && currentSector == 0)
        {
            lapStarted = true;
            lapStartTime = now;
            sectorStartTime = now;
            currentSector = 1;

            sektor1Image.color = kolorDomyslny;
            sektor2Image.color = kolorDomyslny;
            sektor3Image.color = kolorDomyslny;

            lapTimeText.text = "Lap: 00:00.000";
            return;
        }

        if (lapStarted && currentSector == 3)
        {
            if (++lapCount >= 5 && playerNameDeliverObj != null && playerNameDeliverObj.GetComponent<PlayerNameDeliver>().gamemode == Gamemode.Race)
            {
                Destroy(GameObject.FindWithTag("PlayerNameDeliver"));
                pauseMenu.EndRace(position);
            }
            else
            {

                sector3Time = now - sectorStartTime;
                UstawKolor(sektor3Image, sector3Time, ref bestSector3);

                float finalLapTime = now - lapStartTime;
                lastLapTime = finalLapTime;

                lapTimeText.text = "Lap: " + FormatTime(finalLapTime);
                prevTimeText.text = "Previous: " + FormatTime(finalLapTime);
                if (!invalidLap)
                {
                    bestSector1 = Mathf.Min(bestSector1, sector1Time);
                    bestSector2 = Mathf.Min(bestSector2, sector2Time);
                    bestSector3 = Mathf.Min(bestSector3, sector3Time);
                    if (finalLapTime < bestLapTime)
                    {
                        bestLapTime = finalLapTime;
                        bestTimeText.text = "Best: " + FormatTime(bestLapTime);
                        if (playerName != "" && playerNameDeliverObj.GetComponent<PlayerNameDeliver>().gamemode == Gamemode.HotLap)
                        {
                            bool playerFound = false;
                            for (int i = 0; i < lapTimes.playersList.Count; i++)
                            {
                                if (lapTimes.playersList[i].playerName == playerName)
                                {
                                    lapTimes.playersList[i].bestLapTime = bestLapTime;
                                    lapTimes.playersList[i].bestFirstSectorTime = bestSector1;
                                    lapTimes.playersList[i].bestSecondSectorTime = bestSector2;
                                    lapTimes.playersList[i].bestThirdSectorTime = bestSector3;
                                    playerFound = true;
                                    break;
                                }
                            }
                            if (!playerFound)
                            {
                                lapTimes.playersList.Add(new PlayerNameDeliverSerializer(playerName, bestLapTime, bestSector1, bestSector2, bestSector3));
                            }
                            string json = JsonUtility.ToJson(lapTimes, true);
                            File.WriteAllText(lapTimesFilePath, json);
                        }
                    }
                }
                lapStartTime = now;
                sectorStartTime = now;
                currentSector = 1;
                sektor1Image.color = kolorDomyslny;
                sektor2Image.color = kolorDomyslny;
                sektor3Image.color = kolorDomyslny;
                lapTimeText.text = "Lap: 00:00.000";
                invalidLap = false;
                if (playerNameDeliverObj != null && playerNameDeliverObj.GetComponent<PlayerNameDeliver>().gamemode == Gamemode.Race)
                {
                    lapsCounterText.text = "Laps: " + (lapCount + 1) + "/5";
                }
                else
                {
                    lapsCounterText.text = "Laps: " + lapCount;
                }
            }
        }
    }

    public void TriggerSector(int sectorNumber)
    {
        float now = Time.time;
        if (!lapStarted) return;

        if (currentSector == 1 && sectorNumber == 2)
        {
            sector1Time = now - sectorStartTime;
            UstawKolor(sektor1Image, sector1Time, ref bestSector1);
            currentSector = 2;
            sectorStartTime = now;
        }
        else if (currentSector == 2 && sectorNumber == 3)
        {
            sector2Time = now - sectorStartTime;
            UstawKolor(sektor2Image, sector2Time, ref bestSector2);
            currentSector = 3;
            sectorStartTime = now;
        }
    }

    private void UstawKolor(Image img, float czas, ref float najlepszy)
    {
        if (czas < najlepszy)
        {
            img.color = kolorFiolet;
        }
        else if (czas < najlepszy + 0.5f)
        {
            img.color = kolorZielony;
        }
        else
        {
            img.color = kolorZolty;
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int millis = Mathf.FloorToInt((time * 1000f) % 1000f);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, millis);
    }
    public bool isTimerBlocked()
    {
        return blockTimer;
    }
    public int getLaps()
    {
        return lapCount;
    }
    public void UpdatePosition(int position)
    {
        this.position = position;
        possitionText.text = "Position: " + this.position;
    }
    public void SetInvalidLap()
    {
        if((playerNameDeliverObj == null || playerNameDeliverObj.GetComponent<PlayerNameDeliver>().gamemode == Gamemode.HotLap) && !invalidLap)
        {
            invalidLap = true;
            StartCoroutine(ShowLapInvalid());
        }
    }
    IEnumerator ShowLapInvalid()
    {
        invalidLapText.text = "okrążenie unieważnione - wyjechano poza tor";
        yield return new WaitForSeconds(2f);
        invalidLapText.text = "";
    }
}