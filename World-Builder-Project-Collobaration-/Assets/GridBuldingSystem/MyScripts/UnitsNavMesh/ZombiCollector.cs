using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiCollector : MonoBehaviour
{
    List<Zombi> zombisOnTheBlock = new List<Zombi>();
    LocalLevelState levelState;
    private void Awake()
    {
        levelState = GetComponentInParent<LocalLevelState>();
    }
    private void OnEnable()
    {
        levelState.OnChangedState += DestroyZombi;
    }
    public void DestroyZombi()
    {
        foreach (Zombi zombi in zombisOnTheBlock)
        {
            if (zombi != null)
            {
                //if (GetComponentInParent<BlockHealth>().IsBlockInjuring)
                //{
                //    GetComponentInParent<BlockHealth>().SetDyingColor();
                //    Debug.Log("SetDyingColor!!!");
                //}
                GetComponentInParent<BlockHealth>().IsBlockInjuring = false;
                GetComponentInParent<BlockHealth>().IsAttacking = false;
                GetComponentInParent<BlockHealth>().IsBeingDamaged = false; 
                Destroy(zombi.gameObject);
            }
        }
    }
    void CollectZombi(Collider other)
    {
        Zombi zombi;
        if (other.GetComponentInParent<Zombi>())
        {
            zombi = other.GetComponentInParent<Zombi>();
            if (!zombisOnTheBlock.Contains(zombi))
            {
                if (zombi.currentState != ZombiState.None)
                {
                    zombisOnTheBlock.Add(zombi);
                    Debug.Log("ADD Zombi");
                }
            }
        }
    }

    void RemoveZombiFromTheList(Collider other)
    {
        Zombi zombi;
        if (other.GetComponentInParent<Zombi>())
        {
            zombi = other.GetComponentInParent<Zombi>();
            if (zombisOnTheBlock.Contains(zombi))
            {
                if (zombi.currentState != ZombiState.None)
                {
                    zombisOnTheBlock.Remove(zombi);
                    Debug.Log("REMOVE Zombi");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CollectZombi(other);
    }
    private void OnTriggerStay(Collider other)
    {
        CollectZombi(other);
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    RemoveZombiFromTheList(other);
    //}
}
