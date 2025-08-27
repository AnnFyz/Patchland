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
        // If no target block or it's dead → find another
        if (targetBlockHealth == null || targetBlockHealth.IsBlockDead)
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

    private void OnTriggerEnter(Collider other)
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
        // Clear current target
        targetBlock = null;
        targetBlockHealth = null;
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

                    NavMeshPath testPath = new NavMeshPath();
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
            targetBlockHealth = bestBlock;
            targetBlock = bestBlock.GetComponent<BlockPrefab>();
            validWaypoints = bestWaypoints;
            waypointIndex = 0;
            target = validWaypoints[waypointIndex];
            currentState = ZombiState.AttackBlock;

            agent.ResetPath();
            agent.SetDestination(target.position);

            Debug.Log($"[Zombie] Found new target block {targetBlock.name} with {validWaypoints.Count} reachable waypoints.");
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
