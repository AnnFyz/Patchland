using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyRaycast : MonoBehaviour // local raycasting for each block prefab
{

    BlockPrefab localPrefabBlock;
    MyGridBuildingSystem localGrid;
    Ray ray;
    RaycastHit hit;
    BlockPrefab block;
    private void Awake()
    {
        localPrefabBlock = GetComponent<BlockPrefab>();
        localGrid = GetComponent<MyGridBuildingSystem>();
    }
    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            //the collider could be children of the unit, so we make sure to check in the parent
            block = hit.collider.GetComponentInParent<BlockPrefab>();
            if (localPrefabBlock == block)
            {
                localPrefabBlock.IsThisBlockWasHighlighted = true;
                localPrefabBlock.ChangeHighlightedColorl();
                if (localPrefabBlock.IsThisBlockWasSelected)
                {
                    localPrefabBlock.ChangeSelectedMaterial();
                    if (BuildingManager.Instance.placedObjectTypeSO == null)
                    {
                        BuildingManager.Instance.placedObjectTypeSO = BuildingManager.Instance.lastSelectedObjToPlaceTypeSO;
                    }

                        BuildingManager.grid = localGrid.grid;
                        BuildingManager.blockPrefab = localPrefabBlock;
                        BuildingManager.Instance.RefreshSelectedObjectType();

                        localPrefabBlock.blockInside.gameObject.SetActive(false);

                }
                else
                {
                    BuildingManager.Instance.DeselectObjectType();
                }
            }
            else
            {
               
                localPrefabBlock.IsThisBlockWasHighlighted = false;
              
                if (!localPrefabBlock.IsThisBlockWasSelected)
                {
           
                    localPrefabBlock.ChangeColorBack();
                    localPrefabBlock.ChangeMaterialBack();
                    if(localPrefabBlock.GetComponent<LocalLevelState>().GetCurrentLevelState() == LevelState.Forest)
                    {
                        localPrefabBlock.blockInside.gameObject.SetActive(true);
                    }
                }
                else
                {
                    localPrefabBlock.ChangeSelectedMaterial();
                    localPrefabBlock.blockInside.gameObject.SetActive(false);
                }
                
            }
        }

        else
        {
            
            localPrefabBlock.IsThisBlockWasHighlighted = false;
            if (!localPrefabBlock.IsThisBlockWasSelected)
            {
                localPrefabBlock.ChangeColorBack();
                localPrefabBlock.ChangeMaterialBack();
                if (localPrefabBlock.GetComponent<LocalLevelState>().GetCurrentLevelState() == LevelState.Forest)
                {
                    localPrefabBlock.blockInside.gameObject.SetActive(true);
                }
                BuildingManager.Instance.DeselectObjectType();
            }
            else
            {
                localPrefabBlock.ChangeSelectedMaterial();
                localPrefabBlock.blockInside.gameObject.SetActive(false);
            }
        }
    }
}

