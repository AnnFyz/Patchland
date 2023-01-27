using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum ZombiState
{
    None,
    AttackUnit,
    AttackBlock,
    FindAnotherBlock,
    DeadZombi
}
public class Zombi : MonoBehaviour
{
    public ZombiState currentState;
    Unit unit;
    [SerializeField] BlockHealth occupiedBlockHealth;
    [SerializeField] BlockPrefab occupiedBlock;
    [SerializeField] BlockPrefab[] possibleNextOccupiedBlocks;
    private int waypointIndex = 0;
    private float elapsed = 0.0f;
    public Transform target;
    public NavMeshAgent agent;
    private NavMeshPath path;
    public bool isAttacking = false;
    [SerializeField] Renderer modelRenderer;
    [SerializeField] Color zombiTopColor = Color.white;
    [SerializeField] Color zombiBottomColor = Color.grey;
    private void Awake()
    {
        unit = GetComponent<Unit>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        modelRenderer = transform.GetChild(3).GetChild(0).GetComponent<Renderer>();
    }
    private void Start()
    {
        currentState = ZombiState.None;
        possibleNextOccupiedBlocks = new BlockPrefab[4];
        path = new NavMeshPath();
    }
    public void HandleZombiTransformation()
    {
        Debug.Log("I AM A ZOMBI NOW!");
        modelRenderer.material.SetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"), zombiTopColor);
        modelRenderer.material.SetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"), zombiBottomColor);
    }
    private void FixedUpdate()
    {
        HandleZombiMovement();
        if (currentState == ZombiState.FindAnotherBlock)
        {
            MoveToNextNeighbourAliveBlock();
        }
        if (occupiedBlockHealth != null && occupiedBlockHealth.IsBlockDead)
        {
            if (occupiedBlockHealth.currentHealth <= 0 && !isAttacking)
            {
                currentState = ZombiState.FindAnotherBlock;
                FindNeighboursBlocks();
            }
        }
    }

    public void HandleZombiMovement()
    {
        if (currentState == ZombiState.AttackBlock && occupiedBlockHealth != null && !occupiedBlockHealth.IsBlockDead)
        {
            // Update the way to the goal every second.
            elapsed += Time.deltaTime;
            IterateWaypointIndex();
            target = occupiedBlockHealth.generatedWaypoints[waypointIndex];
            if (target != null)
            {
                if (elapsed > 2f)
                {
                    elapsed -= 2f;
                    agent.SetDestination(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                    if (Vector3.Distance(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z), target.transform.position) < 10f)
                    {
                        Debug.Log("MOVE TO NEXT POINT");
                        HandleZombiMovement();
                    }
                    else
                    {
                        Debug.Log("Iterate");
                        IterateWaypointIndex();
                    }
                }
            }

        }
    }

    void IterateWaypointIndex()
    {
        waypointIndex++;
        if (waypointIndex == occupiedBlockHealth.generatedWaypoints.Length)
        {
            waypointIndex = 0;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<BlockHealth>())
        {
            if(occupiedBlockHealth == null) // first assignment
            {
                occupiedBlockHealth = other.GetComponentInParent<BlockHealth>();
                occupiedBlock = occupiedBlockHealth.GetComponent<BlockPrefab>();
            }

            else if (occupiedBlockHealth.currentHealth <= 0)  //reaasign occupied block only if the current one is dead
            {
                occupiedBlockHealth = other.GetComponentInParent<BlockHealth>();
                occupiedBlock = occupiedBlockHealth.GetComponent<BlockPrefab>();
            }
        }
    }

    public IEnumerator AttackBlock()
    {
        while (currentState == ZombiState.AttackBlock && occupiedBlockHealth != null)
        {
            if (occupiedBlockHealth.currentHealth > 0)
            {
                Debug.Log("ATTACK");
                isAttacking = true;
                occupiedBlockHealth.Damage(1f);               
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                isAttacking = false;
                break;
            }
        }
    }

    void FindNeighboursBlocks()
    {
        waypointIndex = 0;
        for (int x = 0; x < GridOfPrefabs.Instance.width; x++)
        {
            for (int y = 0; y < GridOfPrefabs.Instance.height; y++)
            {
                if (GridOfPrefabs.Instance.grid.GetGridObject(x, y).GetPlacedObject().Equals(occupiedBlock))
                {
                    if (GridOfPrefabs.Instance.grid.GetGridObject(x - 1, y) != null && GridOfPrefabs.Instance.grid.GetGridObject(x - 1, y).GetPlacedObject() != null)
                    {
                        possibleNextOccupiedBlocks[0] = GridOfPrefabs.Instance.grid.GetGridObject(x - 1, y).GetPlacedObject();
                    }

                    if (GridOfPrefabs.Instance.grid.GetGridObject(x + 1, y) != null && GridOfPrefabs.Instance.grid.GetGridObject(x + 1, y).GetPlacedObject() != null)
                    {
                        possibleNextOccupiedBlocks[1] = GridOfPrefabs.Instance.grid.GetGridObject(x + 1, y).GetPlacedObject();
                    }
                    if (GridOfPrefabs.Instance.grid.GetGridObject(x, y - 1) != null && GridOfPrefabs.Instance.grid.GetGridObject(x, y - 1).GetPlacedObject() != null)
                    {
                        possibleNextOccupiedBlocks[2] = GridOfPrefabs.Instance.grid.GetGridObject(x, y - 1).GetPlacedObject();
                    }
                    if (GridOfPrefabs.Instance.grid.GetGridObject(x, y + 1) != null && GridOfPrefabs.Instance.grid.GetGridObject(x, y + 1).GetPlacedObject() != null)
                    {
                        possibleNextOccupiedBlocks[3] = GridOfPrefabs.Instance.grid.GetGridObject(x, y + 1).GetPlacedObject();
                    }
                }
            }
        }

    }

    void MoveToNextNeighbourAliveBlock()
    {
        for (int i = 0; i < possibleNextOccupiedBlocks.Length; i++)
        {
            if (possibleNextOccupiedBlocks[i] != null)
            {
                Transform newTarget = possibleNextOccupiedBlocks[i].GetComponent<BlockHealth>().generatedWaypoints[1];
                if (agent.CalculatePath(new Vector3(newTarget.transform.position.x, transform.position.y, newTarget.transform.position.z), path) && !(possibleNextOccupiedBlocks[i].GetComponent<BlockHealth>().IsBlockDead))
                {
                  
                        Debug.Log("PATH WAS CALCULATED"); // WHY IT WAS CALLED SO MANY TIMES????
                        currentState = ZombiState.AttackBlock;
                        agent.SetDestination(new Vector3(newTarget.transform.position.x, transform.position.y, newTarget.transform.position.z));
                        StartCoroutine(AttackBlock());
                        break;
                }
                else if (i == possibleNextOccupiedBlocks.Length -1 && !agent.CalculatePath(new Vector3(newTarget.transform.position.x, transform.position.y, newTarget.transform.position.z), path) && !(possibleNextOccupiedBlocks[i].GetComponent<BlockHealth>().IsBlockDead))
                {
                    DestroyZombi();
                }
                else
                {
                    Debug.Log("THERE ARE NO WAY");
                    continue;
                }
            }
            else
            {
                Debug.Log("Block IS NULL");
                continue;
            }
        }
    }

    void DestroyZombi()
    {
        Destroy(gameObject);
    }
    
}
