using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum LevelState
{
    Desert,
    Forest,
    Pond
}
public class LocalLevelState : MonoBehaviour
{

    [SerializeField] LevelState startLevelState;
    [SerializeField] LevelState currentLevelState;
    [SerializeField] int heightToChangeLevel;
    BlockPrefab blockPrefab;
    Renderer renderer ;
    private void Awake()
    {
        blockPrefab = GetComponent<BlockPrefab>();
        renderer = GetComponentInChildren<Renderer>();
        blockPrefab.OnHeightChanged += ChangeLevel;
    }
    public LevelState GetCurrentLevelState()
    {
        return currentLevelState;
    }

    public void ChangeLevel(int newHeight)
    {
        if (newHeight <= 1 && blockPrefab.transform.localRotation.z <= -1)
        {
            currentLevelState = LevelState.Pond;
            renderer.material = BuildingManager.Instance.levelsMaterials[0];
            blockPrefab.defaultMaterial = renderer.material;
            if (renderer.material.HasColor("_BaseColor")) { blockPrefab.defaultColor = renderer.material.color; }
            else if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
            { 
                blockPrefab.defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
                blockPrefab.defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));
            }
            UIManager.Instance.LocalSetupUIIcons();
            //Debug.Log(" blockPrefab.transform.localRotation.z " + blockPrefab.transform.localRotation.z);
        }

        if (newHeight >= 1 && blockPrefab.transform.localRotation.z >= 0)
        {
            currentLevelState = LevelState.Desert;
            renderer.material = BuildingManager.Instance.levelsMaterials[1];
            blockPrefab.defaultMaterial = renderer.material;
            if (renderer.material.HasColor("_BaseColor")) { blockPrefab.defaultColor = renderer.material.color; }
            else if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
            {
                blockPrefab.defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
                blockPrefab.defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));
            }
            UIManager.Instance.LocalSetupUIIcons();
            //Debug.Log(" blockPrefab.transform.localRotation.z " + blockPrefab.transform.localRotation.z);

        }
        if (newHeight <= 5 && newHeight > 1)
        {
            currentLevelState = LevelState.Desert;
            renderer.material = BuildingManager.Instance.levelsMaterials[1];
            blockPrefab.defaultMaterial = renderer.material;
            if (renderer.material.HasColor("_BaseColor")) { blockPrefab.defaultColor = renderer.material.color; }
            else if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
            {
                blockPrefab.defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
                blockPrefab.defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));
            }
            UIManager.Instance.LocalSetupUIIcons();
        }

        if (newHeight > 5)
        {
            currentLevelState = LevelState.Forest;
            renderer.material = BuildingManager.Instance.levelsMaterials[2];
            blockPrefab.defaultMaterial = renderer.material;
            if (renderer.material.HasColor("_BaseColor")) { blockPrefab.defaultColor = renderer.material.color; }
            else if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
            {
                blockPrefab.defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
                blockPrefab.defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));
            }
            UIManager.Instance.LocalSetupUIIcons();
        }
    }
   
}
