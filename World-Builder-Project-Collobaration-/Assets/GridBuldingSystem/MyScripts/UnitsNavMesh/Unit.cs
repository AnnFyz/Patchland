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

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
    [SerializeField] UnitsTypeSO unitScriptableObjects;
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
    [SerializeField] GameObject targetPoint; 
    private void Awake()
    {
        selectedFigur = gameObject.transform.GetChild(0).gameObject;
        agent = GetComponent<NavMeshAgent>();
    }


    void Start()
    {
        OnDeselected();
        currentMovemenetState = UnitsMovementState.Autopilot;
        path = new NavMeshPath();
        elapsed = 0.0f;
    }
    public virtual void OnEnable()
    {
        UnitsManager.Instance.OnChangedGlobalOrder += UpdateListOfWaypoints;
        UnitsManager.Instance.TimeToMoveAutomatically += MoveAutomaticallyToWayPoint;
        SetupAgentFromConfiguration();
    }

    private void Update()
    {
        targetPoint.transform.position = target.transform.position;
    }
    public void UpdateListOfWaypoints()
    {
        Debug.Log("Order was updated");
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
            Debug.Log("currentPoint was updated");
        }
        List<Transform> reversedList = UnitsManager.Instance.waypoints[placedObjTypeId];
        reversedList.Reverse();
        localOrder.AddRange(reversedList);
        waypointIndex = 0; // to reset the path and start from zero point again
        foreach (var item in localOrder)
        {
            Debug.Log("Waypoints " + item);
        }

    }
    void MoveAutomaticallyToWayPoint()
    {
        if (currentMovemenetState == UnitsMovementState.Autopilot)
        {
            // Update the way to the goal every second.
            elapsed += Time.deltaTime;
            IterateWaypointIndex();
            target = localOrder[waypointIndex];
            if (target != null)
            {
                if (elapsed > 1.0f)
                {
                    elapsed -= 1.0f;
                    agent.SetDestination(target.transform.position);
                    if (Vector3.Distance(transform.position, target.transform.position) < 10f)
                    {
                        IterateWaypointIndex();
                        Debug.Log("destination is approached");
                    }
                    //if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
                    //{
                    //    agent.SetDestination(target.transform.position);
                    //}
                    else
                    {

                        IterateWaypointIndex();
                        Debug.Log("Keeps moving towards the waypoint");
                    }
                }
                for (int i = 0; i < path.corners.Length - 1; i++)
                    Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.blue);
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

        //movement.updateRate = unitScriptableObjects.AIUpdateInterval;
        //movement.radius = unitScriptableObjects.triggerRadius;
        //startHealth = unitScriptableObjects.health;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponentInParent<PlacedObject_Done>())
        {
            if (other.gameObject.GetComponentInParent<PlacedObject_Done>().placedObjectTypeSO.placedObjId == placedObjTypeId)
            {
                GetComponentInChildren<UnitsHealth>().FillHealth(50);
                GetComponentInChildren<UnitsHealth>().isFoodAround = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponentInParent<PlacedObject_Done>())
        {
            if (other.gameObject.GetComponentInParent<PlacedObject_Done>().placedObjectTypeSO.placedObjId == placedObjTypeId)
            {
                Debug.Log("COROUTINE");
                StartCoroutine(GetComponentInChildren<UnitsHealth>().FillHealthGradually());
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


}
