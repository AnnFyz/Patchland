using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum CornerBlock
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    None
}


[System.Serializable]
public class BlockInGrid
{
    public int[] row;
}

public class BlockPrefab : MonoBehaviour
{
    [Header("📍 Grid Position")]

    [Tooltip("Grid coordinates of this block (row, column)")]
    public Vector2 blockId = new Vector2(0,0); // array of rows and columns to store the block ID
    public static Vector3 offset = new Vector3(5f, -5f, 5f); // to habe a local grid in the center -> offeset = cellSize in MyGridBuildingSystem
    public CornerBlock cornerBlock;


    [Header("🖱️ Interaction")]

    public bool IsThisBlockIsHighlighted = false;
    public bool IsThisBlockIsSelected = false;
    public Material defaultMaterial;


    [Header("📦 Block State")]

    public int currentBlocksAmount = 0;
    public int maxAmount = 10;
    public int minAmount = 1;
    public event Action<int> OnBlockHeightChanged;


    [Header("🏗️ Block Prefabs")]

    [Tooltip("The main block prefab")]
    [SerializeField] Transform mainBlock;

    [Tooltip("Additional stacked block prefabs")]
    [SerializeField] Transform[] blockStack;


    // Factory method to create and initialize a BlockPrefab instance
    public static BlockPrefab Create(Vector3 worldPosition, GameObject blockPrefab, Quaternion rotation)
    {
        GameObject placedBlockPrefabObj = Instantiate(blockPrefab, worldPosition + offset, rotation);
        BlockPrefab placedBlockPrefab = placedBlockPrefabObj.GetComponent<BlockPrefab>();
        return placedBlockPrefab;
    }

    public void DeactivateStackOfBlocks()
    {
        foreach (var block in blockStack)
        {
            block.gameObject.SetActive(false);
        }
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void ChangeBlockHeight(int addedAmount)
    {
        if (addedAmount > 0 && (currentBlocksAmount + addedAmount) <= maxAmount)
        {
            SetFirstBlockPos(true);
            ToggleNextBlock(true);
            currentBlocksAmount += addedAmount;
            UIManager.Instance.LocalSetupUIIcons();
            OnBlockHeightChanged?.Invoke(currentBlocksAmount);
        }
        else if (addedAmount < 0 && (currentBlocksAmount + addedAmount) >= minAmount)
        {
            SetFirstBlockPos(false);
            ToggleNextBlock(false);
            currentBlocksAmount += addedAmount;
            UIManager.Instance.LocalSetupUIIcons();
            OnBlockHeightChanged?.Invoke(currentBlocksAmount);
        }
    }

    void SetFirstBlockPos(bool isAdded)
    {
        if (isAdded)
        {
            float yPos = transform.position.y - offset.y;
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        }
        else
        {
            float yPos = transform.position.y + offset.y;
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        }
    }

    void ToggleNextBlock(bool isAdded)
    {
        if (isAdded)
        {
            foreach (var block in blockStack)
            {
                if (!block.gameObject.activeSelf)
                {
                    block.gameObject.SetActive(true);
                    break;
                }
            }
        }
        else
        {
            // Reverse the array without assigning the result to a variable
            System.Array.Reverse(blockStack);

            foreach (var block in blockStack)
            {
                if (block.gameObject.activeSelf)
                {
                    block.gameObject.SetActive(false);
                    break;
                }
            }

            // Reverse the array back to its original order
            System.Array.Reverse(blockStack);
        }
    }

    // Change the material of all blocks
    public void SetStateMaterial(Material stateMaterial)
    {
        defaultMaterial = stateMaterial;
        foreach (var block in blockStack)
        {
            // Change the material of each block in the stack
            block.gameObject.GetComponent<Renderer>().material = stateMaterial;
        }
        // Change the material of the main block
        mainBlock.gameObject.GetComponent<Renderer>().material = stateMaterial;
    }


}
