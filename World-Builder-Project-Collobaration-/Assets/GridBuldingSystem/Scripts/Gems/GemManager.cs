using Evets;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public static GemManager Instance { get; private set; }

    [SerializeField]
    GemsSO[] gems = new GemsSO[4];
    NavMeshTriangulation triangulation;
    public List<Transform> createdGems = new List<Transform>();
    public List<Transform> createdSpecialGems = new List<Transform>();
    List<GemPool> gemPools = new List<GemPool>(); 

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
    }

    void OnEnable()
    {
        skyboxController.OnDarkestTimeReached += StartRain;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SpawnGem(GetRandomGemPool() ,new Vector3(UnityEngine.Random.Range(-5, 5), 10, UnityEngine.Random.Range(-5, 5)));
        }
    }

    void StartRain(bool isDarkestTime)
    {
        if(isDarkestTime)
            StartCoroutine(StartRaining());
    }

    IEnumerator StartRaining()
    {
        rainObj.Play();
        yield return new WaitForSeconds(UnityEngine.Random.Range(5, 7));
        for (int i = 0; i < 10; i++)
        {
            // TO DO to spawn gems on the blocks
            SpawnGem(GetRandomGemPool(), new Vector3(UnityEngine.Random.Range(-5, 5), 10, UnityEngine.Random.Range(-5, 5)));
        }
        yield return new WaitForSeconds(3.5f);
        StartCoroutine(FadeOutParticleSystem(rainObj, 1.5f));
        //rainObj.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }


    public IEnumerator FadeOutParticleSystem(ParticleSystem ps, float duration)
    {
        if (ps == null) yield break;
        var main = ps.main;
        var childPs = ps.GetComponentInChildren<ParticleSystem>();
        var childMain = childPs.main;

        // Get initial start color
        Color startColor = main.startColor.color;
        Color endColor = startColor;
        endColor.a = 0f;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            Color current = Color.Lerp(startColor, endColor, t);
            main.startColor = current;
            childMain.startColor = current;
            yield return null;
        }

        // Ensure fully transparent at the end
        main.startColor = startColor;
        childMain.startColor = startColor;
        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);


    }

    public GameObject SpawnGem(GemPool gemPool, Vector3 position)
    {
        GameObject gem = gemPool.gemPool.Get();
        gem.transform.position = position;
        gem.transform.rotation = Quaternion.identity;
        return gem;
    }

    public void ReleaseGem(GameObject gem) 
    {
        gemPools[0].gemPool.Release(gem);
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
    //        //rainObj.SetActive(false);
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

}

