using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static PlacedObjectTypeSO;
using static Unity.Collections.AllocatorManager;


// This enum represents the different movement states of a unit, such as autopilot or controlled by the player.
public enum UnitsMovementState
{
    Autopilot,
    ControlledFromPlayer
}

// This enum represents the different states a unit can be in, such as alive, hungry, attacked, dead, or zombified.
public enum UnitsState 
{
    Alive,
    Hungry,
    Attacked,
    Dead,
    Zombi
}

[Serializable]
public class WaypointsList
{
    public List<Transform> localOrder = new List<Transform>();
}

//This class represents a unit in the game, which can be controlled by the player or move automatically.
//The unit can interact with placed objects and has chance to become a zombie, if it dies
[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
    [Header("Unit Scriptable Object")]
    [SerializeField] UnitsTypeSO unitScriptableObject;
    public UnitsTypeSO UnitScriptableObject => unitScriptableObject;
    [Header("Unit Sounds")] //TO DO Sound logic
    [SerializeField] AudioClip zombieSound;
    public AudioClip ZombieSound => zombieSound;
    [SerializeField] AudioClip glassBreaking;
    public AudioClip GlassBreaking => glassBreaking;
    AudioSource audioSource;

    [Header("Unit Animation")]
    [SerializeField] Animator animator; // to handle the animations of the unit

    private GameObject selectedFigur; // to show that the unit is selected

    [Header("Unit Movement")]
    private NavMeshAgent Agent { get; set; }
    private NavMeshPath path;
    public Transform StartPoint { get; set; }
    [SerializeField] Transform target; // the target to which the unit is moving
    [SerializeField] int waypointIndex = 0;
    [SerializeField] WaypointsList waypointsList = new WaypointsList();
    [SerializeField] float movingToPointTimer;
    float elapsed = 0.0f;

    [Header("Related Placed Object")]
    public PlacedObjectName placedObjectName; // to get the type of the placed object, so that we can get the waypoints for it
    public int PlacedObjTypeId { get; set; }
    public UnitsMovementState CurrentMovemenetState { get; set; } // to track the state of the unit movement (autopilot, controlled by player)
    public UnitsState CurrentUnitsState { get; set; } // to track the state of the unit (alive, dead, zombi, etc.)
    private Zombi zombi; // to handle the zombi state of the unit
    private BlockPrefab intersectedWithUnitBlock; // to handle the block that the unit is currently intersecting with
    public PlacedObject_Done currentPlacedObject = null; // to handle the placed object that the unit is currently interacting with
    private void Awake()
    {
        selectedFigur = gameObject.transform.GetChild(0).gameObject;
        Agent = GetComponent<NavMeshAgent>();
        zombi = GetComponent<Zombi>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        OnDeselected();
        CurrentMovemenetState = UnitsMovementState.Autopilot;
        CurrentUnitsState = UnitsState.Alive;
        path = new NavMeshPath();
        elapsed = 0.0f;
      
    }

    public void OnEnable()
    {
        SetupAgentFromConfiguration();
        SetupUnitFromConfiguration();
        UnitsManager.Instance.OnChangedGlobalOrder += UpdateListOfWaypoints;
        GetComponentInChildren<UnitsHealth>().OnUnitDeath += UseChanceToBecomeZombi;
        //GetComponentInChildren<UnitsHealth>().IsFoodAround = true;
    }

    private void LateUpdate()
    {
        if (CurrentUnitsState != UnitsState.Dead && CurrentUnitsState != UnitsState.Zombi)
        {
            MoveAutomaticallyToWayPoint();
        }

        if (animator != null)
        {
            animator.SetBool("IsRunning", Agent.velocity.magnitude > 0.01f);
        }
    }

    void UseChanceToBecomeZombi()
    {
        if (CurrentUnitsState == UnitsState.Zombi)
        {
            return; // if the unit is already a zombie or in the process of becoming one, do nothing
        }
        int chance = Mathf.RoundToInt(100 / unitScriptableObject.chanceToBecomeZombi);
        int randomValue = UnityEngine.Random.Range(0, chance);
        if (randomValue == 0)
        {
            if (CurrentUnitsState != UnitsState.Zombi && zombi.currentState == ZombiState.None)
            {
                Bubble.Instance.CreateBubble(transform.position, "I am a Zombie now!");
                audioSource.clip = zombieSound;
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            zombi.attacking_Particles.gameObject.SetActive(true);
            CurrentUnitsState = UnitsState.Zombi;
            SetOccupiedBlock();
            zombi.currentState = ZombiState.AttackBlock;
            intersectedWithUnitBlock.GetComponent<ZombiCollector>().CollectZombi(zombi);
            selectedFigur.SetActive(false);
            zombi.HandleZombiTransformation();
            Debug.Log("UseChanceToBecomeZombi");
            StartCoroutine(zombi.AttackBlock());

        }
        else
        {
            DestroyUnit();
        }

    }

    public void SetOccupiedBlock()
    {
        float dist = Mathf.Infinity;
        if (zombi.currentState == ZombiState.None) // first assignment
        {
            float newDist = Vector3.Distance(transform.position + transform.position * 0.5f, intersectedWithUnitBlock.transform.position + intersectedWithUnitBlock.transform.position * 0.5f);
            if (newDist < dist)
            {
                dist = newDist;
                zombi.targetBlockHealth = intersectedWithUnitBlock.GetComponentInParent<BlockHealth>();
                zombi.targetBlock = intersectedWithUnitBlock;
            }
        }
    }
    void CheckBlock(Collider other)
    {
        BlockPrefab block;
        if (other.GetComponentInParent<BlockPrefab>())
        {
            block = other.GetComponentInParent<BlockPrefab>();
            intersectedWithUnitBlock = block;
        }
    }
    void DestroyUnit()
    {
        UnitsManager.Instance.SetAmountOfUnits(placedObjectName, -1);
        ParticleSystem particles = Instantiate(unitScriptableObject.death_Particles, transform.position, Quaternion.identity);
        particles.gameObject.AddComponent<AudioSource>().clip = glassBreaking;
        particles.gameObject.GetComponent<AudioSource>().volume = 0.01f;
        particles.gameObject.GetComponent<AudioSource>().loop = false;
        particles.gameObject.GetComponent<AudioSource>().Play();
        particles.Play();
        Destroy(gameObject);
    }
    public void UpdateListOfWaypoints()
    {
        Debug.Log($"UpdateListOfWaypoints fired at {Time.frameCount}");

        if (CurrentUnitsState != UnitsState.Dead && CurrentUnitsState != UnitsState.Zombi)
        {
            waypointsList.localOrder.Clear();

            // Add StartPoint only if it is not already included
            if (StartPoint != null &&
                (UnitsManager.Instance.waypointsForPlacedObjects == null ||
                 !UnitsManager.Instance.waypointsForPlacedObjects[placedObjectName].Contains(StartPoint)))
            {
                waypointsList.localOrder.Add(StartPoint);
                target = StartPoint;
            }

            // Add the global waypoints in reverse order (copy before reversing!)
            if (UnitsManager.Instance.waypointsForPlacedObjects != null &&
                UnitsManager.Instance.waypointsForPlacedObjects[placedObjectName] != null)
            {
                var listCopy = new List<Transform>(UnitsManager.Instance.waypointsForPlacedObjects[placedObjectName]);
                listCopy.Reverse();
                waypointsList.localOrder.AddRange(listCopy);
            }

            waypointIndex = 0;
            SanitizeWaypoints();
        }

    }

    // this called after (re)building the list to purge nulls and clamp the index
    private void SanitizeWaypoints()
    {
        // Remove any destroyed/null entries Unity left behind
        waypointsList.localOrder.RemoveAll(t => t == null);

        // Clamp/rewind the index if it points past the end
        if (waypointIndex >= waypointsList.localOrder.Count)
            waypointIndex = Mathf.Max(0, waypointsList.localOrder.Count - 1);

        // If the list is empty, clear the agent’s path
        if (waypointsList.localOrder.Count == 0)
        {
            Agent.ResetPath();
            target = null;
            waypointIndex = 0;
            return;
        }

        // Keep the inspector target in sync with the current index
        target = waypointsList.localOrder[waypointIndex];
    }
    private void MoveAutomaticallyToWayPoint()
    {
        // --- BASIC GUARDS ---
        // Stop immediately if the unit is dead or has become a zombie
        if (CurrentUnitsState == UnitsState.Dead || CurrentUnitsState == UnitsState.Zombi)
            return;

        // Stop if the unit is not in autopilot mode (e.g. being controlled by the player)
        if (CurrentMovemenetState != UnitsMovementState.Autopilot)
            return;

        // Stop if there are no waypoints defined
        if (waypointsList.localOrder == null || waypointsList.localOrder.Count == 0)
            return;

        // Before using 'next'
        while (waypointsList.localOrder.Count > 0 && waypointsList.localOrder[waypointIndex] == null)
        {
            waypointsList.localOrder.RemoveAt(waypointIndex);
            if (waypointIndex >= waypointsList.localOrder.Count)
                waypointIndex = 0;
        }
        if (waypointsList.localOrder.Count == 0) { Agent.ResetPath(); return; }




        // --- MOVEMENT TIMER ---
        // Throttle ONLY when patrolling (2+ waypoints). With 1 waypoint, never delay.
        if (waypointsList.localOrder.Count > 1)
        {
            elapsed += Time.deltaTime;
            if (elapsed < movingToPointTimer) return;
            elapsed = 0;
        }


        // --- SELECT NEXT WAYPOINT ---
        // Get the current target waypoint by index
        var next = waypointsList.localOrder[waypointIndex];

        // If the waypoint is missing, skip to the next one
        if (next == null)
        {
            IterateWaypointIndex();
            return;
        }

        // Optional: keep track of the current target in Inspector (debug only)
        target = next;


        // --- HEALING / STALE TRIGGER SAFETY ---
        // If we were yanked away but trigger exit didn't fire, clear the stale placed object.
        if (currentPlacedObject != null)
        {
            float distFromFood = Vector3.Distance(transform.position, currentPlacedObject.transform.position);
            if (distFromFood > Agent.stoppingDistance + 2f) // clearly away from it
            {
                currentPlacedObject = null;
                //var uh = GetComponentInChildren<UnitsHealth>();
                //if (uh != null) uh.IsFoodAround = false;
            }
        }

        // Only pause for healing if we are actually close enough to the food/source
        var health = GetComponent<UnitsHealth>();
        bool atFoodAndHealing =
            currentPlacedObject != null &&
            Vector3.Distance(transform.position, currentPlacedObject.transform.position) <= Agent.stoppingDistance + 0.5f &&
            health.CurrentHealth < health.MaxHealth;

        if (atFoodAndHealing)
        {
            Debug.Log($"atFoodAndHealing");
            return; // stay here and heal
        }



        // --- PATHFINDING / RECOVERY ---
        // If we lost our path (manual drag/teleport or nav change), recompute immediately.
        if (!Agent.hasPath && !Agent.pathPending)
        {
            if (!Agent.CalculatePath(next.position, path) || path.status != NavMeshPathStatus.PathComplete)
            {
                Debug.LogWarning($"Path is not complete to {next.name}");
                IterateWaypointIndex();
                return;
            }
            Agent.SetDestination(next.position);
            // do not early-return; allow arrival check below to run this frame too
        }
        else
        {
            // Normal case: validate and set destination as usual
            if (!Agent.CalculatePath(next.position, path) || path.status != NavMeshPathStatus.PathComplete)
            {
                Debug.LogWarning($"Path is not complete to {next.name}");
                IterateWaypointIndex();
                return;
            }
            Agent.SetDestination(next.position);
        }


        // --- ARRIVAL CHECK ---
        // Use agent metrics rather than raw Vector3 distance.
        if (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance + 0.5f)
        {
            Debug.Log($"Waypoint approached: {next.name}");
            IterateWaypointIndex();

            // Immediately set the next destination (don’t wait for the next timer tick)
            if (waypointsList.localOrder != null && waypointsList.localOrder.Count > 0)
            {
               
                if (next != null)
                {
                    if (Agent.CalculatePath(next.position, path) && path.status == NavMeshPathStatus.PathComplete)
                        Agent.SetDestination(next.position);
                }
            }
        }
    }

    void IterateWaypointIndex()
    {
        if (waypointsList.localOrder != null)
        {
            if (waypointIndex < waypointsList.localOrder.Count - 1)
            {
                waypointIndex++;
            }
            else
            {
                waypointIndex = 0;
            }
        }

    }
    public void OnSelected()
    {
        selectedFigur.SetActive(true);
    }
    public void OnDeselected()
    {
        selectedFigur.SetActive(false);
    }


    public  void SetupUnitFromConfiguration()
    {
        movingToPointTimer = UnityEngine.Random.Range(unitScriptableObject.minMovingToPointTimer, unitScriptableObject.maxMovingToPointTimer);
    }
    public  void SetupAgentFromConfiguration()
    {
        Agent.acceleration = unitScriptableObject.acceleration;
        Agent.angularSpeed = unitScriptableObject.angularSpeed;
        Agent.areaMask = unitScriptableObject.areaMask;
        Agent.avoidancePriority = UnityEngine.Random.Range(unitScriptableObject.avoidancePriority / 4, unitScriptableObject.avoidancePriority); // Randomize avoidance priority for each unit
        Agent.baseOffset = unitScriptableObject.baseOffset;
        Agent.height = unitScriptableObject.height;
        Agent.obstacleAvoidanceType = unitScriptableObject.obstacleAvoidanceType;
        Agent.radius = unitScriptableObject.radius;
        Agent.speed = unitScriptableObject.speed;
        Agent.stoppingDistance = unitScriptableObject.stoppingDistance;

        Agent.autoRepath = true;       
        Agent.autoBraking = true;       // smoother arrivals
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponentInParent<PlacedObject_Done>())
        {
            if (other.gameObject.GetComponentInParent<PlacedObject_Done>().placedObjectTypeSO.placedObjId == PlacedObjTypeId)
            {
                Debug.Log($"{gameObject.name} entered trigger with {other.gameObject.name}");

                if (CurrentUnitsState != UnitsState.Dead && CurrentUnitsState != UnitsState.Zombi)
                {
                    currentPlacedObject = other.gameObject.GetComponentInParent<PlacedObject_Done>();
                    currentPlacedObject.onDestroyedPlacedObject += OnDestroyedPlacedObject;
                    GetComponentInChildren<UnitsHealth>().IsFoodAround = true;
                    StartCoroutine(GetComponentInChildren<UnitsHealth>().FillHealthGradually());
                }

            }
        }

        if (other.gameObject.tag == "Gem" && CurrentUnitsState != UnitsState.Zombi)
        {
            other.gameObject.GetComponent<Gem>().CollectGem();
        }

        CheckBlock(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInParent<PlacedObject_Done>())
        {
            Debug.Log($"{gameObject.name} exited trigger with {other.gameObject.name}");

            if (other.gameObject.GetComponentInParent<PlacedObject_Done>().placedObjectTypeSO.placedObjId == PlacedObjTypeId && GetComponentInChildren<UnitsHealth>().IsFoodAround)
            {
                if (currentPlacedObject != null)
                    currentPlacedObject.onDestroyedPlacedObject -= OnDestroyedPlacedObject;
                currentPlacedObject = null;
                GetComponentInChildren<UnitsHealth>().IsFoodAround = false;
                GetComponentInChildren<UnitsHealth>().LoseHealth();
            }
        }
    }

    void OnDestroyedPlacedObject()
    {
        currentPlacedObject = null;
        if (GetComponentInChildren<UnitsHealth>().IsFoodAround)
        {
            GetComponentInChildren<UnitsHealth>().IsFoodAround = false;
            GetComponentInChildren<UnitsHealth>().LoseHealth();
            Agent.ResetPath();
            UpdateListOfWaypoints();
        }
    }

}
