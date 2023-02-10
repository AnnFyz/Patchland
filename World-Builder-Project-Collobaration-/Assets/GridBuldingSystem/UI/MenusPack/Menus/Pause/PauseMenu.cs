using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject ui;
    public SceneFader sceneFader;
    public int mainMenuIndex = 0;
   // [SerializeField] AudioSource backgroundSound;
    public GameObject controlsPanel;
    float gameVolume;
    //private void Start()
    //{
    //    gameVolume = backgroundSound.volume;
    //}
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !controlsPanel.activeSelf)
        {
            Toggle();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && controlsPanel.activeSelf)
        {
            controlsPanel.SetActive(false);
           // backgroundSound.volume = gameVolume;
            Time.timeScale = 1f;

        }

    }

    public void CloseControlsUI()
    {
        controlsPanel.SetActive(false);
       // backgroundSound.volume = gameVolume;
        Time.timeScale = 1f;
    }

    public void Toggle()
    {
        ui.SetActive(!ui.activeSelf);

        if (ui.activeSelf)
        {
           // backgroundSound.volume = 0.025f;
            Time.timeScale = 0f;
        }
        else
        {
           // backgroundSound.volume = gameVolume;
            Time.timeScale = 1f;
        }
    }

    public void Retry()
    {
        Toggle();
        sceneFader.FadeTo((SceneManager.GetActiveScene().buildIndex));
    }

    public void Menu()
    {
        Toggle();
        sceneFader.FadeTo(mainMenuIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ControlsPanel()
    {
        Toggle();
        Time.timeScale = 0f;
       // backgroundSound.volume = 0.025f;
        controlsPanel.SetActive(true);
    }
}
