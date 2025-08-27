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
        blockPrefab.OnBlockHeightChanged += ChangeState;
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
           
            currentLevelState = LevelState.SnowMountain;
            blHealth.SetDyingColor();
            UIManager.Instance.LocalSetupUIIcons();
        }

    }

}
