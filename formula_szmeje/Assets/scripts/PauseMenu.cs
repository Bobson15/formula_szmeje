using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused = false;
    private bool raceEnded = false;
    private PlayerInput playerInput;
    public GameObject pauseMenuUi;
    public GameObject raceEndUi;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        pauseMenuUi.SetActive(false);
        raceEndUi.SetActive(false);
    }

    void Update()
    {
        if (playerInput.actions["Pause"].triggered)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }    
    }
    private void Pause()
    {
        if (!raceEnded)
        {
            pauseMenuUi.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
    }
    public void Resume()
    {
        pauseMenuUi.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        Destroy(GameObject.FindWithTag("PlayerNameDeliver"));
        SceneManager.LoadScene(0);
    }
    public void EndRace(int position)
    {
        raceEnded = true;
        pauseMenuUi.SetActive(false);
        raceEndUi.SetActive(true);
        raceEndUi.transform.GetChild(0).GetComponent<TMP_Text>().text = "Ukoñczy³eœ wyœcig na " + position + " pozycji";
        Time.timeScale = 0f;
        isPaused=true;

    }
}
