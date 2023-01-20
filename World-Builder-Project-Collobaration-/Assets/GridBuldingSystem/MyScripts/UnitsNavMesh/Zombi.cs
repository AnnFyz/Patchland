using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZombiState
{
   None,
   AttackUnit,
   AttackBlock,
   DeadZombi
}
public class Zombi : MonoBehaviour
{
    public ZombiState currentState;
    Unit unit;
    [SerializeField] BlockHealth occupiedBlock;
    [SerializeField] float step = 3f;
    List<Transform> generatedWaypoints = new List<Transform>();
    private int waypointIndex = 0;
    private float elapsed = 0.0f;
    public Transform target;
    public UnityEngine.AI.NavMeshAgent agent;
    private void Awake()
    {
       unit = GetComponent<Unit>();
       agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }
    private void Start()
    {
        currentState = ZombiState.None;
    }
    public void HandleZombiTransformation()
    {
        Debug.Log("I AM A ZOMBI NOW!");
    }

    public void FillTheListOfWaypints() 
    {
        //Set Destination to generated waypoint in circle
        float angleStep = 360 / step;
        for (int i = 1; i < step+1; i++)
        {
            GameObject generatedWaypoint = new GameObject();
            generatedWaypoint.transform.RotateAround(occupiedBlock.gameObject.transform.position,Vector3.up ,angleStep * i);
            Vector3 dir = (generatedWaypoint.transform.position - occupiedBlock.gameObject.transform.position).normalized;
            generatedWaypoint.transform.position = occupiedBlock.gameObject.transform.position + dir * 5;
            generatedWaypoints.Add(generatedWaypoint.transform);
            //Debug.DrawLine(occupiedBlock.gameObject.transform.position, occupiedBlock.gameObject.transform.position + dir * 10, Color.red, Mathf.Infinity);
        }
    }
    private void Update()
    {
        HandleZombiMovement();
    }
    public void HandleZombiMovement()
    {
        if (currentState == ZombiState.AttackBlock)
        {
            // Update the way to the goal every second.
            elapsed += Time.deltaTime;
            IterateWaypointIndex();
            target = generatedWaypoints[waypointIndex];
        if (target != null)
            {
                if (elapsed > 0.2f)
                {
                    elapsed -= 0.2f;
                    unit.agent.SetDestination( new Vector3 (target.transform.position.x, transform.position.y, target.transform.position.z));
                    if (Vector3.Distance(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z), target.transform.position) < 10f)
                    {
                        Debug.Log("MOVE TO NEXT POINT");
                        HandleZombiMovement();
                    }
                    else
                    {
                        Debug.Log("Iterate");
                        IterateWaypointIndex();
                    }
                }
            }

        }
    }

    void IterateWaypointIndex()
    {
        waypointIndex++;
        if (waypointIndex == generatedWaypoints.Count)
        {
            waypointIndex = 0;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<BlockHealth>())
        {
            occupiedBlock = other.GetComponentInParent<BlockHealth>();
        }
    }

}
