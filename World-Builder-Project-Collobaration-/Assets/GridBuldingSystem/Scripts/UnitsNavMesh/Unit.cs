using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static PlacedObjectTypeSO;


/// <summary>Different movement states of a unit.</summary>
public enum UnitsMovementState
{
    Autopilot,
    ControlledFromPlayer
}

/// <summary>Different life states of a unit.</summary>
public enum UnitsState
{
    Alive,
    Hungry,
    Attacked,
    Dead,
    Zombi
}

/// <summary>Container for waypoints a unit will follow.</summary>
[Serializable]
public class WaypointsList
{
    public List<Transform> localOrder = new List<Transform>();
}

/// <summary>
/// This class represents a unit in the game, which can be controlled by the player or move automatically.
/// The unit can interact with placed objects and has chance to become a zombie, if it dies
/// </summary>


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class Unit : MonoBehaviour
{
    [Header("📜 Unit Scriptable Object")]
    [SerializeField] UnitsTypeSO unitScriptableObject;
    public UnitsTypeSO UnitScriptableObject => unitScriptableObject;

    [Header("🔊 Unit Sounds")] //TO DO Sound logic
    [SerializeField] private AudioClip zombieSound;
    public AudioClip ZombieSound => zombieSound;
    [SerializeField] private AudioClip glassBreaking;
    public AudioClip GlassBreaking => glassBreaking;
    AudioSource audioSource;

    [Header("🎭 Unit Animation")]
    [SerializeField] private Animator animator; // to handle the animations of the unit

    private GameObject selectedFigur; // to show that the unit is selected

    [Header("🚶 Unit Movement")]
    private NavMeshAgent Agent { get; set; }
    private NavMeshPath path;
    public Transform StartPoint { get; set; }
    [SerializeField] private Transform target; // the target to which the unit is moving
    [SerializeField] private int waypointIndex = 0;
    [SerializeField] private WaypointsList waypointsList = new WaypointsList();
    [SerializeField] private float movingToPointTimer;
    private float elapsed = 0.0f;

    [Header("🏗️ Related Placed Object")]
    public PlacedObjectName placedObjectName; // to get the type of the placed object, so that we can get the waypoints for it
    public PlacedObject_Done currentPlacedObject; // to handle the placed object that the unit is currently interacting with
    public int PlacedObjTypeId { get; set; }
    public UnitsMovementState CurrentMovementState { get; set; } // to track the state of the unit movement (autopilot, controlled by player)
    public UnitsState CurrentUnitsState { get; set; } // to track the state of the unit (alive, dead, zombi, etc.)


    // Zombi related
    private bool isUnitDestroyed = false; // to track if the unit is destroyed
    private Zombi zombi; // to handle the zombi state of the unit
    public BlockPrefab TargetBlock { get; private set; } // to handle the block that the unit is targeting

    private void Awake()
    {
        selectedFigur = transform.GetChild(0).gameObject;
        Agent = GetComponent<NavMeshAgent>();
        zombi = GetComponent<Zombi>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        OnDeselected();
        CurrentMovementState = UnitsMovementState.Autopilot;
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
    }

    private void OnDisable()
    {
        UnitsManager.Instance.OnChangedGlobalOrder -= UpdateListOfWaypoints;
        var health = GetComponentInChildren<UnitsHealth>();
        if (health != null)
            health.OnUnitDeath -= UseChanceToBecomeZombi;
    }


    private void LateUpdate()
    {
        if (CurrentUnitsState != UnitsState.Dead && CurrentUnitsState != UnitsState.Zombi)
            MoveAutomaticallyToWayPoint();

        if (animator != null)
            animator?.SetBool("IsRunning", Agent.velocity.magnitude > 0.0001f);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource.isPlaying) return;
        audioSource.clip = clip;
        audioSource.Play();
    }

    void UseChanceToBecomeZombi()
    {
        if (CurrentUnitsState == UnitsState.Zombi)
            return; // if the unit is already a zombie or in the process of becoming one, do nothing

        // remove one unit from the total count for this placed object type
        UnitsManager.Instance.SetAmountOfUnits(placedObjectName, -1);

        int chance = Mathf.RoundToInt(100 / unitScriptableObject.chanceToBecomeZombi);
        int randomValue = UnityEngine.Random.Range(0, chance);

        if (randomValue == 0)
        {
            if (zombi.currentState == ZombiState.None)
            {
                Bubble.Instance.CreateBubble(transform.position, "I am a Zombie now!");
                PlaySound(zombieSound);
            }
            CurrentUnitsState = UnitsState.Zombi;
            TargetBlock.GetComponent<ZombiCollector>().CollectZombi(zombi);
            selectedFigur.SetActive(false);
            zombi.SetInitialTargetBlock(TargetBlock);
            zombi.HandleZombiTransformation();
            StartCoroutine(zombi.AttackBlock());

        }
        else
        {
            DestroyUnit();
        }

    }
    void CheckBlock(Collider other)
    {
        BlockPrefab block = other.GetComponentInParent<BlockPrefab>();
        if (block != null)
            TargetBlock = block;

    }
    void DestroyUnit()
    {
        if (isUnitDestroyed) { return; }
        isUnitDestroyed = true;
        // Play glass breaking particles and sound
        DeathParticles deathParticles = DeathParticles.Create(transform.position, TargetBlock.transform, unitScriptableObject.death_Particles_Prefab, Quaternion.identity);
        deathParticles.Play(glassBreaking, 0.01f);
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
        if (CurrentUnitsState == UnitsState.Dead || CurrentUnitsState == UnitsState.Zombi) return;

        // Stop if the unit is not in autopilot mode (e.g. being controlled by the player)
        if (CurrentMovementState != UnitsMovementState.Autopilot) return;

        // Stop if there are no waypoints defined
        if (waypointsList.localOrder == null || waypointsList.localOrder.Count == 0) return;

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
        if (waypointsList.localOrder == null || waypointsList.localOrder.Count == 0) return;
        waypointIndex = (waypointIndex + 1) % waypointsList.localOrder.Count;

    }
    public void OnSelected() => selectedFigur.SetActive(true);
    public void OnDeselected() => selectedFigur.SetActive(false);


    public void SetupUnitFromConfiguration()
    {
        float min = unitScriptableObject.minMovingToPointTimer;
        float max = unitScriptableObject.maxMovingToPointTimer;
        float t = UnityEngine.Random.value;         // uniform 0–1
        t = Mathf.Abs(Mathf.Pow(t * 2f - 1f, 3f));  // cubic power makes middle very unlikely
        movingToPointTimer = Mathf.Lerp(
            min,
            max,
            t
        );
    }
    public void SetupAgentFromConfiguration()
    {
        Agent.acceleration = unitScriptableObject.acceleration;
        Agent.angularSpeed = unitScriptableObject.angularSpeed;
        Agent.areaMask = unitScriptableObject.areaMask;
        Agent.avoidancePriority = UnityEngine.Random.Range(unitScriptableObject.avoidancePriority / 5, unitScriptableObject.avoidancePriority); // Randomize avoidance priority for each unit
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
        CheckBlock(other);

        if (CurrentUnitsState == UnitsState.Dead || CurrentUnitsState == UnitsState.Zombi)
            return;

        // Get the placed object once
        var pod = other.GetComponentInParent<PlacedObject_Done>();
        if (pod == null)
            return; // no placed object in that hierarchy

        var so = pod.placedObjectTypeSO;
        if (so == null)
        {
            Debug.LogWarning($"PlacedObject_Done on {pod.name} has no ScriptableObject assigned.");
            return;
        }

        if (so.placedObjId != PlacedObjTypeId)
            return;

        currentPlacedObject = pod;
        currentPlacedObject.onDestroyedPlacedObject += OnDestroyedPlacedObject;
        GetComponentInChildren<UnitsHealth>().IsFoodAround = true;
        StartCoroutine(GetComponentInChildren<UnitsHealth>().FillHealthGradually());


        if (other.gameObject.tag == "Gem")
        {
            other.gameObject.GetComponent<Gem>().CollectGem();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (CurrentUnitsState == UnitsState.Dead || CurrentUnitsState == UnitsState.Zombi)
            return;


        // Get the placed object once
        var pod = other.GetComponentInParent<PlacedObject_Done>();
        if (pod == null)
            return; // no placed object in that hierarchy

        var so = pod.placedObjectTypeSO;
        if (so == null)
        {
            Debug.LogWarning($"PlacedObject_Done on {pod.name} has no ScriptableObject assigned.");
            return;
        }

        if (so.placedObjId == PlacedObjTypeId && GetComponentInChildren<UnitsHealth>()?.IsFoodAround == true)
        {
            if (currentPlacedObject != null)
                currentPlacedObject.onDestroyedPlacedObject -= OnDestroyedPlacedObject;
            currentPlacedObject = null;
            GetComponentInChildren<UnitsHealth>().IsFoodAround = false;
            GetComponentInChildren<UnitsHealth>().LoseHealth();
            Agent.ResetPath();
            UpdateListOfWaypoints();
        }

    }

    void OnDestroyedPlacedObject()
    {
        currentPlacedObject = null;
        target = null;
        if (GetComponentInChildren<UnitsHealth>().IsFoodAround)
        {
            GetComponentInChildren<UnitsHealth>().IsFoodAround = false;
            GetComponentInChildren<UnitsHealth>().LoseHealth();
            Agent.ResetPath();
            UpdateListOfWaypoints();
        }
    }

}
