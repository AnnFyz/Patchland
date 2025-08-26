using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;


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
    [SerializeField] float waypointReachThreshold = 0.3f;
    [SerializeField] float repathInterval = 0.35f;
    private float repathTimer = 0f;
    [SerializeField] List<Transform> validWaypoints = new List<Transform>(); // local cache of validated waypoints
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

    private void Update()
    {
        if (currentState == ZombiState.None || currentState == ZombiState.DeadZombi)
            return;

        // Ensure the agent has sensible settings
        if (agent != null)
        {
            agent.autoBraking = true;
            agent.stoppingDistance = waypointReachThreshold;
        }

        HandleZombiMovement();
    }


    public void HandleZombiMovement()
    {
        // If no target block or it's dead, try finding another block
        if (targetBlockHealth == null || targetBlockHealth.IsBlockDead)
        {
            MoveToNextNeighbourAliveBlock();
            if (targetBlockHealth == null || targetBlockHealth.IsBlockDead)
            {
                //if (agent != null && agent.hasPath) agent.ResetPath();
                //return;
                DestroyZombi();
                return;
            }
        }

        // Ensure we have valid/cached waypoints for the current target block
        if (validWaypoints == null || validWaypoints.Count == 0)
        {
            ValidateWaypoints(); // fills validWaypoints from targetBlockHealth
            if (validWaypoints.Count == 0)
            {
                Debug.LogWarning("[Zombie] No valid waypoints on block " + targetBlockHealth.name);
                //MoveToNextNeighbourAliveBlock();
                DestroyZombi();
                return;
            }
        }

        // Clamp waypointIndex and find the next reachable waypoint index
        waypointIndex = Mathf.Clamp(waypointIndex, 0, Mathf.Max(0, validWaypoints.Count - 1));
        int nextIndex = GetNextReachableWaypointIndex(waypointIndex, validWaypoints);
        if (nextIndex == -1)
        {
            // no reachable waypoint found — try neighboring block or fallback
            Debug.LogWarning("[Zombie] No reachable waypoint found on block " + targetBlockHealth.name);
            //MoveToNextNeighbourAliveBlock();
            DestroyZombi();
            return;
        }

        // Set target transform
        target = validWaypoints[nextIndex];
        waypointIndex = nextIndex; // use that index

        // Repath occasionally or when destination changed
        repathTimer += Time.deltaTime;
        bool destinationChanged = !agent.hasPath || (agent.destination - target.position).sqrMagnitude > 0.01f;

        if (destinationChanged || repathTimer >= repathInterval)
        {
            repathTimer = 0f;

            // Calculate path and set destination; we've already tested reachability in GetNextReachableWaypointIndex
            if (agent != null)
            {
                if (!agent.SetDestination(target.position))
                {
                    Debug.LogWarning("[Zombie] Failed to SetDestination to " + target.name + ". Destroying zombie.");
                    DestroyZombi();
                    return;
                }
            }
        }

        // Arrival check using agent.remainingDistance (more reliable)
        if (agent != null && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
        {
            IterateWaypointIndex();
        }
    }

    void IterateWaypointIndex()
    {
        if (validWaypoints == null || validWaypoints.Count == 0)
        {
            waypointIndex = 0;
            return;
        }

        waypointIndex++;
        if (waypointIndex >= validWaypoints.Count)
            waypointIndex = 0;
    }
    private void ValidateWaypoints()
    {
        validWaypoints = new List<Transform>();

        if (targetBlockHealth == null) return;
        var src = targetBlockHealth.generatedWaypoints;
        if (src == null || src.Length == 0) return;

        foreach (var t in src)
        {
            if (t == null) continue;
            if (!t.gameObject.activeInHierarchy) continue;
            // optional: ignore waypoints that are too close to each other or do other checks
            validWaypoints.Add(t);
        }

        // If you want, sort / reorder validWaypoints here
        Debug.Log($"[Zombie] Validated {validWaypoints.Count} waypoint(s) for block {targetBlockHealth.name}");
    }

    private int GetNextReachableWaypointIndex(int startIndex, List<Transform> candidates)
    {
        if (agent == null || candidates == null || candidates.Count == 0) return -1;

        for (int i = 0; i < candidates.Count; i++)
        {
            int idx = (startIndex + i) % candidates.Count;
            var candidate = candidates[idx];
            if (candidate == null) continue;

            // Quick check: is candidate active
            if (!candidate.gameObject.activeInHierarchy) continue;

            // Build a temp path to test reachability
            NavMeshPath testPath = new NavMeshPath();
            bool calc = agent.CalculatePath(candidate.position, testPath);
            if (!calc) continue;
            if (testPath.status == NavMeshPathStatus.PathComplete)
            {
                // Found reachable waypoint
                return idx;
            }
        }

        // nothing reachable
        return -1;
    }

    public void SetOccupiedBlock(Collider block)
    {
        if (block == null) return;

        var bh = block.GetComponentInParent<BlockHealth>();
        if (bh == null) return;

        targetBlockHealth = bh;
        targetBlock = bh.GetComponent<BlockPrefab>();
        waypointIndex = 0;
        ValidateWaypoints();
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
    

      private void OnCollisionEnter(Collision other)
    {
        if (other == null) return;

        var blockHealth = other.gameObject.GetComponentInParent<BlockHealth>();
        if (blockHealth != null && targetBlockHealth == blockHealth && !isOnTargetBlock)
        {
            isOnTargetBlock = true;

            var collector = other.gameObject.GetComponentInParent<ZombiCollector>();
            if (collector != null)
            {
                collector.CollectZombi(this);
            }
            else
            {
                Debug.LogWarning($"[Zombie] No ZombiCollector found for collider {other.gameObject.name} (parent: {other.transform.parent?.name})");
            }

            if (currentState == ZombiState.FindAnotherBlock)
            {
                currentState = ZombiState.AttackBlock;
                StartCoroutine(AttackBlock());
            }
        }
    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other == null) return;

    //    var blockHealth = other.GetComponentInParent<BlockHealth>();
    //    if (blockHealth != null && targetBlockHealth == blockHealth && !isOnTargetBlock)
    //    {
    //        isOnTargetBlock = true;

    //        var collector = other.GetComponentInParent<ZombiCollector>();
    //        if (collector != null)
    //        {
    //            collector.CollectZombi(this);
    //        }
    //        else
    //        {
    //            Debug.LogWarning($"[Zombie] No ZombiCollector found for collider {other.name} (parent: {other.transform.parent?.name})");
    //        }

    //        if (currentState == ZombiState.FindAnotherBlock)
    //        {
    //            currentState = ZombiState.AttackBlock;
    //            StartCoroutine(AttackBlock());
    //        }
    //    }
    //}

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
        targetBlock = null;
        targetBlockHealth = null;

        if (BuildingManager.Instance.blockList != null)
        {
            foreach (GameObject block in BuildingManager.Instance.blockList.healthyBlocks)
            {
                var bh = block.GetComponent<BlockHealth>();
                if (bh == null || bh.IsBlockDead) continue;

                float distance = (block.transform.position - transform.position).sqrMagnitude;
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    targetBlock = block.GetComponent<BlockPrefab>();
                    targetBlockHealth = bh;
                }
            }
        }
    }
    void MoveToNextNeighbourAliveBlock()
    {
        // Only proceed if the current target block is dead
        if (targetBlockHealth == null || !targetBlockHealth.IsBlockDead)
            return;

        targetBlockHealth.IsBeingDamaged = false;

        // Try to locate the nearest alive block
        LocateNearestBlock();

        if (targetBlockHealth != null && !targetBlockHealth.IsBlockDead)
        {
            // Found a new valid block
            waypointIndex = 0;
            ValidateWaypoints(); // Fill validWaypoints for new target
            if (validWaypoints.Count > 0)
            {
                currentState = ZombiState.AttackBlock;
                isOnTargetBlock = false;
                repathTimer = 0f;

                // Set initial destination
                target = validWaypoints[waypointIndex];
                if (agent != null)
                {
                    agent.ResetPath();
                    agent.SetDestination(target.position);
                }
            }
            else
            {
                // No valid waypoints on the new block
                Debug.LogWarning("[Zombie] New target block has no valid waypoints.");
                currentState = ZombiState.FindAnotherBlock; // optionally keep searching next frame
            }
        }
        else
        {
            // No valid block found anywhere
            Debug.LogWarning("[Zombie] No alive block found. Zombie will idle or be destroyed.");
            currentState = ZombiState.None; // stop movement
            agent.ResetPath();
            targetBlockHealth = null;
            targetBlock = null;
        }
    }

    public void DestroyZombi()
    {
        if(this == null || unit == null)
        {
            return;
        }
        UnitsManager.Instance.SetAmountOfUnits(unit.placedObjectName, -1);
        ParticleSystem particles = Instantiate(unit.UnitScriptableObject.death_Particles, transform.position, Quaternion.identity);
        particles.gameObject.AddComponent<AudioSource>().clip = unit.GlassBreaking;
        particles.gameObject.GetComponent<AudioSource>().volume = 0.01f;
        particles.gameObject.GetComponent<AudioSource>().Play();
        particles.gameObject.GetComponent<AudioSource>().loop = false;
        particles.Play();
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (validWaypoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var w in validWaypoints)
            {
                if (w != null) Gizmos.DrawSphere(w.position, 0.12f);
            }
        }
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(target.position, 0.18f);
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}
