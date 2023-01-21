using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlockPrefab : MonoBehaviour
{
    public static Vector3 offset = new Vector3(7.5f, -5f, 7.5f);
    private int newHeight;
    private int startScale;
    public event Action<int> OnHeightChanged;
    public bool IsThisBlockWasHighlighted = false;
    public bool IsThisBlockWasSelected = false;
    Renderer renderer;
    public Color defaultColor = new Color();
    public Material defaultMaterial;
    private void Start()
    {
        startScale = Mathf.FloorToInt(this.gameObject.transform.GetChild(0).localScale.y);
        renderer = GetComponentInChildren<Renderer>();
        if (renderer.material.HasColor("_BaseColor"))
        {
            defaultColor = renderer.material.color;
        }
        else if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
        {
            defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
        }
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

    //public void ChangeHeight(int addedHeight) 
    //{
    //    if (addedHeight < 0 && transform.localScale.y == 1)
    //    {
    //        float zRotation = BuildingManager.blockPrefab.transform.localRotation.z;
    //        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, -180);
    //        transform.localScale += new Vector3(0, -addedHeight, 0);
    //        newHeight = Mathf.RoundToInt(transform.localScale.y);
    //        UIManager.Instance.LocalSetupUIIcons();
    //        OnHeightChanged?.Invoke(newHeight );
    //    }

    //    if (addedHeight > 0 && transform.localScale.y == 1 && transform.localRotation.z == -1)
    //    {
    //        float zRotation = BuildingManager.blockPrefab.transform.localRotation.z;
    //        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, 0);
    //        transform.localScale += new Vector3(0, 0, 0);
    //        newHeight = Mathf.RoundToInt(transform.localScale.y);
    //        UIManager.Instance.LocalSetupUIIcons();
    //        OnHeightChanged?.Invoke(newHeight);
    //    }

    //    else
    //    {
    //        transform.localScale += new Vector3(0, addedHeight, 0);
    //        newHeight = Mathf.RoundToInt(transform.localScale.y);
    //        UIManager.Instance.LocalSetupUIIcons();
    //        OnHeightChanged?.Invoke(newHeight);

    //    }
    //}

    public void ChangeHeight(int addedHeight)
    {
        if (addedHeight < 0 && transform.localScale.y <= 1 && transform.localRotation.z >= 0) // flip from desert to pond 
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, -180);
            newHeight = Mathf.RoundToInt(transform.localScale.y);
            UIManager.Instance.LocalSetupUIIcons();
            OnHeightChanged?.Invoke(newHeight);
            Debug.Log("flip from desert to pond  " + transform.localRotation.z);
            return;
        }

        if (addedHeight > 0 && transform.localScale.y == 1 && transform.localRotation.z <= -0.1) //flip from pond to desert 
        {
            transform.localRotation = Quaternion.identity;
            newHeight = Mathf.RoundToInt(transform.localScale.y);
            UIManager.Instance.LocalSetupUIIcons();
            OnHeightChanged?.Invoke(newHeight);
            Debug.Log("flip from pond to desert  " + transform.localRotation.z);
            return;
        }

        if (addedHeight > 0 && transform.localScale.y >= 1 && transform.localRotation.z >= 0)  // from desert  to forest
        {
            transform.localRotation = Quaternion.identity;
            transform.localScale += new Vector3(0, addedHeight, 0);
            newHeight = Mathf.RoundToInt(transform.localScale.y);
            UIManager.Instance.LocalSetupUIIcons();
            OnHeightChanged?.Invoke(newHeight);
            Debug.Log("from desert  to forest " + transform.localRotation.z);
            return;

        }

        if (addedHeight < 0 && transform.localScale.y > 1)
        {
            transform.localRotation = Quaternion.identity;
            transform.localScale += new Vector3(0, addedHeight, 0);
            newHeight = Mathf.RoundToInt(transform.localScale.y);
            UIManager.Instance.LocalSetupUIIcons();
            OnHeightChanged?.Invoke(newHeight);
            Debug.Log("from forest or desert to desert or to pond  " + transform.localRotation.z);
            return;
        }

        if (addedHeight == 0) //setup at the start
        {
            transform.localScale += new Vector3(0, addedHeight, 0);
            newHeight = Mathf.RoundToInt(transform.localScale.y);
            UIManager.Instance.LocalSetupUIIcons();
            OnHeightChanged?.Invoke(newHeight);
            return;

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

    public void ChangeHighlightedColorl()
    {
        if (renderer.material.HasColor("_BaseColor"))
        {
            renderer.material.color = GridOfPrefabs.Instance.GetColorOfHighlightedBlocks();
        }
        else if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
        {
            renderer.material.SetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"), GridOfPrefabs.Instance.GetColorOfHighlightedBlocks());
        }

    }

    public void ChangeColorBack()
    {
        if (renderer.material.HasColor("_BaseColor"))
        {
            renderer.material.color = defaultColor;
        }
        else if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
        {
            renderer.material.SetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"), defaultColor);
        }
    }

    public void ChangeMaterialBack()
    {
        renderer.material = defaultMaterial;
    }
    public void ChangeSelectedMaterial()
    {
        renderer.material = GridOfPrefabs.Instance.GetMaterialOfSelectedBlocks();
    }
}
