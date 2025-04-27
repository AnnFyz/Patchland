using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public SceneFader sceneFader;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject nextLevelPanel;
    //public int amountOfCollectedGems;
    [SerializeField] int nextSceneIndex;
    public int amountOfAllBlocks;
    public int amountOfDeadBlocks;
    [SerializeField] int mainMenuIndex = 0;
    PauseMenu pauseMenu;
    private void Awake()
    {
        Instance = this;
        pauseMenu = GetComponent<PauseMenu>();
    }
    //private void OnEnable()
    //{
    //    UIManager.Instance.OnNextLevel += CheckIfEnoughSpecialGemsCollected;
    //}

    private void Start()
    {
        UIManager.Instance.OnNextLevel += CheckIfEnoughSpecialGemsCollected;
        gameOverPanel.SetActive(false);
        nextLevelPanel.SetActive(false);
        amountOfAllBlocks = GridOfPrefabs.Instance.height * GridOfPrefabs.Instance.width;
    }

    public void CheckIfAllBlocksAreDead()
    {
        if (amountOfDeadBlocks == amountOfAllBlocks)
        {
            pauseMenu.ui.gameObject.SetActive(false);
            pauseMenu.controlsPanel.gameObject.SetActive(false);
            Time.timeScale = 0f;
            gameOverPanel.SetActive(true);
        }
    }

    public void RetryAfterGameOver()
    {
        Time.timeScale = 1f;
        sceneFader.FadeTo((SceneManager.GetActiveScene().buildIndex));
    }

    public void MenuAfterGameOverOrCompletedLevel()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene(mainMenuIndex);
        sceneFader.FadeTo(mainMenuIndex);
    }


    public void CheckIfEnoughSpecialGemsCollected()
    {

        pauseMenu.ui.gameObject.SetActive(false);
        pauseMenu.controlsPanel.gameObject.SetActive(false);
        Time.timeScale = 0f;
        nextLevelPanel.SetActive(true);
    }

    public void NextLevel(int nextIndex)
    {
        //int nextIndex = SceneManager.GetActiveScene().buildIndex;
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextIndex);
        //sceneFader.FadeTo(nextIndex++);
    }
}
