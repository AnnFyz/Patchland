using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class GemManager : MonoBehaviour //make spawn in waves with particles
{
    NavMeshSurface surface;
    [SerializeField] List<WeightedGem> gems = new List<WeightedGem>();
    private void Awake()
    {
        surface = GridOfPrefabs.Instance.horizontalSurface;
    }

    Transform GetRandomGem()
    {
        return transform;
    }
    void SpawnGems()
    {
        NavMeshHit hit;
    }

    class WeightedGem
    {
        public int weight;
        public Transform gemPrefab;
    }
}
