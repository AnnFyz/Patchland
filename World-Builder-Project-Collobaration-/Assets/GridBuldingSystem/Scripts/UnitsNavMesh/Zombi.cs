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

/// <summary>
/// Handles zombie movement, targeting, and attacking logic using a NavMeshAgent.
/// </summary>
public class Zombi : MonoBehaviour
{
    [Header("⚙️ Navigation Settings")]
    [SerializeField] private float waypointReachThreshold = 0.3f;
    [SerializeField] private float repathInterval = 0.35f;
    private float repathTimer = 0f;


    [Header("📍 Waypoints")]
    [SerializeField] private List<Transform> validWaypoints = new();

    public ZombiState currentState {get; private set; }


    Unit unit;
    public BlockHealth TargetBlockHealth { get; private set; }
    public BlockPrefab TargetBlock { get; private set; }
    public bool IsOnTargetBlock { get; private set; } = false; // if zombi is on the target block, it can attack it -> the Attack Coroutine will be started

    private int waypointIndex = 0;
    public Transform target;


    [Header("🧟 Combat")]
    public bool isAttacking { get; private set; } = false;
    [SerializeField] float damageToBlock; // how much damage zombi does to the block
    [SerializeField] float attackDelay; // how much time zombi needs to attack the block again


    [Header("🎨 Visuals")]
    [SerializeField] private Renderer[] modelRenderers;
    [SerializeField] private Material zombiMaterial;
    [SerializeField] private Transform attacking_Particles;
    [SerializeField] TMP_Text messageFromZombi;


    private NavMeshAgent agent;
    private void Awake()
    {
        unit = GetComponent<Unit>();
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        currentState = ZombiState.None;

        if (attacking_Particles != null)
        { attacking_Particles.gameObject.SetActive(false); }

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
        // If no target block or it's dead → find another
        if (TargetBlockHealth == null || TargetBlockHealth.IsBlockDead)
        {
            MoveToNextNeighbourAliveBlock();
            return; // Either reassigned target, or zombie destroyed
        }

        // Ensure waypoints exist
        if (validWaypoints == null || validWaypoints.Count == 0)
        {
            MoveToNextNeighbourAliveBlock();
            return;
        }

        // Clamp index
        waypointIndex = Mathf.Clamp(waypointIndex, 0, validWaypoints.Count - 1);

        // Get current waypoint
        target = validWaypoints[waypointIndex];

        // Try path
        NavMeshPath tempPath = new NavMeshPath();
        if (!agent.CalculatePath(target.position, tempPath) || tempPath.status != NavMeshPathStatus.PathComplete)
        {
            Debug.LogWarning("[Zombie] Current waypoint unreachable. Searching another block...");
            MoveToNextNeighbourAliveBlock();
            return;
        }

        // Set destination (repath if needed)
        repathTimer += Time.deltaTime;
        bool needsRepath = !agent.hasPath || (agent.destination - target.position).sqrMagnitude > 0.01f;

        if (needsRepath || repathTimer >= repathInterval)
        {
            repathTimer = 0f;
            if (!agent.SetDestination(target.position))
            {
                Debug.LogWarning("[Zombie] Failed to set destination. Searching another block...");
                MoveToNextNeighbourAliveBlock();
                return;
            }
        }

        // Check arrival
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
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

        waypointIndex = (waypointIndex + 1) % validWaypoints.Count;
    }
    private void ValidateWaypoints()
    {
        validWaypoints.Clear();

        if (TargetBlockHealth == null) return;
        var src = TargetBlockHealth.generatedWaypoints;
        if (src == null || src.Length == 0) return;

        foreach (var t in src)
        {
            if (t == null) continue;
            if (!t.gameObject.activeInHierarchy) continue;
            // optional: ignore waypoints that are too close to each other or do other checks
            validWaypoints.Add(t);
        }

        // If you want, sort / reorder validWaypoints here
        Debug.Log($"[Zombie] Validated {validWaypoints.Count} waypoint(s) for block {TargetBlockHealth.name}");
    }

    public void SetInitialTargetBlock(BlockPrefab blockPrefab)
    {
        if (currentState != ZombiState.None) return;

        TargetBlockHealth = blockPrefab.GetComponentInParent<BlockHealth>();
        TargetBlock = blockPrefab;
    }

    public void SetOccupiedBlock(Collider block)
    {
        if (block == null) return;

        var bh = block.GetComponentInParent<BlockHealth>();
        if (bh == null) return;

        TargetBlockHealth = bh;
        TargetBlock = bh.GetComponent<BlockPrefab>();
        waypointIndex = 0;
        ValidateWaypoints();
    }

