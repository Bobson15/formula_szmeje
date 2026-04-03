using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused = false;
    private PlayerInput playerInput;
    public GameObject pauseMenuUi;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        pauseMenuUi.SetActive(false);
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
        pauseMenuUi.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
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

}
