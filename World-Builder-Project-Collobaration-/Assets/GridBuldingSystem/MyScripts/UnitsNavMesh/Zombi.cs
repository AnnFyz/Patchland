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
    float H_1; 
    float S_1; 
    float V_1;

    float H_2; 
    float S_2;
    float V_2;
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
                    agent.SetDestination(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
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
        if (occupiedBlock == null)
        {
            if (other.GetComponentInParent<BlockHealth>())
            {
                occupiedBlock = other.GetComponentInParent<BlockHealth>();
                Color.RGBToHSV(occupiedBlock.gameObject.GetComponent<BlockPrefab>().defaultColor, out H_1, out S_1, out V_1);
                Color.RGBToHSV(occupiedBlock.gameObject.GetComponent<BlockPrefab>().defaultBottomColor, out H_2, out S_2, out V_2);
            }
        }
    }

    public IEnumerator AttackBlock()
    {
        while (currentState == ZombiState.AttackBlock && occupiedBlock != null)
        {
            Debug.Log("Attack");
            occupiedBlock.Damage(0.01f);
           
            V_1 -= 0.01f;
            S_1 -= 0.01f;
            S_2 -= 0.01f;
            V_2 += 0.01f;
            V_1 = Mathf.Clamp(V_1, 0.4f, 1f);
            S_1 = Mathf.Clamp(S_1, 0.1f, 0.75f);
            S_2 = Mathf.Clamp(S_2, 0.1f, 0.9f);
            V_2 = Mathf.Clamp(V_2, 0.025f, 0.75f);
            occupiedBlock.gameObject.GetComponent<BlockPrefab>().defaultColor = Color.HSVToRGB(H_1, S_1, V_1);           
            occupiedBlock.gameObject.GetComponent<BlockPrefab>().defaultBottomColor = Color.HSVToRGB(H_2, S_2, V_2);
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1f);
    }

}
