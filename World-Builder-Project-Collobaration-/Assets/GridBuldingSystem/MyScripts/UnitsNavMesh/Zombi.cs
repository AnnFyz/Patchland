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
            target = occupiedBlock.generatedWaypoints[waypointIndex];
        if (target != null)
            {
                if (elapsed > 0.2f)
                {
                    elapsed -= 0.2f;
                    agent.SetDestination( new Vector3 (target.transform.position.x, transform.position.y, target.transform.position.z));
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
        if (waypointIndex == occupiedBlock.generatedWaypoints.Length)
        {
            waypointIndex = 0;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(occupiedBlock == null)
        {
            if (other.GetComponentInParent<BlockHealth>())
            {
                occupiedBlock = other.GetComponentInParent<BlockHealth>();
            }
        }  
    }

}
