using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHealth : MonoBehaviour
{
    public Vector3 CenterOfBlock { get; set; }
    [SerializeField] int step = 3;
    public Transform[] generatedWaypoints;
    public float startHealth = 100f;
    public float currentHealth;
    public bool IsBlockDead = false;
    BlockPrefab block;
    public bool IsBlockInjuring = false;
    public bool HasDayingColor = false;
    public bool IsAttacked = false;
    public float ind_Vdif_1;
    public float ind_Sdif_1;
    public float ind_Vdif_2;
    public float ind_Sdif_2;
    [SerializeField] float lastDamage;
    [SerializeField] float newDamage;
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
        currentHealth = startHealth;
        block.OnBlockHeightChanged += UpdateWaypointsPosition;
    }

    public void FillTheListOfWaypints()
    {
        //Set Destination to generated waypoint in circle
        float angleStep = 360 / step;
        for (int i = 1; i < step + 1; i++)
        {
            GameObject generatedWaypoint = new GameObject();
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
        currentHealth = Mathf.Clamp(currentHealth, 0, startHealth);
        if (currentHealth <= 0)
        {
            IsBlockDead = true;
            BuildingManager.Instance.blockList.TransferBlockToDeadList(this.gameObject);
            GetComponent<BlockPrefab>().SetStateMaterial(BuildingManager.Instance.deadBlockMaterial);
            gameObject.GetComponent<GridBuildingSystem>().GetAllPlacedObjectsOnTheBlock();
            GameManager.Instance.amountOfDeadBlocks++;
            GameManager.Instance.CheckIfAllBlocksAreDead();
        }

        if (currentHealth < startHealth && !IsBlockDead)
        {
            IsBlockInjuring = true;
            HasDayingColor = false;
            IsAttacked = true;
        }

        newDamage -= damage;      
    }

    public void SetDyingColor()
    {
        if(!IsBeingDamaged && currentHealth < startHealth) 
        {
            ConvertInDyingColor();
        }
    }

    void ConvertInDyingColor()
    {
        block = GetComponent<BlockPrefab>();
        lastDamage = newDamage; 
    }
}
