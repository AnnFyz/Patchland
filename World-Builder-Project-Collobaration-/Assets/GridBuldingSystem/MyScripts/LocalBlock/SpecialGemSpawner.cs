using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using UnityEngine.EventSystems;


public class SpecialGemSpawner : MonoBehaviour
{
    [SerializeField] GemsSO specialGem;
    NavMeshTriangulation triangulation;
    int index = 0;
    void OnEnable()
    {
        DayAndNightController.Instance.isTimeToSpawnGems += StartSpawningSpecialGems;
    }


    void StartSpawningSpecialGems()
    {
        if (GetComponent<LocalLevelState>().GetCurrentLevelState() == LevelState.SnowMountain && DayAndNightController.Instance.timeOfNight >= 0)
        {
            if (GemManager.Instance.createdSpecialGems.Count <= 2)
            {
                StartCoroutine(SpawnSpecialGems());
            }
        }
        else
        {
            StopCoroutine(SpawnSpecialGems());
        }
    }

    IEnumerator SpawnSpecialGems()
    {
        while (GemManager.Instance.createdSpecialGems.Count <= 1)
        {
            yield return new WaitForSeconds(3f);
            triangulation = UnityEngine.AI.NavMesh.CalculateTriangulation();
            if(GemManager.Instance.createdSpecialGems.Count <= 1)
            {
                SpawnSpecialGem();
                yield return new WaitForSeconds(3f);
            }
        

        }

    }

    void SpawnSpecialGem()
    {
      
        int vertexIndex = UnityEngine.Random.Range(0, triangulation.vertices.Length);
        UnityEngine.AI.NavMeshHit hit;
        float randomPosX = Random.Range(transform.position.x, transform.position.x + 0.5f);
        float randomPosZ = Random.Range(transform.position.z, transform.position.z + 0.5f);
        if (NavMesh.SamplePosition(new Vector3(randomPosX, GetComponent<MyGridBuildingSystem>().GetOriginOfGrid().y, randomPosZ), out hit, 10f, -1))
        {
            Transform gem = Instantiate(specialGem.gemPrefab.transform, hit.position, Quaternion.identity, GemManager.Instance.gameObject.transform);
            gem.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(hit.position);
            gem.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
            GemManager.Instance.createdSpecialGems.Add(gem);
            index = 0;
        }
        else
        {
            if (index < 50)
            {
                SpawnSpecialGem();
                index++;
            }
        }

    }
}
