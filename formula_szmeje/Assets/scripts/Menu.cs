using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public PlayerNameDeliver playerNameDeliver;
    public TMP_InputField playerNameInput;
    public RectTransform TimesTable;
    public List<TMP_Text> namesTMP;
    public List<TMP_Text> lapTimesTMP;
    public List<TMP_Text> firstsSctorTimesTMP;
    public List<TMP_Text> secondSectorTimesTMP;
    public List<TMP_Text> thirdSectorTimesTMP;
    public TMP_Text playerNameTMP;
    public TMP_Text playerBestLapTMP;
    public TMP_Text playerBestFirstSectorTMP;
    public TMP_Text playerBestSecondSectorTMP;
    public TMP_Text playerBestThirdSectorTMP;
    private bool isTableShown = false;
    private bool isTableMoving = false;
    private string playerName = "";
    private string lapTimesFilePath;
    private PlayerNameDeliverSerializerList lapTimes = new PlayerNameDeliverSerializerList();
    private void Start()
    {
        lapTimesFilePath = Application.persistentDataPath + "/lapTimes.json";
        if (File.Exists(lapTimesFilePath))
        {
            string json = File.ReadAllText(lapTimesFilePath);
            lapTimes = JsonUtility.FromJson<PlayerNameDeliverSerializerList>(json);
        }
        for(int i = 0; i < lapTimes.playersList.Count - 1; i++)
        {
            for(int j = i+1; j < lapTimes.playersList.Count; j++)
            {
                if (lapTimes.playersList[i].bestLapTime < lapTimes.playersList[j].bestLapTime)
                {
                    PlayerNameDeliverSerializer temp = lapTimes.playersList[i];
                    lapTimes.playersList[i] = lapTimes.playersList[j];
                    lapTimes.playersList[j] = temp;
                }
            }
        }
        for (int i = 0; i < Mathf.Min(lapTimes.playersList.Count,10); i++)
        {
            namesTMP[i].text = lapTimes.playersList[i].playerName;
            lapTimesTMP[i].text = FormatTime(lapTimes.playersList[i].bestLapTime);
            firstsSctorTimesTMP[i].text = FormatTime(lapTimes.playersList[i].bestFirstSectorTime);
            secondSectorTimesTMP[i].text = FormatTime(lapTimes.playersList[i].bestSecondSectorTime);
            thirdSectorTimesTMP[i].text = FormatTime(lapTimes.playersList[i].bestThirdSectorTime);
        }
    }
    private void Update()
    {
        if (isTableMoving) {
            if (!isTableShown)
            {
                TimesTable.position = new Vector3(Mathf.Min(TimesTable.position.x + (2000 * Time.deltaTime), 2510), TimesTable.position.y, TimesTable.position.z);
                if (TimesTable.position.x == 2510)
                {
                    isTableMoving = false;
                }
            }
            else
            {
                TimesTable.position = new Vector3(Mathf.Max(TimesTable.position.x - (2000 * Time.deltaTime), 1000), TimesTable.position.y, TimesTable.position.z);
                if (TimesTable.position.x == 1000)
                {
                    isTableMoving = false;
                }
            }
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void Hotlap()
    {
        playerNameDeliver.playerName = playerNameInput.text.ToUpper();
        playerNameDeliver.gamemode = Gamemode.HotLap;
        SceneManager.LoadScene(1);
    }
    public void ShowTimes()
    {
        if (!isTableMoving)
        {
            if (isTableShown)
            {
                isTableShown = false;
                isTableMoving = true;
            }
            else
            {
                isTableShown = true;
                isTableMoving = true;
            }
        }
    }
    public void Race()
    {
        playerNameDeliver.playerName = playerNameInput.text.ToUpper();
        playerNameDeliver.gamemode = Gamemode.Race;
        SceneManager.LoadScene(1);
    }
    public void UpdatePlayerTime()
    {
        playerName = playerNameInput.text.ToUpper();
        if (playerName != "")
        {
            for (int i = 0; i < lapTimes.playersList.Count; i++)
            {
                if (lapTimes.playersList[i].playerName == playerName)
                {
                    
                    playerBestLapTMP.text = FormatTime(lapTimes.playersList[i].bestLapTime);
                    playerBestFirstSectorTMP.text = FormatTime(lapTimes.playersList[i].bestFirstSectorTime);
                    playerBestSecondSectorTMP.text = FormatTime(lapTimes.playersList[i].bestSecondSectorTime);
                    playerBestThirdSectorTMP.text = FormatTime(lapTimes.playersList[i].bestThirdSectorTime);
                    return;
                }
            }
        }
        playerBestLapTMP.text = "-";
        playerBestFirstSectorTMP.text = "-";
        playerBestSecondSectorTMP.text = "-";
        playerBestThirdSectorTMP.text = "-";
    }
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int millis = Mathf.FloorToInt((time * 1000f) % 1000f);
        return string.Format("{0:0}:{1:00}.{2:000}", minutes, seconds, millis);
    }
}
