using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool IsBlockDead = false;
    BlockPrefab block;

    [Header("🔄 State Flags")]
    public bool IsBlockInjuring = false;
    public bool HasDyingColor = false;
    public bool IsAttacked = false;
    public bool IsBeingDamaged;
    private void Awake()
    {
        generatedWaypoints = new Transform[step];
        block = GetComponent<BlockPrefab>();
    }
    private void Start()
    {
        FillTheListOfWaypints();
        UpdateWaypointsPosition(0);
        currentHealth = maxHealth;
        block.OnBlockHeightChanged += UpdateWaypointsPosition;
    }

    public void FillTheListOfWaypints()
    {
        //Set Destination to generated waypoint in circle
        float angleStep = 360 / step;
        for (int i = 1; i < step + 1; i++)
        {
            GameObject generatedWaypoint = new GameObject();
            generatedWaypoint.name = "generated waypoint";
            generatedWaypoint.transform.RotateAround(transform.position, Vector3.up, angleStep * i);
            Vector3 dir = (generatedWaypoint.transform.position - transform.position).normalized;
            Vector3 position = transform.position + dir * 4;
            generatedWaypoint.transform.position = new Vector3(position.x, block.transform.position.y - BlockPrefab.Offset.y, position.z);
            generatedWaypoints[i - 1] = generatedWaypoint.transform;
            generatedWaypoint.transform.SetParent(transform);
        }
    }

    private void UpdateWaypointsPosition(int addedAmount)
    {
        foreach (var waypoint in generatedWaypoints)
        {
            waypoint.transform.position = new Vector3(waypoint.transform.position.x, block.transform.position.y - BlockPrefab.Offset.y, waypoint.transform.position.z);
        }
    }

    public void Damage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0)
        {
            IsBlockDead = true;
            BuildingManager.Instance.blockList.TransferBlockToDeadList(this.gameObject);
            GetComponent<BlockPrefab>().SetStateMaterial(BuildingManager.Instance.deadBlockMaterial);
            gameObject.GetComponent<GridBuildingSystem>().GetAllPlacedObjectsOnTheBlock();
            GameManager.Instance.amountOfDeadBlocks++;
            GameManager.Instance.CheckIfAllBlocksAreDead();
        }

        if (currentHealth < maxHealth && !IsBlockDead)
        {
            IsBlockInjuring = true;
            HasDyingColor = false;
            IsAttacked = true;
        }    
    }

    public void SetDyingColor()
    {
        if(!IsBeingDamaged && currentHealth < maxHealth) 
        {
            ConvertInDyingColor();
        }
    }

    void ConvertInDyingColor()
    {
        block = GetComponent<BlockPrefab>();
    }
}
