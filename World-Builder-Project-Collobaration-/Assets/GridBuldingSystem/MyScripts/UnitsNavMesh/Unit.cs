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
    public UnitsTypeSO unitScriptableObjects;
    public GameObject selectedFigur;
    public NavMeshAgent agent;
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
    private void Awake()
    {
        selectedFigur = gameObject.transform.GetChild(0).gameObject;
        agent = GetComponent<NavMeshAgent>();
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

    private void FixedUpdate()
    {
        //Pointer.transform.position = target.position;
        if(currentUnitsState != UnitsState.Dead && currentUnitsState != UnitsState.Zombi)
        {
            MoveAutomaticallyToWayPoint();
        }
    }

    void UseChangeToBecomeZombi()
    {
        int chance = Mathf.RoundToInt(100 / unitScriptableObjects.chanceToBecomeZombi);
        int randomValue = UnityEngine.Random.Range(0, chance);
        if (randomValue == 0)
        {
            currentUnitsState = UnitsState.Zombi;
            Zombi zombi = GetComponent<Zombi>();
            zombi.currentState = ZombiState.AttackBlock;
            zombi.HandleZombiTransformation();
            zombi.HandleZombiMovement();
            StartCoroutine(zombi.AttackBlock());
           
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void DestroyUnit()
    {
        Destroy(gameObject);
    }
    public void UpdateListOfWaypoints()
    {
        if(currentUnitsState != UnitsState.Dead && currentUnitsState != UnitsState.Zombi)
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
            List<Transform> reversedList = UnitsManager.Instance.waypoints[placedObjTypeId];
            reversedList.Reverse();
            localOrder.AddRange(reversedList);
            waypointIndex = 0; // to reset the path and start from zero point again
        }
   
    }

    void MoveAutomaticallyToWayPoint()
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
                    if (elapsed > 2.0f)
                    {
                        elapsed -= 2.0f;
                        agent.SetDestination(target.transform.position);
                        if (Vector3.Distance(transform.position, target.transform.position) < 1f)
                        {
                            MoveAutomaticallyToWayPoint();
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
        waypointIndex++;
        if (waypointIndex == localOrder.Count)
        {
            waypointIndex = 0;
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
        agent.acceleration = unitScriptableObjects.acceleration;
        agent.angularSpeed = unitScriptableObjects.angularSpeed;
        agent.areaMask = unitScriptableObjects.areaMask;
        agent.avoidancePriority = unitScriptableObjects.avoidancePriority;
        agent.baseOffset = unitScriptableObjects.baseOffset;
        agent.height = unitScriptableObjects.height;
        agent.obstacleAvoidanceType = unitScriptableObjects.obstacleAvoidanceType;
        agent.radius = unitScriptableObjects.radius;
        agent.speed = unitScriptableObjects.speed;
        agent.stoppingDistance = unitScriptableObjects.stoppingDistance;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponentInParent<PlacedObject_Done>())
        {
            if (other.gameObject.GetComponentInParent<PlacedObject_Done>().placedObjectTypeSO.placedObjId == placedObjTypeId)
            {
                if(currentUnitsState != UnitsState.Dead && currentUnitsState != UnitsState.Zombi)
                {
                    GetComponentInChildren<UnitsHealth>().FillHealth(50);
                    GetComponentInChildren<UnitsHealth>().isFoodAround = true;
                    //if health.amout = full
                    // else stay until health.amout = full
                }

            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponentInParent<PlacedObject_Done>())
        {
            if (other.gameObject.GetComponentInParent<PlacedObject_Done>().placedObjectTypeSO.placedObjId == placedObjTypeId)
            {
                if (currentUnitsState != UnitsState.Dead && currentUnitsState != UnitsState.Zombi)
                {
                    StartCoroutine(GetComponentInChildren<UnitsHealth>().FillHealthGradually());
                }
            }
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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Gem")
        {
            collision.gameObject.GetComponent<Gem>().CollectGem();
        }
    }

}
