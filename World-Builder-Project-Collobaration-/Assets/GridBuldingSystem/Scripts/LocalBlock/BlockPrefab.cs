using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ChristinaCreatesGames.Animations;


public enum CornerBlock
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    None
}


/// <summary>
/// Represents a block in the grid system.
/// Tracks position, corner type, and block state.
/// Handles stacking (height increase/decrease).
/// Supports highlighting, selection, and material changes.
/// Raises events when block height changes.
/// </summary>
public class BlockPrefab : MonoBehaviour
{
    [Header("📍 Grid Position")]

    [Tooltip("Grid coordinates of this block (row, column)")]
    public Vector2 blockId = Vector2.zero; // array of rows and columns to store the block ID
    public static readonly Vector3 Offset = new Vector3(5f, -2.5f, 5f); // to habe a local grid in the center -> offeset = cellSize in MyGridBuildingSystem
    public CornerBlock cornerBlock;


    [Header("🖱️ Interaction")]

    public bool isHighlighted = false;
    public bool isSelected = false;
    public Material defaultMaterial;
    SquashAndStretch squashAndStretch;

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


    void Awake()
    {
        squashAndStretch = GetComponent<SquashAndStretch>();
    }

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


    // Change the height of the block stack at the start of the game
    public void InitializeBlockHeight(int delta)
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


    public void ChangeBlockHeight(int delta)
    {
        int newAmount = Mathf.Clamp(currentBlocksAmount + delta, minAmount, maxAmount);
        if (newAmount == currentBlocksAmount) return; // No valid change
        bool isAdded = delta > 0;
        SetFirstBlockPos(isAdded);
        ToggleNextBlock(isAdded);

        currentBlocksAmount = newAmount;

        squashAndStretch.PlaySquashAndStretch();
        UIManager.Instance.LocalSetupUIIcons();
        OnBlockHeightChanged?.Invoke(currentBlocksAmount);
    }

    // Adjust block base position when height changes
    void SetFirstBlockPos(bool isAdded)
    {
        float yDelta = Offset.y * (isAdded ? -1 : 1);
        transform.position = new Vector3(transform.position.x, transform.position.y + yDelta, transform.position.z);
    }


    // Enable/disable stacked blocks when height changes
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
            for (int i = blockStack.Length - 1; i >= 0; i--)
            {
                if (blockStack[i] != null && blockStack[i].gameObject.activeSelf)
                {
                    blockStack[i].gameObject.SetActive(false);
                    break;
                }
            }
        }
    }

    //  // Change the material of the main block and its stack
    public void SetStateMaterial(Material stateMaterial)
    {
        if (stateMaterial == null) return;
        defaultMaterial = stateMaterial;
        ApplyMaterial(mainBlock, stateMaterial);

        foreach (var block in blockStack)
        {
            ApplyMaterial(block, stateMaterial);
        }
    }

    // Apply material to a single block
    private void ApplyMaterial(Transform block, Material material)
    {
        if (block == null) return;

        var renderer = block.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }
    }
}
