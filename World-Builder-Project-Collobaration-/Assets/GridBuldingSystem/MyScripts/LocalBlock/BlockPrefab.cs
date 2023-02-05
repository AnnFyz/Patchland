using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlockPrefab : MonoBehaviour
{
    //public static Vector3 offset = new Vector3(7.5f, -5f, 7.5f);
    public static Vector3 offset = new Vector3(6f, -5f, 6f);
    private int newHeight;
    private int startScale;
    public event Action<int> OnHeightChanged;
    public bool IsThisBlockWasHighlighted = false;
    public bool IsThisBlockWasSelected = false;
    public Renderer renderer;
    public Color defaultColor = new Color();
    public Color defaultBottomColor = new Color();
    public float origin;
    public Material defaultMaterial;
    float minOrigin;
    float maxOrigin;
    float duration = 2.0f;
    float startTime;
    float t;
    public Transform blockInside;
    public Renderer blockInsiderenderer;
    public GameObject reflectionBlock;
    private void Start()
    {
        blockInside = gameObject.transform.GetChild(0).GetChild(0);
        blockInside.gameObject.SetActive(false);
        startScale = Mathf.FloorToInt(this.gameObject.transform.GetChild(0).localScale.y + 4.0f);
        renderer = GetComponentInChildren<Renderer>();
        blockInsiderenderer = blockInside.GetComponent<Renderer>();
        if (renderer.material.HasColor("_BaseColor"))
        {
            defaultColor = renderer.material.color;
            blockInsiderenderer.material.color = defaultColor;
        }
        if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
        {
            defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
            defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));
            origin = renderer.material.GetFloat(Shader.PropertyToID("Vector1_6e12275293314cb7a52c177f83f8f9aa"));
            blockInsiderenderer.material.SetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"), defaultColor);
            blockInsiderenderer.material.SetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"), defaultBottomColor);
        }
        minOrigin = -0f;
        maxOrigin =  0.75f;
        startTime = Time.deltaTime;
        t = UnityEngine.Random.Range(0.5f, 7f);
    }
    public static BlockPrefab Create(Vector3 worldPosition, GameObject blockPrefab)
    {
        GameObject placedBlockPrefabObj = Instantiate(blockPrefab, worldPosition + offset, Quaternion.identity);
        BlockPrefab placedBlockPrefab = placedBlockPrefabObj.GetComponent<BlockPrefab>();
        return placedBlockPrefab;
    }

    private void Update()
    {
        ChangeOriginOfGradient();
    }

    void ChangeOriginOfGradient()
    {
        float v = Mathf.PingPong(Time.time, t);
        origin = Mathf.SmoothStep(minOrigin, maxOrigin, v * 0.5f);
        renderer.material.SetFloat("Vector1_6e12275293314cb7a52c177f83f8f9aa", origin);
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
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
            //Debug.Log("flip from pond to desert  " + transform.localRotation.z);
            return;
        }

        if (addedHeight > 0 && transform.localScale.y >= 1 && transform.localRotation.z >= 0)  // from desert  to forest
        {
            transform.localRotation = Quaternion.identity;
            transform.localScale += new Vector3(0, addedHeight, 0);
            newHeight = Mathf.RoundToInt(transform.localScale.y);
            UIManager.Instance.LocalSetupUIIcons();
            OnHeightChanged?.Invoke(newHeight);
            //Debug.Log("from desert  to forest " + transform.localRotation.z);
            return;

        }

        if (addedHeight < 0 && transform.localScale.y > 1)
        {
            transform.localRotation = Quaternion.identity;
            transform.localScale += new Vector3(0, addedHeight, 0);
            newHeight = Mathf.RoundToInt(transform.localScale.y);
            UIManager.Instance.LocalSetupUIIcons();
            OnHeightChanged?.Invoke(newHeight);
            //Debug.Log("from forest or desert to desert or to pond  " + transform.localRotation.z);
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
        if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
        {
            renderer.material.SetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"), GridOfPrefabs.Instance.GetColorOfHighlightedBlocks());
            blockInsiderenderer.material.SetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"), GridOfPrefabs.Instance.GetColorOfHighlightedBlocks());
        }

    }

    public void ChangeColorBack()
    {
        if (renderer.material.HasColor("_BaseColor"))
        {
            renderer.material.color = defaultColor;
        }
        if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6") )
        {
            renderer.material.SetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"), defaultColor);
            renderer.material.SetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"), defaultBottomColor);
            blockInsiderenderer.material.SetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"), defaultColor);
            blockInsiderenderer.material.SetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"), defaultBottomColor);
        }
    }

    public void ChangeMaterialBack()
    {
        renderer.material = defaultMaterial;
        //ChangeColorBack();
    }
    public void ChangeSelectedMaterial()
    {
        renderer.material = GridOfPrefabs.Instance.GetMaterialOfSelectedBlocks();
        //ChangeColorBack();
    }
}
