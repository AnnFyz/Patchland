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


public class BlockPrefab : MonoBehaviour
{
    public int[,] blockId = new int[0, 0];
    public static Vector3 offset = new Vector3(5f, -5f, 5f); // to habe a local grid in the center -> offeset = cellSize in MyGridBuildingSystem
    public CornerBlock cornerBlock;
    public event Action<int> OnBlockHeightChanged;
    public bool IsThisBlockIsHighlighted = false;
    public bool IsThisBlockIsSelected = false;
    public Material defaultMaterial;
    public int blocksAmount = 0;
    public int maxAmount = 10;
    public int minAmount = 1;
    [SerializeField] Transform mainBlock;
    [SerializeField] Transform[] blockStack;

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
        if (addedAmount > 0 && (blocksAmount + addedAmount) <= maxAmount)
        {
            SetFirstBlockPos(true);
            ToggleNextBlock(true);
            blocksAmount += addedAmount;
            UIManager.Instance.LocalSetupUIIcons();
            OnBlockHeightChanged?.Invoke(blocksAmount);
        }
        else if (addedAmount < 0 && (blocksAmount + addedAmount) >= minAmount)
        {
            SetFirstBlockPos(false);
            ToggleNextBlock(false);
            blocksAmount += addedAmount;
            UIManager.Instance.LocalSetupUIIcons();
            OnBlockHeightChanged?.Invoke(blocksAmount);
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
