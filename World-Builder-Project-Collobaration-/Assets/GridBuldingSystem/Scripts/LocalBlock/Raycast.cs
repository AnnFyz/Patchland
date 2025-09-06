using System;
using UnityEngine;
using UnityEngine.EventSystems;


// Handles local raycasting interactions for each BlockPrefab
public class Raycast : MonoBehaviour
{

    private BlockPrefab blockPrefab;    // The block this script is attached to
    private GridBuildingSystem gridSystem;  // Local grid reference
    private Outline outline;    // Cached outline component


    BlockPrefab block;
    private void Awake()
    {
        blockPrefab = GetComponent<BlockPrefab>();
        gridSystem = GetComponent<GridBuildingSystem>();
        outline = GetComponent<Outline>();
    }
    private void Update()
    {
        // Skip if pointer is over UI
        if (EventSystem.current.IsPointerOverGameObject()) return;


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            HandleRaycastHit(hit);
        }
        else
        {
            HandleRaycastMiss();
        }
    }

    // Process logic when raycast hits something
    private void HandleRaycastHit(RaycastHit hit)
    {
        BlockPrefab hitBlock = hit.collider.GetComponentInParent<BlockPrefab>();
        bool isThisBlock = (blockPrefab == hitBlock);

        blockPrefab.isHighlighted = isThisBlock;
        SetOutline(isThisBlock);

        if (!isThisBlock) return;

        if (blockPrefab.isSelected && blockPrefab.isHighlighted)
        {
            HandleBlockSelection();
        }
        else
        {
            BuildingManager.Instance.DeselectObjectType();
        }
    }

    // Process logic when raycast hits nothing
    private void HandleRaycastMiss()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        blockPrefab.isHighlighted = false;
        blockPrefab.isSelected = false;

        BuildingManager.Instance.DeselectObjectType();
        UIManager.Instance.LocalSetupUIIcons();
    }

    // Handles updating BuildingManager when a block is selected
    private void HandleBlockSelection()
    {
        if (BuildingManager.Instance.currentObjectTypeSO == null)
        {
            BuildingManager.Instance.currentObjectTypeSO = BuildingManager.Instance.lastSelectedObjToPlaceTypeSO;
        }

        BuildingManager.localGrid = gridSystem.Grid;
        BuildingManager.Instance.lastBlockPrefab = BuildingManager.Instance.currentBlockPrefab;
        BuildingManager.Instance.currentBlockPrefab = blockPrefab;

        // Refresh or reset selection depending on block type
        if (BuildingManager.Instance.lastBlockPrefab != null &&
            blockPrefab.GetComponent<LocalLevelState>().GetCurrentLevelState() ==
            BuildingManager.Instance.lastBlockPrefab.GetComponent<LocalLevelState>().GetCurrentLevelState())
        {
            BuildingManager.Instance.RefreshSelectedObjectType();
        }
        else
        {
            BuildingManager.Instance.DeselectObjectTypeOnSelectedAnotherBlockType();
        }
    }

    // Enable/disable outline visuals
    public void SetOutline(bool enable)
    {
        if (outline == null) return;
        if (enable)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.white;
            outline.OutlineWidth = 3.5f;
        }
        else if (!blockPrefab.isSelected)
        {
            outline.enabled = false;
        }
    }
}

