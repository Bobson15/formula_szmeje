using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LapTimer : MonoBehaviour
{
    [Header("Teksty UI")]
    public TMP_Text lapTimeText;
    public TMP_Text bestTimeText;
    public TMP_Text prevTimeText;

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

    private float lapStartTime;
    private float sectorStartTime;

    private float sector1Time, sector2Time, sector3Time;
    private float bestSector1 = Mathf.Infinity;
    private float bestSector2 = Mathf.Infinity;
    private float bestSector3 = Mathf.Infinity;

    private float currentLapTime;
    private float lastLapTime;
    private float bestLapTime = Mathf.Infinity;

    private int currentSector = 0;
    private int lapCount = 0;

    private bool lapStarted = false;

    private void Start()
    {
        sektor1Image = sektor1Box.GetComponent<Image>();
        sektor2Image = sektor2Box.GetComponent<Image>();
        sektor3Image = sektor3Box.GetComponent<Image>();

        ResetUI();
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

            Debug.Log("Start nowego okrążenia");
            return;
        }

        if (lapStarted && currentSector == 3)
        {
            sector3Time = now - sectorStartTime;
            UstawKolor(sektor3Image, sector3Time, ref bestSector3);

            float finalLapTime = now - lapStartTime;
            lastLapTime = finalLapTime;

            prevTimeText.text = "Previous: " + FormatTime(finalLapTime);

            if (finalLapTime < bestLapTime)
            {
                bestLapTime = finalLapTime;
                bestTimeText.text = "Best: " + FormatTime(bestLapTime);
            }

            lapStarted = false; 

            currentSector = 0;
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
            najlepszy = czas;
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

    private void ResetUI()
    {
        lapTimeText.text = "Lap: 00:00.000";
        sektor1Image.color = kolorDomyslny;
        sektor2Image.color = kolorDomyslny;
        sektor3Image.color = kolorDomyslny;

        sector1Time = 0f;
        sector2Time = 0f;
        sector3Time = 0f;

        currentSector = 0;
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int millis = Mathf.FloorToInt((time * 1000f) % 1000f);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, millis);
    }
}