using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlockPrefab : MonoBehaviour
{
    public static Vector3 offset = new Vector3(5f, -5f, 5f);
    private int newAmount;
    private int startScale;
    public event Action<int> OnAmountChanged;
    public bool IsThisBlockIsHighlighted = false;
    public bool IsThisBlockIsSelected = false;
    public Renderer[] renderers;
    public Color defaultColor = new Color();
    public Color defaultBottomColor = new Color();
    public float origin;
    public Material defaultMaterial;
    float minOrigin;
    float maxOrigin;
    float duration = 2.0f;
    float startTime;
    float t;
    public int blocksAmount = 0;
    public int maxAmount = 10;
    public int minAmount = 1;
    [SerializeField] Transform[] blockStack;

    private void Start()
    {
        startScale = Mathf.FloorToInt(this.gameObject.transform.GetChild(0).localScale.y + 4.0f);
        renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            if (renderer.material.HasColor("_BaseColor"))
            {
                defaultColor = renderer.material.color;
            }
            if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
            {
                defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
                defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));
                origin = renderer.material.GetFloat(Shader.PropertyToID("Vector1_6e12275293314cb7a52c177f83f8f9aa"));
            }
        }
        minOrigin = 0f;
        maxOrigin =  0.95f;
        startTime = Time.deltaTime;
        t = UnityEngine.Random.Range(2f, 7f);
    }
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

    private void Update()
    {
        ChangeOriginOfGradient();
    }

    void ChangeOriginOfGradient()
    {
        float v = Mathf.PingPong(Time.time, t);
        origin = Mathf.SmoothStep(minOrigin, maxOrigin, v * 0.25f);
        foreach (var renderer in renderers)
        {
            renderer.material.SetFloat("Vector1_6e12275293314cb7a52c177f83f8f9aa", origin);
        }
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void ChangeAmount(int addedAmount)
    {
        if (addedAmount > 0 && (blocksAmount + addedAmount) <= maxAmount)
        {
            SetFirstBlockPos(true);
            ToggleNextBlock(true);
            blocksAmount += addedAmount;
            UIManager.Instance.LocalSetupUIIcons();
            OnAmountChanged?.Invoke(blocksAmount);
            Debug.Log("addedAmount " + addedAmount);
        }
        else if (addedAmount < 0 && (blocksAmount + addedAmount) >= minAmount)
        {
            SetFirstBlockPos(false);
            ToggleNextBlock(false);
            blocksAmount += addedAmount;
            UIManager.Instance.LocalSetupUIIcons();
            OnAmountChanged?.Invoke(blocksAmount);
            Debug.Log("subtructed amount " + addedAmount);
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
            block.gameObject.GetComponent<Renderer>().material = stateMaterial;
        }
    }

    //public void ChangeAmount(int addedAmount)
    //{
    //    if (addedAmount < 0 && transform.localScale.y <= 1 && transform.localRotation.z >= 0) // flip from desert to pond 
    //    {
    //        //transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, -180);
    //        newAmount = Mathf.RoundToInt(transform.localScale.y);
    //        UIManager.Instance.LocalSetupUIIcons();
    //        OnAmountChanged?.Invoke(newAmount);
    //        //Debug.Log("flip from desert to pond  " + transform.localRotation.z);
    //        return;
    //    }

    //    if (addedAmount > 0 && transform.localScale.y == 1 && transform.localRotation.z <= -0.1) //flip from pond to desert 
    //    {
    //        transform.localRotation = Quaternion.identity;
    //        newAmount = Mathf.RoundToInt(transform.localScale.y);
    //        UIManager.Instance.LocalSetupUIIcons();
    //        OnAmountChanged?.Invoke(newAmount);
    //        //Debug.Log("flip from pond to desert  " + transform.localRotation.z);
    //        return;
    //    }

    //    if (addedAmount > 0 && transform.localScale.y >= 1 && transform.localRotation.z >= 0)  // from desert  to forest
    //    {
    //        transform.localRotation = Quaternion.identity;
    //        transform.localScale += new Vector3(0, addedAmount, 0);
    //        newAmount = Mathf.RoundToInt(transform.localScale.y);
    //        UIManager.Instance.LocalSetupUIIcons();
    //        OnAmountChanged?.Invoke(newAmount);
    //        //Debug.Log("from desert  to forest " + transform.localRotation.z);
    //        return;

    //    }

    //    if (addedAmount < 0 && transform.localScale.y > 1)
    //    {
    //        transform.localRotation = Quaternion.identity;
    //        transform.localScale += new Vector3(0, addedAmount, 0);
    //        newAmount = Mathf.RoundToInt(transform.localScale.y);
    //        UIManager.Instance.LocalSetupUIIcons();
    //        OnAmountChanged?.Invoke(newAmount);
    //        //Debug.Log("from forest or desert to desert or to pond  " + transform.localRotation.z);
    //        return;
    //    }

    //    if (addedAmount == 0) //setup at the start
    //    {
    //        transform.localScale += new Vector3(0, addedAmount, 0);
    //        newAmount = Mathf.RoundToInt(transform.localScale.y);
    //        UIManager.Instance.LocalSetupUIIcons();
    //        OnAmountChanged?.Invoke(newAmount);
    //        return;

    //    }
    //}

    public int GetNewHeight()
    {
        return newAmount;
    }

    public int GetStartScale()
    {
        return startScale;
    }

    //public void ChangeHighlightedColorl()
    //{
    //    if (renderer.material.HasColor("_BaseColor"))
    //    {
    //        renderer.material.color = GridOfPrefabs.Instance.GetColorOfHighlightedBlocks();
    //    }
    //    if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
    //    {
    //        renderer.material.SetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"), GridOfPrefabs.Instance.GetColorOfHighlightedBlocks());
          
    //    }

    //}

    //public void ChangeColorBack()
    //{
    //    if (renderer.material.HasColor("_BaseColor"))
    //    {
    //        renderer.material.color = defaultColor;
    //    }
    //    if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6") )
    //    {
    //        renderer.material.SetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"), defaultColor);
    //        renderer.material.SetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"), defaultBottomColor);
         
    //    }
    //}

    //public void ChangeMaterialBack()
    //{
    //    renderer.material = defaultMaterial;
    //    //ChangeColorBack();
    //}
//    public void ChangeSelectedMaterial()
//    {
//        renderer.material = GridOfPrefabs.Instance.GetMaterialOfSelectedBlocks();
//        //ChangeColorBack();
//    }
}
