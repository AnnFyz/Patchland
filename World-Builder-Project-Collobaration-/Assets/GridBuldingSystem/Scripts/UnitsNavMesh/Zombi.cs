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
    [SerializeField] float movingToPointTimer = 3f;
    public ZombiState currentState;
    Unit unit;
    public BlockHealth targetBlockHealth;
    public BlockPrefab targetBlock;
    public bool isOnTargetBlock = false; // if zombi is on the target block, it can attack it -> the Attack Coroutine will be started
    private int waypointIndex = 0;
    private float elapsed = 0.0f;
    public Transform target;
    public NavMeshAgent agent;
    public NavMeshPath path;
    public bool isAttacking = false;
    [SerializeField] float damageToBlock; // how much damage zombi does to the block
    [SerializeField] float attackDelay; // how much time zombi needs to attack the block again
    [SerializeField] Renderer[] modelRenderers;
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
        path = new NavMeshPath();
        attacking_Particles.gameObject.SetActive(false);
        damageToBlock = unit.UnitScriptableObject.damageToBlock;
        attackDelay = unit.UnitScriptableObject.attackDelay;
    }

    private void FixedUpdate()
    {
        HandleZombiMovement();

    }
    public void HandleZombiMovement()
    {
        if (targetBlockHealth != null && !targetBlockHealth.IsBlockDead)
        {
            // Update the way to the goal every amount of sec in movingToPointTimer.
            elapsed += Time.deltaTime;
            target = targetBlockHealth.generatedWaypoints[waypointIndex];
            if (target != null)
            {
                if (elapsed > movingToPointTimer)
                {
                    elapsed = 0f;
                    agent.CalculatePath(target.transform.position, path);
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        if (agent.SetDestination(target.transform.position))
                        {
                            if (Vector3.Distance(transform.position, target.transform.position) < 3f)
                            {
                                Debug.Log("Waypoint approached: " + target.name);
                                IterateWaypointIndex();
                            }
                        }
                        else
                        {
                            Debug.Log("Failed to set destination to: " + target.name);
                            DestroyZombi();
                        }
                    }
                    else
                    {
                        Debug.Log("Path is not complete to: " + target.name);
                        DestroyZombi();
                    }
                }

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

    public void HandleZombiTransformation()
    {  
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

    public void SetOccupiedBlock(Collider block) 
    {
        if (currentState != ZombiState.None)
        {
            targetBlockHealth = block.GetComponentInParent<BlockHealth>();
            targetBlock = targetBlockHealth.GetComponent<BlockPrefab>();
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null)
        {
            return;
        }
        if (other.GetComponentInParent<BlockHealth>() && targetBlockHealth == other.GetComponentInParent<BlockHealth>() && !isOnTargetBlock)
        {
            isOnTargetBlock = true;
            other.GetComponent<ZombiCollector>().CollectZombi(this);
            if (currentState == ZombiState.FindAnotherBlock)
            {
                currentState = ZombiState.AttackBlock;
                StartCoroutine(AttackBlock());
            }
        }
    }

    public IEnumerator AttackBlock()
    {
        while (currentState == ZombiState.AttackBlock && targetBlockHealth != null)
        {
            if (targetBlockHealth.currentHealth > 0)
            {
                isAttacking = true;
                targetBlockHealth.Damage(damageToBlock);
                targetBlockHealth.IsBeingDamaged = true;
                yield return new WaitForSeconds(attackDelay);
            }
            else
            {
                isAttacking = false;
                targetBlockHealth.IsBeingDamaged = false;
                waypointIndex = 0;
                MoveToNextNeighbourAliveBlock();

                yield break;
            }
        }
        yield return new WaitForSeconds(0);
    }

    // // This method finds the nearest block to the zombi and sets it as the target block.
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
                if (!isAttacking)
                {
                    isAttacking = true;
                }
            }
        }
    }
    public void DestroyZombi()
    {
        UnitsManager.Instance.SetAmountOfUnits(unit.placedObjectName, -1);
        ParticleSystem particles = Instantiate(unit.UnitScriptableObject.death_Particles, transform.position, Quaternion.identity);
        particles.gameObject.AddComponent<AudioSource>().clip = unit.GlassBreaking;
        particles.gameObject.GetComponent<AudioSource>().volume = 0.01f;
        particles.gameObject.GetComponent<AudioSource>().Play();
        particles.gameObject.GetComponent<AudioSource>().loop = false;
        particles.Play();
        Destroy(gameObject);
    }


}