    public void HandleZombiTransformation()
    {  
        if (zombiMaterial != null)
        {
            currentState = ZombiState.AttackBlock;
            ChangeMaterial(zombiMaterial);
            attacking_Particles.gameObject.SetActive(true);
        }
    }
    void ChangeMaterial(Material newMat)
    {
        foreach (Renderer rend in modelRenderers)
        {
            if (rend == null) continue;
            var mats = new Material[rend.materials.Length];
            for (int j = 0; j < mats.Length; j++)
                mats[j] = newMat;

            rend.materials = mats;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        var collector = other.gameObject.GetComponentInParent<ZombiCollector>();
        if (collector != null)
        {
            collector.CollectZombi(this);
            Debug.Log($"[Zombie] Collected by ZombiCollector on block {other.gameObject.name} (parent: {other.transform.parent?.name})");
        }
        else
        {
            Debug.LogWarning($"[Zombie] No ZombiCollector found for collider {other.gameObject.name} (parent: {other.transform.parent?.name})");
        }


        var blockHealth = other.gameObject.GetComponentInParent<BlockHealth>();
        if (blockHealth != null && TargetBlockHealth == blockHealth && !IsOnTargetBlock)
        {
            IsOnTargetBlock = true;
            Debug.Log($"[Zombie] Reached target block {other.gameObject.name} (parent: {other.transform.parent?.name})");
            if (currentState == ZombiState.FindAnotherBlock)
            {
                currentState = ZombiState.AttackBlock;
                StartCoroutine(AttackBlock());
            }
        }
    }
    public IEnumerator AttackBlock()
    {
        while (currentState == ZombiState.AttackBlock && TargetBlockHealth != null)
        {
            if (TargetBlockHealth.currentHealth > 0)
            {
                isAttacking = true;
                TargetBlockHealth.Damage(damageToBlock);
                yield return new WaitForSeconds(attackDelay);
            }
            else
            {
                isAttacking = false;
                TargetBlockHealth.IsBeingDamaged = false;
                waypointIndex = 0;
                MoveToNextNeighbourAliveBlock();
                yield break;
            }
        } 
    }
    void MoveToNextNeighbourAliveBlock()
    {
        // Clear current target
        TargetBlock = null;
        TargetBlockHealth = null;
        validWaypoints.Clear();
        waypointIndex = 0;

        float nearestDistance = Mathf.Infinity;
        BlockHealth bestBlock = null;
        List<Transform> bestWaypoints = null;

        // Search all healthy blocks
        if (BuildingManager.Instance.blockList != null)
        {
            foreach (GameObject block in BuildingManager.Instance.blockList.healthyBlocks)
            {
                var bh = block.GetComponent<BlockHealth>();
                if (bh == null || bh.IsBlockDead) continue;

                // Validate its waypoints
                List<Transform> candidates = new List<Transform>();
                foreach (var t in bh.generatedWaypoints)
                {
                    if (t == null) continue;
                    if (!t.gameObject.activeInHierarchy) continue;

                    NavMeshPath testPath = new ();
                    if (agent.CalculatePath(t.position, testPath) && testPath.status == NavMeshPathStatus.PathComplete)
                    {
                        candidates.Add(t);
                    }
                }

                if (candidates.Count == 0) continue; // skip unreachable block

                // Pick nearest block
                float dist = (block.transform.position - transform.position).sqrMagnitude;
                if (dist < nearestDistance)
                {
                    nearestDistance = dist;
                    bestBlock = bh;
                    bestWaypoints = candidates;
                }
            }
        }

        // Did we find a valid block?
        if (bestBlock != null && bestWaypoints != null && bestWaypoints.Count > 0)
        {
            TargetBlockHealth = bestBlock;
            TargetBlock = bestBlock.GetComponent<BlockPrefab>();
            validWaypoints = bestWaypoints;
            waypointIndex = 0;
            target = validWaypoints[waypointIndex];
            currentState = ZombiState.AttackBlock;

            agent.ResetPath();
            agent.SetDestination(target.position);

            Debug.Log($"[Zombie] Found new target block {TargetBlock.name} with {validWaypoints.Count} reachable waypoints.");
        }
        else
        {
            Debug.LogWarning("[Zombie] No reachable blocks left. Destroying zombie.");
            DestroyZombi();
        }
    }

    public void DestroyZombi()
    {
        if(this == null || unit == null)
        {
            return;
        }

        // Play glass breaking particles and sound
        DeathParticles deathParticles = DeathParticles.Create(transform.position, unit.TargetBlock.transform, unit.UnitScriptableObject.death_Particles_Prefab, Quaternion.identity);
        deathParticles.Play(unit.GlassBreaking, 0.01f);


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
