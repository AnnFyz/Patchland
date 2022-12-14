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
    void ChangeLevel(int newHeight)
    {
        if(newHeight <= 0)
        {
            currentLevelState = LevelState.Pond;
            renderer.material = BuildingManager.Instance.levelsMaterials[0];
            blockPrefab.defaultColor = renderer.material.color;
        }

        else if(newHeight <= 5)
        {
            currentLevelState = LevelState.Desert;
            renderer.material = BuildingManager.Instance.levelsMaterials[1];
            blockPrefab.defaultColor = renderer.material.color;
        }

        else if (newHeight > 5)
        {
            currentLevelState = LevelState.Forest;
            renderer.material = BuildingManager.Instance.levelsMaterials[2];
            blockPrefab.defaultColor = renderer.material.color;
        }
    }
}
