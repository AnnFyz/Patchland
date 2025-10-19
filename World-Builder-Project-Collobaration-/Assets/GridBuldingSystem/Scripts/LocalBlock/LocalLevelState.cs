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

/// <summary>
/// Tracks and updates the environmental state of a block (Pond, Desert, Forest, etc.)
/// based on its height. Updates materials, triggers events, and refreshes UI when the state changes.
/// </summary>
public class LocalLevelState : MonoBehaviour
{
    [Header("🌍 Level State")]
    [Tooltip("The current environment state of this block (e.g., Pond, Forest, Hill).")]
    [SerializeField] private LevelState currentLevelState;


    private BlockPrefab blockPrefab;
    private new Renderer renderer;
    BlockHealth blockHealth;

    public Action OnChangedState;
    private void Awake()
    {
        blockPrefab = GetComponent<BlockPrefab>();
        renderer = GetComponentInChildren<Renderer>();
        blockHealth = GetComponent<BlockHealth>();
    }
    private void OnEnable()
    {
        blockPrefab.OnBlockHeightChanged += ChangeState;
    }
    public LevelState GetCurrentLevelState() => currentLevelState;

    public void ChangeState(int newAmount)
    {
        LevelState newState = GetLevelStateFromHeight(newAmount);

        if (newState == currentLevelState) return;
        OnChangedState?.Invoke(); // ⚡ trigger event only when state actually changes

        // ⚡ material index matches enum order
        int materialIndex = (int)newState;
        Material stateMaterial = BuildingManager.Instance.levelsMaterials[materialIndex];

        renderer.material = stateMaterial;
        blockPrefab.SetStateMaterial(stateMaterial);

        currentLevelState = newState;
        UIManager.Instance.LocalSetupUIIcons();

    }

    // ⚡ helper method: maps block height to LevelState
    private LevelState GetLevelStateFromHeight(int height)
    {
        if (height == 1) return LevelState.Pond;
        if (height == 2) return LevelState.Desert;
        if (height > 2 && height <= 3) return LevelState.Forest;
        if (height > 3 && height <= 6) return LevelState.Hill;
        if (height > 6 && height <= 8) return LevelState.Mountain;
        if (height > 8 && height <= 10) return LevelState.SnowMountain;

        return currentLevelState; // fallback (shouldn't normally happen)
    }
}
