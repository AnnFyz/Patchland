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
    public Transform currentPoint;
    List<Transform> localOrder = new List<Transform>();
    private NavMeshPath path;
    private float elapsed = 0.0f;
    public int placedObjTypeId;
    int waypointIndex = 0;
    public UnitsMovementState currentMovemenetState;
    public UnitsState currentUnitsState;
    public GameObject Pointer;
    public bool isWaypointApproached = false;
    BlockHealth occupiedBlockHealth;
    Zombi zombi;
    List<BlockPrefab> intersectedWithUnitBlocks = new List<BlockPrefab>();
    [SerializeField] Animator animator;
    public AudioSource audioSource;
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
        Pointer = GameObject.Find("Pointer");
        MoveAutomaticallyToWayPoint();
    }

    public void OnEnable()
    {
        SetupAgentFromConfiguration();
        UnitsManager.Instance.OnChangedGlobalOrder += UpdateListOfWaypoints;
        GetComponentInChildren<UnitsHealth>().OnUnitDeath += UseChangeToBecomeZombi;
    }

    private void LateUpdate()
    {
        //Pointer.transform.position = target.position;
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

    void UseChangeToBecomeZombi()
    {
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
            foreach (var block in intersectedWithUnitBlocks)
            {
                float newDist = Vector3.Distance(transform.position + transform.position * 0.5f, block.transform.position + block.transform.position * 0.5f);
                if (newDist < dist)
                {
                    dist = newDist;
                    zombi.occupiedBlockHealth = block.GetComponentInParent<BlockHealth>();
                    zombi.occupiedBlock = block;
                }
            }
        }
    }
    void CheckIntersectedBlock(Collider other)
    {
        BlockPrefab block;
        if (other.GetComponentInParent<BlockPrefab>())
        {
            block = other.GetComponentInParent<BlockPrefab>();
            if (!intersectedWithUnitBlocks.Contains(block))
            {
                intersectedWithUnitBlocks.Add(block);

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
            localOrder.Clear();
            if (target == null) // it means the unit was just created
            {
                localOrder.Add(startPoint);
                currentPoint = startPoint;
                target = startPoint;
            }
            else
            {
                currentPoint = target;
                localOrder.Add(currentPoint);
            }
            if(UnitsManager.Instance.waypoints != null)
            {
               if( UnitsManager.Instance.waypoints[placedObjTypeId] != null)
                {
                    List<Transform> reversedList = UnitsManager.Instance.waypoints[placedObjTypeId];
                    reversedList.Reverse();
                    localOrder.AddRange(reversedList);
                   
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
                // Update the way to the goal every second.
                elapsed += Time.deltaTime;
                IterateWaypointIndex();
                target = localOrder[waypointIndex];
                if (target != null)
                {
                    if (elapsed > 1.5f)
                    {
                        elapsed -= 1.5f;
                        if (agent.SetDestination(target.transform.position))
                        {
                            if (Vector3.Distance(transform.position, target.transform.position) < 1f)
                            {
                                MoveAutomaticallyToWayPoint();
                            }
                            else
                            {
                                IterateWaypointIndex();
                            }
                        }
                        else
                        {
                            IterateWaypointIndex();
                        }
                    }
                }

            }
        }
    }
    void IterateWaypointIndex()
    {
        if(localOrder != null)
        {
            if (waypointIndex < localOrder.Count)
            {
                waypointIndex++;
            }
            if (waypointIndex == localOrder.Count)
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
                if (currentUnitsState != UnitsState.Dead && currentUnitsState != UnitsState.Zombi)
                {
                    GetComponentInChildren<UnitsHealth>().FillHealth(50);
                }

            }
        }

        if (other.gameObject.tag == "Gem" && currentUnitsState != UnitsState.Zombi)
        {
            other.gameObject.GetComponent<Gem>().CollectGem();
        }
        CheckIntersectedBlock(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponentInParent<PlacedObject_Done>() && other.gameObject.GetComponentInParent<PlacedObject_Done>() != null)
        {
            if (other.gameObject.GetComponentInParent<PlacedObject_Done>().placedObjectTypeSO.placedObjId == placedObjTypeId)
            {
                if (currentUnitsState != UnitsState.Dead && currentUnitsState != UnitsState.Zombi)
                {
                    GetComponentInChildren<UnitsHealth>().isFoodAround = true;
                    StartCoroutine(GetComponentInChildren<UnitsHealth>().FillHealthGradually());
                }
            }
        }
        else
        {

            GetComponentInChildren<UnitsHealth>().isFoodAround = false;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInParent<PlacedObject_Done>())
        {
            if (other.gameObject.GetComponentInParent<PlacedObject_Done>().placedObjectTypeSO.placedObjId == placedObjTypeId)
            {
                StopCoroutine(GetComponentInChildren<UnitsHealth>().FillHealthGradually());
                GetComponentInChildren<UnitsHealth>().isFoodAround = false;
            }
        }
    }

}
