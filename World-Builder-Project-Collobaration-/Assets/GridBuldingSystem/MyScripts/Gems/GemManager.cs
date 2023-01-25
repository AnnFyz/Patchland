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
    NavMeshTriangulation triangulation;
    GameObject rainObj;
    List<Transform> createdGems = new List<Transform>();
    private void Start()
    {
        surface = GridOfPrefabs.Instance.horizontalSurface;
        rainObj = transform.GetChild(0).gameObject;
        rainObj.SetActive(false);
        StartCoroutine(SpawnGemsInWaves());
    }

    IEnumerator SpawnGemsInWaves()
    {
        while (createdGems.Count <= 30)
        {
         rainObj.SetActive(true);
         yield return new WaitForSeconds(5f);
        for (int i = 0; i < 10; i++)
        {
          SpawnRandomGems(1);
          yield return new WaitForSeconds(1f);
        }
         rainObj.SetActive(false);
         yield return null;

        }    
    }
    Transform GetRandomGem()
    {
        Transform gem = null;
        var totalWeight = 0;
        foreach (var item in gems)
        {
            totalWeight += item.weight;
        }
        var rndWeightGem = UnityEngine.Random.Range(0, totalWeight);
        var processedWeight = 0;
        foreach (var item in gems)
        {
            processedWeight += item.weight;
            if (rndWeightGem <= processedWeight)
            {
                gem = item.gemPrefab.transform;
                break;
            }
        }
        return gem;
    }
    void SpawnRandomGems(int numberOfGems)
    {
        triangulation = NavMesh.CalculateTriangulation();
        int vertexIndex = UnityEngine.Random.Range(0, triangulation.vertices.Length);
        NavMeshHit hit;
        Vector3 randomPos;
        for (int i = 0; i < numberOfGems; i++)
        {
            randomPos = new Vector3(UnityEngine.Random.Range(-5, 5), 0, UnityEngine.Random.Range(-5, 5));
            if (NavMesh.SamplePosition(triangulation.vertices[vertexIndex] + randomPos, out hit, 1f, -1))
            {
                Transform gem = Instantiate(GetRandomGem(), hit.position, Quaternion.identity, transform);
                gem.GetComponent<NavMeshAgent>().Warp(hit.position);
                gem.GetComponent<NavMeshAgent>().enabled = true;
                createdGems.Add(gem);
            }
            else
            {
                SpawnRandomGem();
            }
        }

    }

    void SpawnRandomGem()
    {
        triangulation = NavMesh.CalculateTriangulation();
        int vertexIndex = UnityEngine.Random.Range(0, triangulation.vertices.Length);
        NavMeshHit hit;
        Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-5, 5), 0, UnityEngine.Random.Range(-5, 5));
        if (NavMesh.SamplePosition(triangulation.vertices[vertexIndex] + randomPos, out hit, 1f, -1))
        {
            Transform gem = Instantiate(GetRandomGem(), hit.position, Quaternion.identity, transform);
            gem.GetComponent<NavMeshAgent>().Warp(hit.position);
            gem.GetComponent<NavMeshAgent>().enabled = true;
            createdGems.Add(gem);
        }
        else
        {
            SpawnRandomGem();
        }

    }

}

