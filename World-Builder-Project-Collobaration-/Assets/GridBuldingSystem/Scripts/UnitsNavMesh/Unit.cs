using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
public enum UnitsMovementState
{
    Autopilot,
    ControlledFromPlayer
}

public enum UnitsState // to add weight
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

[Serializable]
public class BlocksList
{
   public List<BlockPrefab> blocks = new List<BlockPrefab>();
}

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
    [SerializeField] AudioClip zombieSound;
    public AudioClip glassBreaking;
    public UnitsTypeSO unitScriptableObject;
    public GameObject selectedFigur;
    public NavMeshAgent agent;
    public float agentVel;
    public Transform target;
    public Transform startPoint;
    //public Transform currentPoint;
    //List<Transform> localOrder = new List<Transform>();
    [SerializeField] WaypointsList waypointsList = new WaypointsList();
    private NavMeshPath path;
    [SerializeField] float elapsed = 0.0f;
    [SerializeField] float movingToPointTimer = 3f;
    public int placedObjTypeId;
    [SerializeField] int waypointIndex = 0;
    public UnitsMovementState currentMovemenetState;
    public UnitsState currentUnitsState;
    public bool isWaypointApproached = false;
    BlockHealth occupiedBlockHealth;
    Zombi zombi;
    [SerializeField] BlocksList intersectedWithUnitBlocks = new BlocksList();
    //List<BlockPrefab> intersectedWithUnitBlocks = new List<BlockPrefab>();
    [SerializeField] Animator animator;
    public AudioSource audioSource;
    PlacedObject_Done currentPlacedObject = null;
    private void Awake()
    {
        selectedFigur = gameObject.transform.GetChild(0).gameObject;
        agent = GetComponent<NavMeshAgent>();
        zombi = GetComponent<Zombi>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        OnDeselected();
        currentMovemenetState = UnitsMovementState.Autopilot;
        currentUnitsState = UnitsState.Alive;
        path = new NavMeshPath();
        elapsed = 0.0f;
        //MoveAutomaticallyToWayPoint();
    }

    public void OnEnable()
    {
        SetupAgentFromConfiguration();
        UnitsManager.Instance.OnChangedGlobalOrder += UpdateListOfWaypoints;
        GetComponentInChildren<UnitsHealth>().OnUnitDeath += UseChanceToBecomeZombi;
    }

    private void LateUpdate()
    {
        if (currentUnitsState != UnitsState.Dead && currentUnitsState != UnitsState.Zombi)
        {
            MoveAutomaticallyToWayPoint();
        }

        if (animator != null)
        {
            animator.SetBool("IsRunning", agent.velocity.magnitude > 0.01f);
            if (agent.velocity.magnitude < 0.01f && currentUnitsState != UnitsState.Zombi)
            {
                audioSource.volume = 0;
            }
            else
            {
                if (unitScriptableObject.unitId == 0 || unitScriptableObject.unitId == 3)
                {
                    audioSource.volume = 0.95f;
                }
                else if (currentUnitsState != UnitsState.Zombi)
                {
                    audioSource.volume = 0.25f;
                }
                else
                {
                    audioSource.volume = 0.95f;
                }



            }
        }

        agentVel = agent.velocity.magnitude;
    }

    void UseChanceToBecomeZombi()
    {
        if(currentUnitsState == UnitsState.Zombi)
        {
            return; // if the unit is already a zombie or in the process of becoming one, do nothing
        }
        int chance = Mathf.RoundToInt(100 / unitScriptableObject.chanceToBecomeZombi);
        int randomValue = UnityEngine.Random.Range(0, chance);
        if (randomValue == 0)
        {
            if (currentUnitsState != UnitsState.Zombi && zombi.currentState == ZombiState.None)
            {
                Bubble.Instance.CreateBubble(transform.position, "I am a Zombie now!");
                audioSource.clip = zombieSound;
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            zombi.attacking_Particles.gameObject.SetActive(true);
            currentUnitsState = UnitsState.Zombi;
            SetOccupiedBlock();
            zombi.currentState = ZombiState.AttackBlock;
            zombi.HandleZombiTransformation();
            zombi.HandleZombiMovement();
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
            foreach (var block in intersectedWithUnitBlocks.blocks)
            {
                float newDist = Vector3.Distance(transform.position + transform.position * 0.5f, block.transform.position + block.transform.position * 0.5f);
                if (newDist < dist)
                {
                    dist = newDist;
                    zombi.targetBlockHealth = block.GetComponentInParent<BlockHealth>();
                    zombi.targetBlock = block;
                }
            }
        }
    }
    void CheckBlock(Collider other)
    {
        BlockPrefab block;
        if (other.GetComponentInParent<BlockPrefab>())
        {
            block = other.GetComponentInParent<BlockPrefab>();
            if (!intersectedWithUnitBlocks.blocks.Contains(block))
            {
                intersectedWithUnitBlocks.blocks.Add(block);

            }
        }
    }
    void DestroyUnit()
    {
        UnitsManager.Instance.SetAmountOfUnits(unitScriptableObject.unitId, -1);
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
        if (currentUnitsState != UnitsState.Dead && currentUnitsState != UnitsState.Zombi)
        {
            waypointsList.localOrder.Clear();
            //if (startPoint == null)
            //{
            //    startPoint = UnitsManager.Instance.startPoint;
            //}
            if (target == null && startPoint != null) // it means the unit was just created
            {
                waypointsList.localOrder.Add(startPoint);
                //currentPoint = startPoint;
                target = startPoint; 
            }
            //else
            //{
            //    currentPoint = target;
            //    waypointsList.localOrder.Add(currentPoint);
            //}
            if (UnitsManager.Instance.waypoints != null)
            {
                if (UnitsManager.Instance.waypoints[placedObjTypeId] != null)
                {
                    List<Transform> reversedList = UnitsManager.Instance.waypoints[placedObjTypeId];
                    reversedList.Reverse();
                    waypointsList.localOrder.AddRange(reversedList);

                }
            }
            waypointIndex = 0; // to reset the path and start from zero point again

        }

    }

    void MoveAutomaticallyToWayPoint() //ELAPSED 
    {
        if (currentUnitsState != UnitsState.Dead && currentUnitsState != UnitsState.Zombi)
        {
            if (currentMovemenetState == UnitsMovementState.Autopilot)
            {
                // Update the way to the goal every amount of sec in movingToPointTimer.
                elapsed += Time.deltaTime;
                if(waypointsList.localOrder == null || waypointsList.localOrder.Count == 0)
                {
                    return;
                }
                target = waypointsList.localOrder[waypointIndex];
                if (target != null)
                {
                    if (elapsed > movingToPointTimer && (GetComponent<UnitsHealth>().curretValue >= GetComponent<UnitsHealth>().maxValue || currentPlacedObject == null))
                    {
                        elapsed = 0;
                        if (agent.SetDestination(target.transform.position))
                        {
                            if (Vector3.Distance(transform.position, target.transform.position) < 3f)
                            {
                                Debug.Log("Waypoint approached: " + target.name);
                                IterateWaypointIndex();
                                //MoveAutomaticallyToWayPoint();
                            }
                            //else
                            //{
                            //    IterateWaypointIndex();
                            //}
                        }
                        else
                        {
                            Debug.Log("Failed to set destination to: " + target.name);
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

    public virtual void SetupAgentFromConfiguration()
    {
        agent.acceleration = unitScriptableObject.acceleration;
        agent.angularSpeed = unitScriptableObject.angularSpeed;
        agent.areaMask = unitScriptableObject.areaMask;
        agent.avoidancePriority = unitScriptableObject.avoidancePriority;
        agent.baseOffset = unitScriptableObject.baseOffset;
        agent.height = unitScriptableObject.height;
        agent.obstacleAvoidanceType = unitScriptableObject.obstacleAvoidanceType;
        agent.radius = unitScriptableObject.radius;
        agent.speed = unitScriptableObject.speed;
        agent.stoppingDistance = unitScriptableObject.stoppingDistance;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponentInParent<PlacedObject_Done>())
        {
            if (other.gameObject.GetComponentInParent<PlacedObject_Done>().placedObjectTypeSO.placedObjId == placedObjTypeId)
            {
                if (currentUnitsState != UnitsState.Dead && currentUnitsState != UnitsState.Zombi && !GetComponentInChildren<UnitsHealth>().isFoodAround)
                {
                    currentPlacedObject = other.gameObject.GetComponentInParent<PlacedObject_Done>();
                    currentPlacedObject.onDestroyedPlacedObject += OnDestroyedPlacedObject;
                    GetComponentInChildren<UnitsHealth>().isFoodAround = true;
                    //GetComponentInChildren<UnitsHealth>().isHealthLosing = false;
                    StartCoroutine(GetComponentInChildren<UnitsHealth>().FillHealthGradually());
                }

            }
        }

        if (other.gameObject.tag == "Gem" && currentUnitsState != UnitsState.Zombi)
        {
            other.gameObject.GetComponent<Gem>().CollectGem();
        }

        CheckBlock(other);
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.GetComponentInParent<PlacedObject_Done>())
    //    {
    //        if (other.gameObject.GetComponentInParent<PlacedObject_Done>().placedObjectTypeSO.placedObjId == placedObjTypeId)
    //        {
    //            if (currentUnitsState != UnitsState.Dead && currentUnitsState != UnitsState.Zombi && !GetComponentInChildren<UnitsHealth>().isFoodAround)
    //            {
    //                Debug.Log("There is a food around");
    //                GetComponentInChildren<UnitsHealth>().isFoodAround = true;
    //                StartCoroutine(GetComponentInChildren<UnitsHealth>().FillHealthGradually());
    //                CheckIntersectedBlock(other);
    //            }
    //        }

    //        else if(currentUnitsState != UnitsState.Dead && currentUnitsState != UnitsState.Zombi && GetComponentInChildren<UnitsHealth>().isFoodAround)
    //        {
    //            Debug.Log("There is no food around");
    //            GetComponentInChildren<UnitsHealth>().isFoodAround = false;
    //            GetComponentInChildren<UnitsHealth>().LoseHealth();


    //        }
    //    }
    //}


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInParent<PlacedObject_Done>())
        {
            if (other.gameObject.GetComponentInParent<PlacedObject_Done>().placedObjectTypeSO.placedObjId == placedObjTypeId && GetComponentInChildren<UnitsHealth>().isFoodAround)
            {
                currentPlacedObject.onDestroyedPlacedObject -= OnDestroyedPlacedObject;
                currentPlacedObject = null;
                GetComponentInChildren<UnitsHealth>().isFoodAround = false;
                //StopCoroutine(GetComponentInChildren<UnitsHealth>().FillHealthGradually());
                GetComponentInChildren<UnitsHealth>().LoseHealth();


            }
        }
    }

    void OnDestroyedPlacedObject()
    {
        currentPlacedObject = null;
        if (GetComponentInChildren<UnitsHealth>().isFoodAround)
        {
            GetComponentInChildren<UnitsHealth>().isFoodAround = false;
            GetComponentInChildren<UnitsHealth>().LoseHealth();
            agent.ResetPath();
            UpdateListOfWaypoints();
            //agent.SetDestination(target.transform.position);
            //MoveAutomaticallyToWayPoint();
        }
    }

}
