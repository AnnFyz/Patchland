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
    public Vector2 blockId = Vector2.zero; // array of rows and columns to store the block ID
    public static readonly Vector3 Offset = new Vector3(5f, -5f, 5f); // to habe a local grid in the center -> offeset = cellSize in MyGridBuildingSystem
    public CornerBlock cornerBlock;


    [Header("🖱️ Interaction")]

    public bool isHighlighted = false;
    public bool isSelected = false;
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
    public static BlockPrefab Create(Vector3 worldPosition, GameObject prefab, Quaternion rotation)
    {
        GameObject obj = Instantiate(prefab, worldPosition + Offset, rotation);
        BlockPrefab placedBlockPrefab = obj.GetComponent<BlockPrefab>();
        return placedBlockPrefab;
    }


    public void DeactivateStackOfBlocks()
    {
        foreach (var block in blockStack)
        {
            block.gameObject.SetActive(false);
        }
    }
    public void DestroySelf() => Destroy(gameObject);

    public void ChangeBlockHeight(int delta)
    {
        int newAmount = Mathf.Clamp(currentBlocksAmount + delta, minAmount, maxAmount);
        if (newAmount == currentBlocksAmount) return; // No valid change
        bool isAdded = delta > 0;
        SetFirstBlockPos(isAdded);
        ToggleNextBlock(isAdded);

        currentBlocksAmount = newAmount;

        UIManager.Instance.LocalSetupUIIcons();
        OnBlockHeightChanged?.Invoke(currentBlocksAmount);
    }

    void SetFirstBlockPos(bool isAdded)
    {
        float yDelta = Offset.y * (isAdded ? -1 : 1);
        transform.position = new Vector3(transform.position.x, transform.position.y + yDelta, transform.position.z);
    }

    void ToggleNextBlock(bool isAdded)
    {
        if (isAdded)
        {
            foreach (var block in blockStack)
            {
                if (block != null && !block.gameObject.activeSelf)
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
