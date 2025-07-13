using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum LevelState
{
    Pond,
    Desert,
    Forest,
    Hill,
    Mountain,
    SnowMountain,
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
        blockPrefab.OnAmountChanged += ChangeState;
    }
    public LevelState GetCurrentLevelState()
    {
        return currentLevelState;
    }

    public void ChangeState(int newAmount)
    {
        if (newAmount == 1) //Pond
        {
            if (currentLevelState != LevelState.Pond) //condition to not call the event if the state is not changed
            {
                OnChangedState?.Invoke();
            }
            renderer.material = BuildingManager.Instance.levelsMaterials[0];
            //blockPrefab.defaultMaterial = renderer.material;
            blockPrefab.SetStateMaterial(renderer.material);
            if (renderer.material.HasColor("_BaseColor")) { blockPrefab.defaultColor = renderer.material.color; }
            if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
            {
                blockPrefab.defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
                blockPrefab.defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));

            }
           
            currentLevelState = LevelState.Pond;
            blHealth.SetDyingColor();
            UIManager.Instance.LocalSetupUIIcons();
        }

        else if (newAmount == 2) // Desert
        {
            if (currentLevelState != LevelState.Desert)  //condition to not call the event if the state is not changed
            {
                OnChangedState?.Invoke();
            }
            renderer.material = BuildingManager.Instance.levelsMaterials[1];
            blockPrefab.SetStateMaterial(renderer.material);
            if (renderer.material.HasColor("_BaseColor")) { blockPrefab.defaultColor = renderer.material.color; }
            else if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
            {
                blockPrefab.defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
                blockPrefab.defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));
            }
           
            currentLevelState = LevelState.Desert;
            blHealth.SetDyingColor();
            UIManager.Instance.LocalSetupUIIcons();

        }
        else if (newAmount > 2 && newAmount <= 3) // Forest
        {
            if (currentLevelState != LevelState.Forest)  //condition to not call the event if the state is not changed
            {
                OnChangedState?.Invoke();
            }
            renderer.material = BuildingManager.Instance.levelsMaterials[2];
            blockPrefab.SetStateMaterial(renderer.material);
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
        }

        else if (newAmount > 3  && newAmount <= 6) // Hill
        {
            if (currentLevelState != LevelState.Hill)  //condition to not call the event if the state is not changed
            {
                OnChangedState?.Invoke();
            }
            renderer.material = BuildingManager.Instance.levelsMaterials[3];
            //blockPrefab.defaultMaterial = renderer.material;
            blockPrefab.SetStateMaterial(renderer.material);
            if (renderer.material.HasColor("_BaseColor")) { blockPrefab.defaultColor = renderer.material.color; }
            if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
            {
                blockPrefab.defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
                blockPrefab.defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));
            }
           
            currentLevelState = LevelState.Hill;
            blHealth.SetDyingColor();
            UIManager.Instance.LocalSetupUIIcons();
        }

        else if (newAmount > 6 && newAmount <= 8) // Montain
        {
            if (currentLevelState != LevelState.Mountain) //condition to not call the event if the state is not changed
            {
                OnChangedState?.Invoke();
            }
            renderer.material = BuildingManager.Instance.levelsMaterials[4];
            blockPrefab.SetStateMaterial(renderer.material);
            if (renderer.material.HasColor("_BaseColor")) { blockPrefab.defaultColor = renderer.material.color; }
            if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
            {
                blockPrefab.defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
                blockPrefab.defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));
            }

            currentLevelState = LevelState.Mountain;
            blHealth.SetDyingColor();
            UIManager.Instance.LocalSetupUIIcons();
        }

        else if (newAmount > 8 && newAmount <= 10) //Snow mountain
        {
            if (currentLevelState != LevelState.SnowMountain) //condition to not call the event if the state is not changed
            {
                OnChangedState?.Invoke();
            }
            renderer.material = BuildingManager.Instance.levelsMaterials[5];
            blockPrefab.SetStateMaterial(renderer.material);
            if (renderer.material.HasColor("_BaseColor")) { blockPrefab.defaultColor = renderer.material.color; }
            if (renderer.material.HasColor("Color_d3f90b46fa4040c48d4031973961bef6"))
            {
                blockPrefab.defaultColor = renderer.material.GetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"));
                blockPrefab.defaultBottomColor = renderer.material.GetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"));
            }
         
            currentLevelState = LevelState.SnowMountain;
            blHealth.SetDyingColor();
            UIManager.Instance.LocalSetupUIIcons();
        }

    }

}
