using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZombiState
{
   AttackUnit,
   AttackBlock,
   DeadZombi
}
public class Zombi : MonoBehaviour
{
    [SerializeField] float rotation = 0;
    [SerializeField] float speed = 10;

    public ZombiState currentState;
    Unit unit;
    [SerializeField] BlockHealth occupiedBlock;
    [SerializeField] float step = 5f;
    private void Awake()
    {
       unit = GetComponent<Unit>();
    }
    public void HandleZombiTransformation()
    {
        Debug.Log("I AM A ZOMBI NOW!");
    }

    public void HandleZombiMovement()
    {
        //Set Destination to generated waypoint in circle
        float angleStep = 360 / step;
        for (int i = 1; i < step+1; i++)
        {
            GameObject generatedWaypoint = new GameObject();
            generatedWaypoint.transform.RotateAround(occupiedBlock.gameObject.transform.position,Vector3.up ,angleStep * i);
            Vector3 dir = (generatedWaypoint.transform.position - occupiedBlock.gameObject.transform.position).normalized;
            generatedWaypoint.transform.position = occupiedBlock.gameObject.transform.position + dir * 5;
            //Debug.DrawLine(occupiedBlock.gameObject.transform.position, occupiedBlock.gameObject.transform.position + dir * 10, Color.red, Mathf.Infinity);
        }
    }


    private void Update()
    {
      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<BlockHealth>())
        {
            occupiedBlock = other.GetComponentInParent<BlockHealth>();
            Debug.Log("TO ATTACK THIS BLOCK");
        }
    }

}
