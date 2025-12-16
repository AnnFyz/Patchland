using ChristinaCreatesGames.Animations;
using Evets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Pool;



public class GemPool : MonoBehaviour
{
    public GemsSO gemSO;
    public int weight;
    public ObjectPool<GameObject> gemPool;
    public GemPool(GemsSO gemSO, Transform parentTransform)
    {
        this.gemSO = gemSO;
        weight = gemSO.weight;
        gemPool = new ObjectPool<GameObject>(() =>
        {
            GameObject obj = Instantiate(gemSO.gemPrefab, parentTransform);
            Debug.Log("Instantiated new gem: " + obj.name);
            obj.SetActive(false);
            return obj;
        },
        actionOnGet: (obj) =>
        {
            obj.gameObject.SetActive(true);
        },
        actionOnRelease: (obj) =>
        {
            obj.gameObject.SetActive(false);
        },
        actionOnDestroy: (obj) =>
        {
            GameObject.Destroy(obj.gameObject);
        },
        collectionCheck: false,
        defaultCapacity: 10,
        maxSize: 100
        );
    }
}

public class GemManager : MonoBehaviour //make spawn in waves with particles
{
    [SerializeField]
    private SkyboxController skyboxController;
    [SerializeField]
    private ParticleSystem rainObj;
    [SerializeField]
    int maxGemsOnField = 10;
    public static GemManager Instance { get; private set; }

    [SerializeField]
    GemsSO[] gems = new GemsSO[4];
    NavMeshTriangulation triangulation;
    public List<GameObject> createdGems = new List<GameObject>();
    public List<Transform> createdSpecialGems = new List<Transform>();
    List<GemPool> gemPools = new List<GemPool>();
    [SerializeField]
    Vector3 centerOfGrid;
    [SerializeField] float range = 50f;

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < gems.Length; i++)
        {
            GemPool gemPool = new GemPool(gems[i], this.transform);
            gemPools.Add(gemPool);         
            Debug.Log("Created pool for " + gems[i].gemPrefab.name);
        }

    }

    private void Start()
    {
        rainObj.Stop();

        foreach (var gemPool in gemPools)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject gem = gemPool.gemPool.Get();
                gemPool.gemPool.Release(gem);
            }
        }
    }

    void OnEnable()
    {
        skyboxController.OnDarkestTimeReached += StartRain;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            centerOfGrid = new Vector3(GridOfPrefabs.Instance.GetCenterObjInGrid().position.x, GridOfPrefabs.Instance.GetCenterObjInGrid().position.y + (BlockPrefab.Offset.y * -1 ) + 0.25f, GridOfPrefabs.Instance.GetCenterObjInGrid().position.z);
            SpawnGem(GetRandomGemPool());
        }
    }

    void StartRain(bool isDarkestTime)
    {
        if (isDarkestTime)
            StartCoroutine(StartRaining());
    }


    // Coroutine to handle the raining and gem spawning
    IEnumerator StartRaining()
    {
        rainObj.Play();
        yield return new WaitForSeconds(UnityEngine.Random.Range(5, 7));
        //for (int i = 0; i < 10; i++)
        //{

        //    SpawnGem(GetRandomGemPool());
        //    yield return new WaitForSeconds(0.5f);
        //}
        //yield return new WaitForSeconds(3.5f);
        StartCoroutine(StrartSpawningGems());
        StartCoroutine(FadeOutParticleSystem(rainObj, 1.5f));
        //rainObj.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    IEnumerator StrartSpawningGems()
    {
        int amountToSpawn = maxGemsOnField - createdGems.Count;
        for (int i = 0; i < amountToSpawn; i++)
        {
            centerOfGrid = new Vector3(GridOfPrefabs.Instance.GetCenterObjInGrid().position.x, GridOfPrefabs.Instance.GetCenterObjInGrid().position.y + (BlockPrefab.Offset.y * -1), GridOfPrefabs.Instance.GetCenterObjInGrid().position.z);
            SpawnGem(GetRandomGemPool());
            yield return new WaitForSeconds(0.5f);
        }

    }


    public IEnumerator FadeOutParticleSystem(ParticleSystem ps, float duration)
    {
        if (ps == null) yield break;
        var main = ps.main;
        var childPs = ps.GetComponentInChildren<ParticleSystem>();
        var childMain = childPs.main;

        // Get initial start color
        UnityEngine.Color startColor = main.startColor.color;
        UnityEngine.Color endColor = startColor;
        endColor.a = 0f;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            UnityEngine.Color current = UnityEngine.Color.Lerp(startColor, endColor, t);
            childMain.startColor = current;
            yield return null;
        }

        // Ensure fully transparent at the end
        main.startColor = startColor;
        childMain.startColor = startColor;
        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);


    }

    GemPool GetRandomGemPool()
    {
        GemPool gemPool = null;
        var totalWeight = 0;
        foreach (var item in gemPools)
        {
            totalWeight += item.weight;
        }
        var rndWeightGem = UnityEngine.Random.Range(0, totalWeight);
        var processedWeight = 0;
        foreach (var item in gemPools)
        {
            processedWeight += item.weight;
            if (rndWeightGem <= processedWeight)
            {
                gemPool = item;
                break;
            }
        }
        return gemPool;
    }


    public void SpawnGem(GemPool gemPool)
    {
        //GameObject gem = gemPool.gemPool.Get();
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = centerOfGrid + UnityEngine.Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                GameObject gem = gemPool.gemPool.Get();
                gem.transform.position = hit.position;
                createdGems.Add(gem);
                //GameObject gem = gemPool.gemSO.gemPrefab;
                //Instantiate(gem, hit.position, Quaternion.identity, this.transform);
                //gem.SetActive(true);
                //gem.GetComponent<Gem>().SquashAndStretch();
                return;
            }
            else
            {
                i--;
                //gemPool.gemPool.Release(gem);
                Debug.Log("Failed to find valid position for gem. Retrying...");
                continue;
            }

        }

    }
}

