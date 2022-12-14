using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject arrowsPanel;
    [SerializeField] GameObject iconsPanel;
    [SerializeField] List<GameObject> icons;

    public LocalLevelState prefabsState;
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HidePanels();
        foreach (var icon in icons)
        {
            icon.SetActive(false);
        }

    }

    public void LocalSetupUIIcons()
    {
        //prefabsState = BuildingManager.blockPrefab.gameObject.GetComponent<LocalLevelState>();
        if (prefabsState != null)
        {
            switch (prefabsState.GetCurrentLevelState())
            {
                case LevelState.Pond:
                    foreach (var icon in icons)
                    {
                        icon.SetActive(false);
                    }
                    break;

                case LevelState.Desert:
                    icons[0].SetActive(true);
                    break;

                case LevelState.Forest:
                    foreach (var icon in icons)
                    {
                        icon.SetActive(true);
                    }
                    break;

                default:
                    Debug.Log("NOTHING");
                    break;
            }
        }       
    }
    public void ShowPanels()
    {
        arrowsPanel.SetActive(true);
        iconsPanel.SetActive(true);
    }

    public void HidePanels()
    {
        arrowsPanel.SetActive(false);
        foreach (var icon in icons)
        {
            icon.SetActive(false);
        }
        iconsPanel.SetActive(false);
    }

    public void BlockUp(int addedV)
    {
        BuildingManager.blockPrefab.ChangeHeight(addedV);
    }

    public void BlockDown(int subtractedV)
    {
        BuildingManager.blockPrefab.ChangeHeight(subtractedV);
    }
}
