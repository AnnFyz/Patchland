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
    private NavMeshPath path;
    private float elapsed = 0.0f;
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
        UnitsManager.Instance.TimeToGo += GoToWayPoint;
        SetupAgentFromConfiguration();
    }
    void GoToWayPoint()
    {
        // Update the way to the goal every second.
        elapsed += Time.deltaTime;
        if(UnitsManager.Instance.waypoints[1] != null)
        {
            target = UnitsManager.Instance.waypoints[0];
        }

        foreach (var waypoint in UnitsManager.Instance.waypoints)
        { }
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
                        Debug.Log("Unable to approach destination"); // to continue the loop at this point
                    }
                }
                for (int i = 0; i < path.corners.Length - 1; i++)
                    Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.blue);
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