//void StartSpawning()
//{ 
//    if (DayAndNightController.Instance.timeOfNight >= 0)
//    {
//        StartCoroutine(SpawnGemsInWaves());
//        Debug.Log("SPAWN GEMS");
//    }
//    else
//    {
//        StopCoroutine(SpawnGemsInWaves());
//        Debug.Log("STOP SPAWN GEMS");
//    }
//}
//    IEnumerator SpawnGemsInWaves()
//{

//    while (createdGems.Count <= 30)
//    {
//        //rainObj.SetActive(true);
//        yield return new WaitForSeconds(2f);
//        thunder.Play();
//        yield return new WaitForSeconds(2f);
//        for (int i = 0; i < 10; i++)
//        {
//            SpawnRandomGems(1);
//            yield return new WaitForSeconds(1f);
//        }
//        //rainObj.SetActive(false);n 
//        yield return null;

//    }
//}
//Transform GetRandomGem() ObjectPool  ()
//{
//    Transform gem = null;
//    var totalWeight = 0;
//    foreach (var item in gems)
//    {
//        totalWeight += item.weight;
//    }
//    var rndWeightGem = UnityEngine.Random.Range(0, totalWeight);
//    var processedWeight = 0;
//    foreach (var item in gems)
//    {
//        processedWeight += item.weight;
//        if (rndWeightGem <= processedWeight)
//        {
//            gem = item.gemPrefab.transform;
//            break;
//        }
//    }
//    return gem;
//}
//void SpawnRandomGems(int numberOfGems)
//{
//    triangulation = NavMesh.CalculateTriangulation();
//    int vertexIndex = UnityEngine.Random.Range(0, triangulation.vertices.Length);
//    NavMeshHit hit;
//    Vector3 randomPos;
//    for (int i = 0; i < numberOfGems; i++)
//    {
//        randomPos = new Vector3(UnityEngine.Random.Range(-5, 5), 0, UnityEngine.Random.Range(-5, 5));
//        if (NavMesh.SamplePosition(triangulation.vertices[vertexIndex] + randomPos, out hit, 1f, -1))
//        {
//            //Transform gem = Instantiate(GetRandomGem(), hit.position, Quaternion.identity, transform);
//            ObjectPool currentGemPool = GetRandomGemPool();
//            Transform gem = currentGemPool.Get();
//            
//            gem.GetComponent<NavMeshAgent>().Warp(hit.position);
//            gem.GetComponent<NavMeshAgent>().enabled = true;
//            createdGems.Add(gem);
//        }
////        else
////        {
////            SpawnRandomGem();
////        }
//    }
//}

////void SpawnRandomGem()
////{
////    triangulation = NavMesh.CalculateTriangulation();
////    int vertexIndex = UnityEngine.Random.Range(0, triangulation.vertices.Length);
////    NavMeshHit hit;
////    Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-5, 5), 0, UnityEngine.Random.Range(-5, 5));
////    if (NavMesh.SamplePosition(triangulation.vertices[vertexIndex] + randomPos, out hit, 1f, -1))
////    {
////        Transform gem = Instantiate(GetRandomGem(), hit.position, Quaternion.identity, transform);
////        gem.GetComponent<NavMeshAgent>().Warp(hit.position);
////        gem.GetComponent<NavMeshAgent>().enabled = true;
////        createdGems.Add(gem);
////    }
////    else
////    {
////        SpawnRandomGem();
////    }

////}



