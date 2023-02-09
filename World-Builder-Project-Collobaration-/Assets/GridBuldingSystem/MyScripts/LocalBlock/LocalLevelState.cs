using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum LevelState
{
    Desert,
    Forest,
    Pond,
    Mountain,
    SnowMountain,
    Hill
}
public class LocalLevelState : MonoBehaviour
{

    [SerializeField] LevelState startLevelState;
    [SerializeField] LevelState currentLevelState;
    [SerializeField] int heightToChangeLevel;
    BlockPrefab blockPrefab;
    Renderer renderer;
    public Action OnChangedState;
    BlockHealth blHealth;
    private void Awake()
    {
        blockPrefab = GetComponent<BlockPrefab>();
        renderer = GetComponentInChildren<Renderer>();
        blHealth = GetComponent<BlockHealth>();
    }
    private void OnEnable()
    {
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
            if (currentLevelState != LevelState.Pond)
            {
                OnChangedState?.Invoke();
            }
            renderer.material = BuildingManager.Instance.levelsMaterials[0];
            blockPrefab.defaultMaterial = renderer.material;
            if (renderer.material.HasColor("_BaseColor")) { blockPrefab.defaultColor = renderer.material.color; }
            if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
            {
                blockPrefab.defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
                blockPrefab.defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));

            }
           
            currentLevelState = LevelState.Pond;
            blHealth.SetDyingColor();
            UIManager.Instance.LocalSetupUIIcons();
            blockPrefab.reflectionBlock.SetActive(false);
        }

        if (newHeight >= 1 && blockPrefab.transform.localRotation.z >= 0)
        {
            if (currentLevelState != LevelState.Desert)
            {
                OnChangedState?.Invoke();
            }
            //if (currentLevelState != LevelState.Desert)
            //{ Debug.Log("State was changed"); }
            renderer.material = BuildingManager.Instance.levelsMaterials[1];
            blockPrefab.defaultMaterial = renderer.material;
            if (renderer.material.HasColor("_BaseColor")) { blockPrefab.defaultColor = renderer.material.color; }
            else if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
            {
                blockPrefab.defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
                blockPrefab.defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));
            }
           
            currentLevelState = LevelState.Desert;
            blHealth.SetDyingColor();
            UIManager.Instance.LocalSetupUIIcons();
            blockPrefab.blockInside.gameObject.SetActive(false);
            //Debug.Log(" blockPrefab.transform.localRotation.z " + blockPrefab.transform.localRotation.z);

        }
        if (newHeight < 5 && newHeight > 1) // Desert
        {
            if (currentLevelState != LevelState.Desert)
            {
                OnChangedState?.Invoke();
            }
            renderer.material = BuildingManager.Instance.levelsMaterials[1];
            blockPrefab.defaultMaterial = renderer.material;
            if (renderer.material.HasColor("_BaseColor")) { blockPrefab.defaultColor = renderer.material.color; }
            if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
            {
                blockPrefab.defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
                blockPrefab.defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));
                //blockPrefab.defaultColor = Color.HSVToRGB(blHealth.H_1, blHealth.S_1, blHealth.V_1);
                //blockPrefab.defaultBottomColor = Color.HSVToRGB(blHealth.H_2, blHealth.S_2, blHealth.V_2);
            }

            
            currentLevelState = LevelState.Desert;
            blHealth.SetDyingColor();
            UIManager.Instance.LocalSetupUIIcons();
            blockPrefab.blockInside.gameObject.SetActive(false);
            blockPrefab.reflectionBlock.SetActive(true);
        }

        if (newHeight >= 5 && newHeight <= 8) // Forest
        {
            if (currentLevelState != LevelState.Forest)
            {
                OnChangedState?.Invoke();
            }
            renderer.material = BuildingManager.Instance.levelsMaterials[2];
            blockPrefab.defaultMaterial = renderer.material;
            if (renderer.material.HasColor("_BaseColor")) { blockPrefab.defaultColor = renderer.material.color; }
            if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
            {
                blockPrefab.defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
                blockPrefab.defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));
                //blockPrefab.defaultColor = Color.HSVToRGB(blHealth.H_1, blHealth.S_1, blHealth.V_1);
                //blockPrefab.defaultBottomColor = Color.HSVToRGB(blHealth.H_2, blHealth.S_2, blHealth.V_2);
            }
           
            currentLevelState = LevelState.Forest;
            blHealth.SetDyingColor();
            UIManager.Instance.LocalSetupUIIcons();
            blockPrefab.blockInside.gameObject.SetActive(true);
            blockPrefab.reflectionBlock.SetActive(true);
        }

        if (newHeight > 8  && newHeight < 11) // Hill
        {
            if (currentLevelState != LevelState.Hill)
            {
                OnChangedState?.Invoke();
            }
            renderer.material = BuildingManager.Instance.levelsMaterials[3];
            blockPrefab.defaultMaterial = renderer.material;
            if (renderer.material.HasColor("_BaseColor")) { blockPrefab.defaultColor = renderer.material.color; }
            if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
            {
                blockPrefab.defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
                blockPrefab.defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));
                //blockPrefab.defaultColor = Color.HSVToRGB(blHealth.H_1, blHealth.S_1, blHealth.V_1);
                //blockPrefab.defaultBottomColor = Color.HSVToRGB(blHealth.H_2, blHealth.S_2, blHealth.V_2);
            }
           
            currentLevelState = LevelState.Hill;
            blHealth.SetDyingColor();
            UIManager.Instance.LocalSetupUIIcons();
            blockPrefab.blockInside.gameObject.SetActive(false);
            blockPrefab.reflectionBlock.SetActive(true);
        }

        if (newHeight >= 11 && newHeight < 15) // Montain
        {
            if (currentLevelState != LevelState.Mountain)
            {
                OnChangedState?.Invoke();
            }
            renderer.material = BuildingManager.Instance.levelsMaterials[4];
            blockPrefab.defaultMaterial = renderer.material;
            if (renderer.material.HasColor("_BaseColor")) { blockPrefab.defaultColor = renderer.material.color; }
            if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
            {
                blockPrefab.defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
                blockPrefab.defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));
                //blockPrefab.defaultColor = Color.HSVToRGB(blHealth.H_1, blHealth.S_1, blHealth.V_1);
                //blockPrefab.defaultBottomColor = Color.HSVToRGB(blHealth.H_2, blHealth.S_2, blHealth.V_2);
            }
          
            currentLevelState = LevelState.Mountain;
            blHealth.SetDyingColor();
            UIManager.Instance.LocalSetupUIIcons();
            blockPrefab.blockInside.gameObject.SetActive(false);
            blockPrefab.reflectionBlock.SetActive(true);
        }

        if (newHeight >= 15) //Snow mountain
        {
            if (currentLevelState != LevelState.SnowMountain)
            {
                OnChangedState?.Invoke();
            }
            renderer.material = BuildingManager.Instance.levelsMaterials[5];
            blockPrefab.defaultMaterial = renderer.material;
            if (renderer.material.HasColor("_BaseColor")) { blockPrefab.defaultColor = renderer.material.color; }
            if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
            {
                blockPrefab.defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
                blockPrefab.defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));
                //blockPrefab.defaultColor = Color.HSVToRGB(blHealth.H_1, blHealth.S_1, blHealth.V_1);
                //blockPrefab.defaultBottomColor = Color.HSVToRGB(blHealth.H_2, blHealth.S_2, blHealth.V_2);
            }
         
            currentLevelState = LevelState.SnowMountain;
            blHealth.SetDyingColor();
            UIManager.Instance.LocalSetupUIIcons();
            blockPrefab.blockInside.gameObject.SetActive(false);
            blockPrefab.reflectionBlock.SetActive(true);
        }

    }

}
