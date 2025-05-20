using System;
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
                localPrefabBlock.IsThisBlockIsHighlighted = true;
                SetOutline(true);
                if (localPrefabBlock.IsThisBlockIsSelected && localPrefabBlock.IsThisBlockIsHighlighted)
                {
                    //localPrefabBlock.ChangeSelectedMaterial();
                    if (BuildingManager.Instance.placedObjectTypeSO == null)
                    {
                        BuildingManager.Instance.placedObjectTypeSO = BuildingManager.Instance.lastSelectedObjToPlaceTypeSO;
                    }

                        BuildingManager.localGrid = localGrid.grid;
                        BuildingManager.Instance.lastBlockPrefab = BuildingManager.Instance.currentBlockPrefab;
                        BuildingManager.Instance.currentBlockPrefab = localPrefabBlock;     
                       if (BuildingManager.Instance.lastBlockPrefab != null && BuildingManager.Instance.currentBlockPrefab.GetComponent<LocalLevelState>().GetCurrentLevelState() == BuildingManager.Instance.lastBlockPrefab.GetComponent<LocalLevelState>().GetCurrentLevelState())
                        {
                        BuildingManager.Instance.RefreshSelectedObjectType();
                    }
                    else
                    {
                        BuildingManager.Instance.DeselectObjectTypeOnSelectedAnotherBlockType();
                    }
                      


                }
                else
                {
                    BuildingManager.Instance.DeselectObjectType();
                }
            }
            else
            {
               
                localPrefabBlock.IsThisBlockIsHighlighted = false;
                SetOutline(false);
                //BuildingManager.Instance.DeselectObjectType();

                if (!localPrefabBlock.IsThisBlockIsSelected)
                {
           
                    //localPrefabBlock.ChangeColorBack();
                   // localPrefabBlock.ChangeMaterialBack();
                    if(localPrefabBlock.GetComponent<LocalLevelState>().GetCurrentLevelState() == LevelState.Forest)
                    {
                    }
                }
                else
                {
                  //  localPrefabBlock.ChangeSelectedMaterial();
                }
                
            }
        }

        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                localPrefabBlock.IsThisBlockIsHighlighted = false;
                //SetOutline(false);
                localPrefabBlock.IsThisBlockIsSelected = false;
               // localPrefabBlock.ChangeColorBack();
               // localPrefabBlock.ChangeMaterialBack();
                BuildingManager.Instance.DeselectObjectType();
                UIManager.Instance.LocalSetupUIIcons();
            }
           

            //if (!localPrefabBlock.IsThisBlockIsSelected)
            //{
            //    localPrefabBlock.ChangeColorBack();
            //    localPrefabBlock.ChangeMaterialBack();
            //    BuildingManager.Instance.DeselectObjectType();
            //}
            //else
            //{
            //    localPrefabBlock.ChangeSelectedMaterial();
            //}
        } 
    }

    public void SetOutline(bool isSelected)
    {
        if (isSelected)
        {
            GetComponent<Outline>().enabled = true;
            GetComponent<Outline>().OutlineColor = Color.white;
            GetComponent<Outline>().OutlineWidth = 3.5f;
        }
        else
        {
            if(!localPrefabBlock.IsThisBlockIsSelected)
            GetComponent<Outline>().enabled = false;
        }
    }
}

