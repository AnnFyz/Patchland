using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlockPrefab : MonoBehaviour
{
    public static Vector3 offset = new Vector3(7.5f, -5f, 7.5f);
    private int newHeight;
    private int startScale;
    public event Action <int> OnHeightChanged;
    public bool IsThisBlockWasHighlighted = false;
    public bool IsThisBlockWasSelected = false;
    Renderer renderer;
    public Color defaultColor = new Color();

    private void Start()
    {
        startScale = Mathf.FloorToInt(this.gameObject.transform.GetChild(0).localScale.y)/100;
        renderer = GetComponentInChildren<Renderer>();
        defaultColor = renderer.material.color;
    }
    public static BlockPrefab Create(Vector3 worldPosition, GameObject blockPrefab)
    {
        GameObject placedBlockPrefabObj = Instantiate(blockPrefab, worldPosition + offset, Quaternion.identity);
        BlockPrefab placedBlockPrefab = placedBlockPrefabObj.GetComponent<BlockPrefab>();
        return placedBlockPrefab;
    }


    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void ChangeHeight(int addedHeight) 
    {
        if (addedHeight < 0 && transform.localScale.y == 1)
        {
            float zRotation = BuildingManager.blockPrefab.transform.localRotation.z;
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, -180);
            transform.localScale += new Vector3(0, -addedHeight, 0);
            newHeight = Mathf.FloorToInt(transform.localScale.y);
            UIManager.Instance.LocalSetupUIIcons();
            OnHeightChanged?.Invoke(newHeight);
        }
        if (addedHeight > 0 && transform.localScale.y == 1)
        {
            float zRotation = BuildingManager.blockPrefab.transform.localRotation.z;
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, 0);
            transform.localScale += new Vector3(0, addedHeight, 0);
            newHeight = Mathf.FloorToInt(transform.localScale.y);
            UIManager.Instance.LocalSetupUIIcons();
            OnHeightChanged?.Invoke(newHeight);
        }
        else
        {
            transform.localScale += new Vector3(0, addedHeight, 0);
            newHeight = Mathf.FloorToInt(transform.localScale.y);
            UIManager.Instance.LocalSetupUIIcons();
            OnHeightChanged?.Invoke(newHeight);
        }
    }

    public int GetNewHeight()
    {
        return newHeight;
    }

    public int GetStartScale()
    {
        return startScale;
    }

    public void ChangeHighlightedColor()
    {
        renderer.material.color = GridOfPrefabs.Instance.GetColorOfHighlightedBlocks();
    }

    public void ChangeColorBack()
    {
        renderer.material.color = defaultColor;
    }

    public void ChangeSelectedColor()
    {
        renderer.material.color = GridOfPrefabs.Instance.GetColorOfSelectedBlocks();
    }
}
