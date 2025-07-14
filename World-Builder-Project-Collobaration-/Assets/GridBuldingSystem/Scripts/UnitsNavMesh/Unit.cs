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
//The unit can interact with placed objects, has chance to become a zombie, if it dies, and has various properties defined in a scriptable object. 
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
    private Transform target; // the target to which the unit is moving
    int waypointIndex = 0;
    private WaypointsList waypointsList = new WaypointsList();
    float movingToPointTimer;
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
            zombi.HandleZombiMovement();
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
        if (CurrentUnitsState != UnitsState.Dead && CurrentUnitsState != UnitsState.Zombi)
        {
            waypointsList.localOrder.Clear();
            if (target == null && StartPoint != null) // it means the unit was just created
            {
                waypointsList.localOrder.Add(StartPoint);
                target = StartPoint;
            }
            if (UnitsManager.Instance.waypointsForPlacedObjects != null)
            {
                if (UnitsManager.Instance.waypointsForPlacedObjects[placedObjectName] != null)
                {
                    List<Transform> reversedList = UnitsManager.Instance.waypointsForPlacedObjects[placedObjectName];
                    reversedList.Reverse();
                    waypointsList.localOrder.AddRange(reversedList);

                }
            }

            waypointIndex = 0; // to reset the path and start from zero point again

        }

    }

    void MoveAutomaticallyToWayPoint() //ELAPSED 
    {
        if (CurrentUnitsState != UnitsState.Dead && CurrentUnitsState != UnitsState.Zombi)
        {
            if (CurrentMovemenetState == UnitsMovementState.Autopilot)
            {
                // Update the way to the goal every amount of sec in movingToPointTimer.
                elapsed += Time.deltaTime;
                if (waypointsList.localOrder == null || waypointsList.localOrder.Count == 0)
                {
                    return;
                }
                target = waypointsList.localOrder[waypointIndex];
                if (target != null)
                {
                    if (elapsed > movingToPointTimer && (GetComponent<UnitsHealth>().CurretValue >= GetComponent<UnitsHealth>().MaxValue || currentPlacedObject == null))
                    {
                        elapsed = 0;
                        Agent.CalculatePath(target.transform.position, path);
                        if (path.status == NavMeshPathStatus.PathComplete)
                        {
                            if (Agent.SetDestination(target.transform.position))
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
                                IterateWaypointIndex();
                            }
                        }
                        else
                        {
                            Debug.Log("Path is not complete to: " + target.name);
                            IterateWaypointIndex();
                        }
                    }

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
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponentInParent<PlacedObject_Done>())
        {
            if (other.gameObject.GetComponentInParent<PlacedObject_Done>().placedObjectTypeSO.placedObjId == PlacedObjTypeId)
            {
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

            if (other.gameObject.GetComponentInParent<PlacedObject_Done>().placedObjectTypeSO.placedObjId == PlacedObjTypeId && GetComponentInChildren<UnitsHealth>().IsFoodAround)
            {
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
