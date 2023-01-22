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
    float H_1; float oldH_1;
    float S_1; float oldS_1;
    float V_1; float oldV_1;

    float H_2; float oldH_2;
    float S_2; float oldS_2;
    float V_2; float oldV_2;
    public bool OLDISDEAD = false;
    private void Awake()
    {
        unit = GetComponent<Unit>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
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
            currentState = ZombiState.FindAnotherBlock;
            FindNeighboursBlocks();
            OLDISDEAD = true;
        }
        else
        {
            OLDISDEAD = false;
        }
    }
    public void HandleZombiMovement()
    {
        if (currentState == ZombiState.AttackBlock && !occupiedBlockHealth.IsBlockDead)
        {
            // Update the way to the goal every second.
            elapsed += Time.deltaTime;
            IterateWaypointIndex();
            target = occupiedBlockHealth.generatedWaypoints[waypointIndex];
            if (target != null)
            {
                if (elapsed > 0.2f)
                {
                    elapsed -= 0.2f;
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
        //if (occupiedBlockHealth == null)
        //{
        if (other.GetComponentInParent<BlockHealth>())
        {
            occupiedBlockHealth = other.GetComponentInParent<BlockHealth>();
            occupiedBlock = occupiedBlockHealth.GetComponent<BlockPrefab>();
            //StartCoroutine(AttackBlock()); Debug.Log("COROUTINE");
            Color.RGBToHSV(occupiedBlock.defaultColor, out H_1, out S_1, out V_1);
            Color.RGBToHSV(occupiedBlock.defaultBottomColor, out H_2, out S_2, out V_2);
            //oldH_1 = H_1;
            //oldS_1 = S_1;
            //oldV_1 = V_1;

            //oldH_2 = H_2;
            //oldS_2 = S_2;
            //oldV_2 = V_2;
        }
        // }
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.GetComponentInParent<BlockHealth>())
    //    {
    //        StopCoroutine(AttackBlock()); Debug.Log("COROUTINE");
    //    }
    //}
        void ResetColor()
    {
        H_1 = oldH_1;
        S_1 = oldS_1;
        V_1 = oldV_1;

        H_2 = oldH_2;
        S_2 = oldS_2;
        V_2 = oldV_2;
    }
    public IEnumerator AttackBlock()
    {
        while (currentState == ZombiState.AttackBlock && occupiedBlockHealth != null)
        {

            occupiedBlockHealth.Damage(1f);

            V_1 -= 0.005f;
            S_1 -= 0.005f;
            S_2 -= 0.005f;
            V_2 += 0.005f;
            V_1 = Mathf.Clamp(V_1, 0.4f, 1f);
            S_1 = Mathf.Clamp(S_1, 0.01f, 0.75f);
            S_2 = Mathf.Clamp(S_2, 0.001f, 0.9f);
            V_2 = Mathf.Clamp(V_2, 0.025f, 0.75f);
            occupiedBlock.defaultColor = Color.HSVToRGB(H_1, S_1, V_1);
            occupiedBlock.defaultBottomColor = Color.HSVToRGB(H_2, S_2, V_2);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.1f);
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

        foreach (var newOccupiedBlock in possibleNextOccupiedBlocks)
        {
            if (newOccupiedBlock != null)
            {
                Transform newTarget = newOccupiedBlock.GetComponent<BlockHealth>().generatedWaypoints[1];
                //if (Vector3.Distance(transform.position, new Vector3(newTarget.transform.position.x, transform.position.y, newTarget.transform.position.z)) < 5f)
                if (agent.CalculatePath(new Vector3(newTarget.transform.position.x, transform.position.y, newTarget.transform.position.z), path) && !newOccupiedBlock.GetComponent<BlockHealth>().IsBlockDead)
                {
                    Debug.Log("PATH WAS CALCULATED"); // WHY IT WAS CALLED SO MANY TIMES????
                    agent.SetDestination(new Vector3(newTarget.transform.position.x, transform.position.y, newTarget.transform.position.z));
                    currentState = ZombiState.AttackBlock;
                    StartCoroutine(AttackBlock());
                    break;
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
}
