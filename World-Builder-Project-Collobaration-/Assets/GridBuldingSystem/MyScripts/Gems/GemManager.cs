using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class GemManager : MonoBehaviour //make spawn in waves with particles
{
    public List<GemsSO> gems = new List<GemsSO>();
    NavMeshSurface surface;

    private void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            Debug.Log(GetRandomGem().ToString());
        }

        surface = GridOfPrefabs.Instance.horizontalSurface;
    }
    Transform GetRandomGem()
    {
        Transform gem = null;
        var totalWeight = 0;
        foreach(var item in gems)
        {
            totalWeight += item.weight;
        }
        var rndWeightGem = UnityEngine.Random.Range(0, totalWeight);
        var processedWeight = 0;
        foreach(var item in gems)
        {
            processedWeight += item.weight;
            if(rndWeightGem <= processedWeight)
            {
                gem = item.gemPrefab.transform;
                break;
            }
        }
        return gem;
    }
    void SpawnGems()
    {
        NavMeshHit hit;
    }

}

