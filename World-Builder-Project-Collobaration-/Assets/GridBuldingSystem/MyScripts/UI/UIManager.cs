using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject arrowsPanel;
    [SerializeField] GameObject iconsPanel;
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HidePanels();
    }
    public void ShowPanels()
    {
        arrowsPanel.SetActive(true);
        iconsPanel.SetActive(true);
    }

    public void HidePanels()
    {
        arrowsPanel.SetActive(false);
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
