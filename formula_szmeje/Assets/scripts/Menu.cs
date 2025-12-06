using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public PlayerNameDeliver playerNameDeliver;
    public TMP_InputField playerNameInput;
    public void QuitGame()
    {
        Application.Quit();
    }
    public void Hotlap()
    {
        playerNameDeliver.playerName = playerNameInput.text.ToUpper();
        SceneManager.LoadScene(1);
    }
}
