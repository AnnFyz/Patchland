using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;


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
    public BlockHealth targetBlockHealth;
    public BlockPrefab targetBlock;
    public bool isOnTargetBlock = false; // if zombi is on the target block, it can attack it -> the Attack Coroutine will be started
    [SerializeField] BlockPrefab[] possibleNextOccupiedBlocks;
    private int waypointIndex = 0;
    private float elapsed = 0.0f;
    public Transform target;
    public NavMeshAgent agent;
    public NavMeshPath path;
    public bool isAttacking = false;
    [SerializeField] float damageToBlock; // how much damage zombi does to the block
    [SerializeField] float attackDelay; // how much time zombi needs to attack the block again
    [SerializeField] Renderer[] modelRenderers;
    [SerializeField] Color zombiTopColor = Color.white;
    [SerializeField] Color zombiBottomColor = Color.grey;
    [SerializeField] Material zombiMaterial;
    public GameObject attacking_Particles;
    [SerializeField] TMP_Text messageFroomZombi;
    int x = 0;
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
        attacking_Particles.gameObject.SetActive(false);
        damageToBlock = unit.unitScriptableObject.damageToBlock;
        attackDelay = unit.unitScriptableObject.attackDelay;
    }
    public void HandleZombiTransformation()
    {
        //modelRenderer.material.SetColor(Shader.PropertyToID("Color_d3f90b46fa4040c48d4031973961bef6"), zombiTopColor);
        //modelRenderer.material.SetColor(Shader.PropertyToID("Color_64d861fce71044349695d1bac7f2ea98"), zombiBottomColor);
        if (zombiMaterial != null)
        {
            ChangeMaterial(zombiMaterial);
        }

    }

    void ChangeMaterial(Material newMat)
    {
        Renderer[] oldMat = new Renderer[modelRenderers.Length];
        for (int i = 0; i < modelRenderers.Length; i++)
        {
            oldMat[i] = modelRenderers[i];
        }
        // Change the material of all renderers to the new material
        foreach (Renderer rend in oldMat)
        {
            var mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.materials.Length; j++)
            {
                mats[j] = newMat;
            }
            rend.materials = mats;
        }
    }
    private void FixedUpdate()
    {
        HandleZombiMovement();
        //if (currentState == ZombiState.FindAnotherBlock && unit.currentUnitsState == UnitsState.Zombi)
        //{
        //    MoveToNextNeighbourAliveBlock();
        //}
        //if (occupiedBlockHealth != null && occupiedBlockHealth.IsBlockDead && unit.currentUnitsState == UnitsState.Zombi)
        //{
        //    occupiedBlockHealth.IsBeingDamaged = false;
        //    if (occupiedBlockHealth.currentHealth <= 0 && !isAttacking)
        //    {
        //        currentState = ZombiState.FindAnotherBlock;
        //        LocateNearestBlock();
        //    }
        //}
    }

    public void HandleZombiMovement()
    {
        if (targetBlockHealth != null && !targetBlockHealth.IsBlockDead) //currentState == ZombiState.AttackBlock  && 
        {
            // Update the way to the goal every second.
            elapsed += Time.deltaTime;
            IterateWaypointIndex();
            target = targetBlockHealth.generatedWaypoints[waypointIndex];
            if (target != null)
            {
                if (elapsed > 2f)
                {
                    elapsed -= 2f;
                    agent.SetDestination(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                    if (Vector3.Distance(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z), target.transform.position) < 3f)
                    {
                        IterateWaypointIndex();
                    }
                    //else
                    //{
                    //    DestroyZombi();
                    //    //IterateWaypointIndex();
                    //}
                }
            }
            else
            {
                Debug.Log("Target is null, zombi will be destroyed");
                DestroyZombi();
            }

        }
    }

    void IterateWaypointIndex()
    {
        waypointIndex++;
        if (waypointIndex == targetBlockHealth.generatedWaypoints.Length)
        {
            waypointIndex = 0;
        }
    }

    public void SetOccupiedBlock(Collider block) //
    {
        if (currentState != ZombiState.None) {
            targetBlockHealth = block.GetComponentInParent<BlockHealth>();
            targetBlock = targetBlockHealth.GetComponent<BlockPrefab>();
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<BlockHealth>() && targetBlockHealth == other.GetComponentInParent<BlockHealth>() && !isOnTargetBlock)
        {
            isOnTargetBlock = true;
            other.GetComponent<ZombiCollector>().CollectZombi(this);
            //Debug.Log("OnTriggerEnter " + other.gameObject.name);
            //SetOccupiedBlock(other);
            if (currentState == ZombiState.FindAnotherBlock)
            {
                currentState = ZombiState.AttackBlock;
                StartCoroutine(AttackBlock());
                //if (!isAttacking)
                //{
                //    isAttacking = true;
                //    StartCoroutine(AttackBlock());
                //}

            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<BlockHealth>() && targetBlockHealth == other.GetComponentInParent<BlockHealth>() && isOnTargetBlock)
        {
            //isOnTargetBlock = false;
            //other.GetComponent<ZombiCollector>().RemoveZombiFromTheList(this);
            Debug.Log("RemoveZombiFromTheList " + this.gameObject.name);

        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    SetOccupiedBlock(other);
    //}
    public IEnumerator AttackBlock()
    {
        Debug.Log("Start Coroutine AttackBlock");
        while (currentState == ZombiState.AttackBlock && targetBlockHealth != null)
        {
            if (targetBlockHealth.currentHealth > 0)
            {
                isAttacking = true;
                targetBlockHealth.Damage(damageToBlock);
                targetBlockHealth.IsBeingDamaged = true;
                //Debug.Log("ATTACKIN BLOCK " + occupiedBlockHealth.gameObject.name);
                yield return new WaitForSeconds(attackDelay);
            }
            else
            {
                isAttacking = false;
                targetBlockHealth.IsBeingDamaged = false;
                waypointIndex = 0;
                Debug.Log("MoveToNextNeighbourAliveBlock");
                MoveToNextNeighbourAliveBlock();

                yield break;
            }
        }
        yield return new WaitForSeconds(0);
    }

    //void FindNeighboursBlocks()
    //{

    //    waypointIndex = 0;
    //    for (int x = 0; x < GridOfPrefabs.Instance.width; x++)
    //    {
    //        for (int y = 0; y < GridOfPrefabs.Instance.height; y++)
    //        {
    //            if (GridOfPrefabs.Instance.globalGrid.GetGridObject(x, y).GetPlacedObject().Equals(occupiedBlock))
    //            {
    //                if (GridOfPrefabs.Instance.globalGrid.GetGridObject(x - 1, y) != null && GridOfPrefabs.Instance.globalGrid.GetGridObject(x - 1, y).GetPlacedObject() != null)
    //                {
    //                    possibleNextOccupiedBlocks[0] = GridOfPrefabs.Instance.globalGrid.GetGridObject(x - 1, y).GetPlacedObject();
    //                }

    //                if (GridOfPrefabs.Instance.globalGrid.GetGridObject(x + 1, y) != null && GridOfPrefabs.Instance.globalGrid.GetGridObject(x + 1, y).GetPlacedObject() != null)
    //                {
    //                    possibleNextOccupiedBlocks[1] = GridOfPrefabs.Instance.globalGrid.GetGridObject(x + 1, y).GetPlacedObject();
    //                }
    //                if (GridOfPrefabs.Instance.globalGrid.GetGridObject(x, y - 1) != null && GridOfPrefabs.Instance.globalGrid.GetGridObject(x, y - 1).GetPlacedObject() != null)
    //                {
    //                    possibleNextOccupiedBlocks[2] = GridOfPrefabs.Instance.globalGrid.GetGridObject(x, y - 1).GetPlacedObject();
    //                }
    //                if (GridOfPrefabs.Instance.globalGrid.GetGridObject(x, y + 1) != null && GridOfPrefabs.Instance.globalGrid.GetGridObject(x, y + 1).GetPlacedObject() != null)
    //                {
    //                    possibleNextOccupiedBlocks[3] = GridOfPrefabs.Instance.globalGrid.GetGridObject(x, y + 1).GetPlacedObject();
    //                }
    //            }
    //        }
    //    }

    //}
    void LocateNearestBlock()
    {
        float nearestDistance = Mathf.Infinity;

        if (BuildingManager.Instance.blockList != null)
        {
            foreach (GameObject block in BuildingManager.Instance.blockList.healthyBlocks)
            {
                float distance = (block.transform.position - transform.position).sqrMagnitude;

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    targetBlock = block.GetComponent<BlockPrefab>();
                    targetBlockHealth = block.GetComponent<BlockHealth>();
                    //StopCoroutine(AttackBlock());
                }
            }
        }
        else
        {
            Debug.LogWarning("Target objects list is empty.");
        }
    }
    void MoveToNextNeighbourAliveBlock()
    {
        if (targetBlockHealth != null && targetBlockHealth.IsBlockDead && unit.currentUnitsState == UnitsState.Zombi)
        {
            targetBlockHealth.IsBeingDamaged = false;
            if (targetBlockHealth.currentHealth <= 0 && !isAttacking)
            {
                currentState = ZombiState.FindAnotherBlock;
                isOnTargetBlock = false;
                LocateNearestBlock();
                if(!isAttacking)
                {
                    isAttacking = true;
                    //StartCoroutine(AttackBlock());
                }
            }
        }
    }
    //void MoveToNextNeighbourAliveBlock()
    //{

    //    for (int i = 0; i < possibleNextOccupiedBlocks.Length; i++)
    //    {
    //        if (possibleNextOccupiedBlocks[i] != null)
    //        {
    //            Transform newTarget = possibleNextOccupiedBlocks[i].GetComponent<BlockHealth>().generatedWaypoints[1];
    //            if (agent.CalculatePath(new Vector3(newTarget.transform.position.x, transform.position.y, newTarget.transform.position.z), path) && !(possibleNextOccupiedBlocks[i].GetComponent<BlockHealth>().IsBlockDead))
    //            {

    //                    Debug.Log("PATH WAS CALCULATED"); // WHY IT WAS CALLED SO MANY TIMES????
    //                    currentState = ZombiState.AttackBlock;
    //                    agent.SetDestination(new Vector3(newTarget.transform.position.x, transform.position.y, newTarget.transform.position.z));
    //                    StartCoroutine(AttackBlock());
    //                    break;
    //            }
    //            else if (i == possibleNextOccupiedBlocks.Length -1 && !agent.CalculatePath(new Vector3(newTarget.transform.position.x, transform.position.y, newTarget.transform.position.z), path) && !(possibleNextOccupiedBlocks[i].GetComponent<BlockHealth>().IsBlockDead))
    //            {
    //                DestroyZombi();
    //            }
    //            else
    //            {
    //                x++;
    //                Debug.Log("THERE ARE NO WAY");
    //                if (x > 2000)
    //                {
    //                    DestroyZombi();
    //                }
    //                else
    //                {
    //                    continue;
    //                }

    //            }
    //        }
    //        else
    //        {
    //            x++;
    //            Debug.Log("Block is Dead");
    //            if (x > 2000)
    //            {
    //                DestroyZombi();
    //            }
    //            else
    //            {
    //                continue;
    //            }
    //        }
    //    }
    //}

    public void DestroyZombi()
    {
        UnitsManager.Instance.SetAmountOfUnits(unit.unitScriptableObject.unitId, -1);
        ParticleSystem particles = Instantiate(unit.unitScriptableObject.death_Particles, transform.position, Quaternion.identity);
        particles.gameObject.AddComponent<AudioSource>().clip = unit.glassBreaking;
        particles.gameObject.GetComponent<AudioSource>().volume = 0.01f;
        particles.gameObject.GetComponent<AudioSource>().Play();
        particles.gameObject.GetComponent<AudioSource>().loop = false;
        particles.Play();
        Destroy(gameObject);
    }

}
