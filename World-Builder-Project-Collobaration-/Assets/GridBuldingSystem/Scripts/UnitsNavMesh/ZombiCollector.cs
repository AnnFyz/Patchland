using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ZombiList
{
    public List<Zombi> zombis = new List<Zombi>();
    public void AddZombi(Zombi zombi)
    {
        if (!zombis.Contains(zombi))
        {
            zombis.Add(zombi);
        }
    }
    public void RemoveZombi(Zombi zombi)
    {
        if (zombis.Contains(zombi))
        {
            zombis.Remove(zombi);
        }
    }
    public void Clear()
    {
        zombis.Clear();
    }
}
public class ZombiCollector : MonoBehaviour
{
    [SerializeField] ZombiList zombisOnTheBlock;
    LocalLevelState levelState;
    private void Awake()
    {
        levelState = GetComponentInParent<LocalLevelState>();
    }
    private void OnEnable()
    {
        levelState.OnChangedState += RemoveAllZombis;
    }
    public void RemoveAllZombis()
    {
        GetComponentInParent<BlockHealth>().IsBlockInjuring = false;
        GetComponentInParent<BlockHealth>().IsAttacked = false;
        GetComponentInParent<BlockHealth>().IsBeingDamaged = false;
        for (int i = zombisOnTheBlock.zombis.Count - 1; i >= 0; i--)
        {
            zombisOnTheBlock.zombis[i].DestroyZombi(); // Destroy the zombi object
            zombisOnTheBlock.zombis.RemoveAt(i);
        }
    }
    public void CollectZombi(Zombi zombi)
    {
        if (zombi.currentState != ZombiState.None) // to make sure that unit has already become a zombi
        {
            zombisOnTheBlock.AddZombi(zombi);
            Debug.Log("ADD Zombi");
        }

    }

    public void RemoveZombiFromTheList(Zombi zombi)
    {
        if (zombi.currentState != ZombiState.None) // to make sure that unit has already become a zombi
        {
            zombisOnTheBlock.RemoveZombi(zombi);
            Debug.Log("REMOVE Zombi");
        }
    }

}
