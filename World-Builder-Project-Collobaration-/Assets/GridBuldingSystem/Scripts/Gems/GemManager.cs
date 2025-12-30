using ChristinaCreatesGames.Animations;
using Evets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using Unity.AI.Navigation;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;



public class GemPool
{
    public GemsSO gemSO;
    public int weight;
    public IObjectPool<Gem> gemPool;

    public GemPool(GemsSO gemSO, Transform parentTransform, int capacity)
    {
        this.gemSO = gemSO;
        weight = gemSO.weight;
        gemPool = new ObjectPool<Gem>(() =>
        {
            Gem obj = GameObject.Instantiate(gemSO.gemPrefab, parentTransform);        
            obj.GemPool = gemPool;
            obj.gameObject.SetActive(false);
            return obj;
        },
        actionOnGet: (obj) =>
        {
            obj.gameObject.SetActive(true);
            obj.Setup(gemSO);
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
        defaultCapacity: capacity,
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


    [SerializeField, Tooltip("The area of the biggest unit, to make sure each unit can collect gems")] string  navMeshArea;
    [SerializeField]
    GemsSO[] gems = new GemsSO[4];
    [SerializeField]
    int poolCapacityPerGem = 10;
    public List<Gem> createdGems = new List<Gem>();
    List<GemPool> gemPools = new List<GemPool>();
    [SerializeField]
    Vector3 centerOfGrid;
    [SerializeField] float range = 50f;

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < gems.Length; i++)
        {
            GameObject poolObject = new GameObject(gems[i].gemPrefab.name + " Pool");
            GemPool gemPool = new GemPool(gems[i], poolObject.transform, poolCapacityPerGem);
            gemPools.Add(gemPool);
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            centerOfGrid = GridOfPrefabs.Instance.GetCenterOnGridSurface();
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
        StartCoroutine(StrartSpawningGems());
        StartCoroutine(FadeOutParticleSystem(rainObj, 1.5f));
    }

    IEnumerator StrartSpawningGems()
    {
        int amountToSpawn = maxGemsOnField - createdGems.Count;
        for (int i = 0; i < amountToSpawn; i++)
        {           
            centerOfGrid = GridOfPrefabs.Instance.GetCenterOnGridSurface();
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
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = centerOfGrid + UnityEngine.Random.insideUnitSphere * range;
            NavMeshHit hit;
            // Find nearest point on area.
            int areaMask = 1 << NavMesh.GetAreaFromName(navMeshArea);
            if (NavMesh.SamplePosition(randomPoint, out hit, 2, areaMask))
            {
                Gem gem = gemPool.gemPool.Get();
                createdGems.Add(gem);
                gem.transform.SetPositionAndRotation(hit.position, Quaternion.identity);
                gem.SquashAndStretch();
                return;
            }
            else
            {
                continue;
            }

        }
    }
}


