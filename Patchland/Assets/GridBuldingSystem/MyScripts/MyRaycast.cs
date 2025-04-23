using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRaycast : MonoBehaviour
{
    public bool IsThisObjWasSelected = false;
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
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            //the collider could be children of the unit, so we make sure to check in the parent
            block = hit.collider.GetComponentInParent<BlockPrefab>();
            if (localPrefabBlock == block)
            {
                localPrefabBlock.IsThisBlockWasSelected = true;
                if(BuildingManager.grid != localGrid.grid)
                {
                    BuildingManager.grid = localGrid.grid;
                    BuildingManager.blockPrefab = localPrefabBlock;
                    BuildingManager.Instance.RefreshSelectedObjectType();
                }
            }
            else
            {
                localPrefabBlock.IsThisBlockWasSelected = false;
            }
        }

        else
        {
            localPrefabBlock.IsThisBlockWasSelected = false;
        }
    }
}

