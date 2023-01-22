using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class GemManager : MonoBehaviour
{
    NavMeshSurface surface;
    [SerializeField] List<Transform> Gems;
    private void Awake()
    {
        surface = GridOfPrefabs.Instance.horizontalSurface;
    }

    void SpawnGems()
    {
        NavMeshHit hit;
    }
}
