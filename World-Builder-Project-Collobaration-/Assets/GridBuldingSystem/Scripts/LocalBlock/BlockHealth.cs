using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manages a block’s health, death state, and waypoints.
/// Handles damage, updates visuals, and reacts to height changes.
/// </summary>
public class BlockHealth : MonoBehaviour
{
    [Header("⚙️ Waypoints")]
    [Tooltip("Number of waypoints to generate around the block.")]
    [SerializeField] private int step = 3;

    [Tooltip("Number of waypoints to generate around the block.")]
    public Transform[] generatedWaypoints;

    [Header("❤️ Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    BlockPrefab block;

    [Header("🔄 State Flags")]
    public bool IsBeingDamaged;
    public bool IsBlockDead = false;
    private void Awake()
    {
        generatedWaypoints = new Transform[step];
        block = GetComponent<BlockPrefab>();
    }
    private void Start()
    {
        FillTheListOfWaypoints();
        UpdateWaypointsPosition(0);
        currentHealth = maxHealth;
        block.OnBlockHeightChanged += UpdateWaypointsPosition;
    }

    /// Generates waypoints around the block in a circular pattern.
    public void FillTheListOfWaypoints()
    {
        float angleStep = 360f / step;
        for (int i = 0; i < step; i++)
        {
            GameObject generatedWaypoint = new GameObject($"Waypoint_{i}");
            generatedWaypoint.transform.RotateAround(transform.position, Vector3.up, angleStep * (i + 1));
            Vector3 dir = (generatedWaypoint.transform.position - transform.position).normalized;
            Vector3 position = transform.position + dir * 4f;
            generatedWaypoint.transform.position = new Vector3(position.x, block.transform.position.y - BlockPrefab.Offset.y, position.z);
            generatedWaypoints[i] = generatedWaypoint.transform;
            generatedWaypoint.transform.SetParent(transform);
        }
    }

    /// Updates the vertical position of waypoints when block height changes.
    private void UpdateWaypointsPosition(int addedAmount)
    {
        foreach (var waypoint in generatedWaypoints)
        {
            if (waypoint == null) continue;
            waypoint.transform.position = new Vector3(
                waypoint.transform.position.x, 
                block.transform.position.y - BlockPrefab.Offset.y, 
                waypoint.transform.position.z
            );
        }
    }

    /// Apply damage to the block and handle death logic.
    public void Damage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0 && !IsBlockDead)
        {
            IsBlockDead = true;
            IsBeingDamaged = false;
            BuildingManager.Instance.blockList.TransferBlockToDeadList(gameObject);
            block.SetStateMaterial(BuildingManager.Instance.deadBlockMaterial);
            GetComponent<GridBuildingSystem>().RemoveAllPlacedObjectsFromBlock();
            GameManager.Instance.amountOfDeadBlocks++;
            GameManager.Instance.CheckIfAllBlocksAreDead();
        }

        if (currentHealth < maxHealth && !IsBlockDead)
        {
            IsBeingDamaged = true;
        }    
    }
}
