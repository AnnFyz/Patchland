using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using Cinemachine;

public class UnitsSpawner : MonoBehaviour
{
    public List<Unit> selectedUnits = new List<Unit>();
    public LayerMask unitMask;
    public LayerMask groundMask;
    //[SerializeField] MazeSpawner mazeSpawner;
    [SerializeField] int numberOfUnits = 1;
    NavMeshSurface surface;
    //NavMeshTriangulation triangulation;
    List<GameObject> units = new List<GameObject>();
    MyGridBuildingSystem localBuildingSystem;
    Transform unitPrefabToSpawn;
    GameObject currentUnit;
    private void Awake()
    {
        localBuildingSystem = GetComponent<MyGridBuildingSystem>();
    }
    void OnEnable()
    {
        localBuildingSystem.OnObjectPlaced += SpawnUnits;
    }

    private void Start()
    {
        surface = GridOfPrefabs.Instance.horizontalSurface;
    }

    void SpawnUnits(int placedObjId)
    {
        //triangulation = NavMesh.CalculateTriangulation();
        NavMeshHit hit;
        for (int i = 0; i < numberOfUnits; i++)
        {
            // int vertexIndex = UnityEngine.Random.Range(0, triangulation.vertices.Length);
            float randomPosX = Random.Range(transform.position.x, transform.position.x + 1.5f);
            float randomPosZ = Random.Range(transform.position.z, transform.position.z + 1.5f);
            //int vertexIndex = UnityEngine.Random.Range(transform.position, );
            if (NavMesh.SamplePosition(new Vector3(randomPosX, 0, randomPosZ), out hit, 200f, groundMask))
            {
                currentUnit = Instantiate(SelectRightUnit(placedObjId).gameObject, Vector3.zero, Quaternion.identity);
                //currentUnit.transform.parent = this.transform;
                units.Add(currentUnit);
                if (currentUnit.GetComponentInChildren<Unit>())
                {
                    currentUnit.GetComponentInChildren<Unit>().agent.Warp(hit.position);
                    currentUnit.GetComponentInChildren<Unit>().agent.enabled = true;
                }
            }
            else
            {
                Debug.LogError($"Unable to place NavMeshAgent on NavMesh. Tried to use"); //{triangulation.vertices[vertexIndex]}");
            }
        }
    }

    Transform SelectRightUnit(int placedObjId)
    {
        switch (placedObjId)
        {
            case 0:
                unitPrefabToSpawn = UnitsManager.Instance.GetListOfUnits()[0];
                break;
            case 1:
                unitPrefabToSpawn = UnitsManager.Instance.GetListOfUnits()[1];
                break;
            case 2:
                unitPrefabToSpawn = UnitsManager.Instance.GetListOfUnits()[2];
                break;
        }
        return unitPrefabToSpawn;
    }
    void DestroyUnits()
    {
        foreach (var unit in units)
        {
            if (unit != null)
                Destroy(unit);
        }
    }
}
