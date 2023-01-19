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
   public ZombiState currentState;
   public void HandleZombiTransformation()
    {
        Debug.Log("I AM A ZOMBI NOW!");
    }
}
