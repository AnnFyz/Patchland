using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



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

    private void Awake()
    {
        selectedFigur = gameObject.transform.GetChild(0).gameObject;
        agent = GetComponent<NavMeshAgent>();
    }



    void Start()
    {
        OnDeselected();
        path = new NavMeshPath();
        elapsed = 0.0f;
    }
    public virtual void OnEnable()
    {
        UnitsManager.Instance.OnChangedGlobalOrder += UpdateListOfWaypoints;
        UnitsManager.Instance.TimeToGo += GoToWayPoint;
        SetupAgentFromConfiguration();
    }
    public void UpdateListOfWaypoints()
    {
        Debug.Log("Order was updated");
        localOrder.Clear();
        if (target == null) // it means the unit was just created
        {
            localOrder.Add(startPoint);
            target = startPoint;
        }
        else
        {
            localOrder.Add(currentPoint);
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
    void GoToWayPoint()
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
                if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
                {
                    agent.SetDestination(target.transform.position);
                }
                else
                {
                    IterateWaypointIndex(); // to continue the loop at this point
                    //GoToWayPoint();
                    Debug.Log("Unable to approach destination"); 
                }
            }
            for (int i = 0; i < path.corners.Length - 1; i++)
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.blue);
        }


    }
    void IterateWaypointIndex()
    {
        waypointIndex++;
        if(waypointIndex == localOrder.Count)
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

}
