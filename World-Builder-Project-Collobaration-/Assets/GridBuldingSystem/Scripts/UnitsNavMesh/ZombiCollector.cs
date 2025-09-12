using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ZombiList
{
    public List<Zombi> Zombies = new();
    public void AddZombi(Zombi zombi)
    {
        if (zombi != null && !Zombies.Contains(zombi))
        {
            Zombies.Add(zombi);
        }
    }
    public void RemoveZombi(Zombi zombi)
    {
        if (zombi != null && Zombies.Contains(zombi))
        {
            Zombies.Remove(zombi);
        }
    }
    public void Clear()
    {
        Zombies.Clear();
    }
}

/// <summary>
/// Manages zombies on a block, including collecting,
/// removing, and clearing them when the level state changes.
/// </summary>
public class ZombiCollector : MonoBehaviour
{
    [SerializeField] private ZombiList zombisOnTheBlock;
    private LocalLevelState levelState;
    private BlockHealth blockHealth;
    private void Awake()
    {
        levelState = GetComponentInParent<LocalLevelState>();
        blockHealth = GetComponentInParent<BlockHealth>();
    }
    private void OnEnable()
    {
        if (levelState != null)
            levelState.OnChangedState += RemoveAllZombis;
    }

    private void OnDisable()
    {
        if (levelState != null) 
            levelState.OnChangedState -= RemoveAllZombis;
    }
    public void RemoveAllZombis()
    {
        if (blockHealth != null) 
            blockHealth.IsBeingDamaged = false;

        foreach (var zombi in zombisOnTheBlock.Zombies)
        {
            zombi.DestroyZombi();
        }
        zombisOnTheBlock.Clear();
    }
    public void CollectZombi(Zombi zombi)
    {
        if (zombi != null && zombi.currentState != ZombiState.None)
        {
            zombisOnTheBlock.AddZombi(zombi);
        }

    }

    public void RemoveZombiFromTheList(Zombi zombi)
    {
        if (zombi != null && zombi.currentState != ZombiState.None) // to make sure that unit has already become a zombi
        {
            zombisOnTheBlock.RemoveZombi(zombi);
        }
    }

}
