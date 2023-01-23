using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public TMP_Text rounsText;
    public SceneFader sceneFader;
    public int mainMenuIndex = 0;
    public GameObject ui;
    private void OnEnable()
    {
       // rounsText.text = PlayerStats.Rounds.ToString();
    }

    public void Toggle()
    {
        ui.SetActive(!ui.activeSelf);

        if (ui.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
    public void Retry()
    {
        Toggle();
        sceneFader.FadeTo((SceneManager.GetActiveScene().buildIndex));
    }

    public void GoToMenu()
    {
        Toggle();
        sceneFader.FadeTo(mainMenuIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
