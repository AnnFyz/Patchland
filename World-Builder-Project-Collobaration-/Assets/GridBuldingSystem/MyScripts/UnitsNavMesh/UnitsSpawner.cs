using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using Cinemachine;
using System.Linq;

public class UnitsSpawner : MonoBehaviour
{
    public List<Unit> selectedUnits = new List<Unit>();
    public LayerMask unitMask;
    public LayerMask groundMask;
    //[SerializeField] MazeSpawner mazeSpawner;
    [SerializeField] GameObject unitPrefab;
    [SerializeField] int numberOfUnits = 1;
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

    void SpawnUnits(int placedObjId)
    {
        NavMeshHit hit;
        Debug.Log("Height " + localBuildingSystem.GetOriginOfGrid().y);
        for (int i = 0; i < numberOfUnits; i++)
        {
            float randomPosX = Random.Range(transform.position.x, transform.position.x + 0.5f);
            float randomPosZ = Random.Range(transform.position.z, transform.position.z + 0.5f);
            //int vertexIndex = UnityEngine.Random.Range(transform.position, );
            if (NavMesh.SamplePosition(new Vector3(randomPosX, localBuildingSystem.GetOriginOfGrid().y, randomPosZ), out hit, 10f, groundMask))
            {
                currentUnit = Instantiate(SelectRightUnit(placedObjId).gameObject, Vector3.zero, Quaternion.identity);
                if (UnitsManager.Instance.waypoints[placedObjId].Last() != null)
                {
                    currentUnit.GetComponent<Unit>().startPoint = UnitsManager.Instance.waypoints[placedObjId].Last();
                    currentUnit.GetComponent<Unit>().placedObjTypeId = placedObjId;
                    //currentUnit.GetComponent<Unit>().UpdateListOfWaypoints();

                }
                //currentUnit.transform.parent = this.transform;
                //units.Add(currentUnit);
                if (currentUnit.GetComponent<Unit>())
                {
                    currentUnit.GetComponent<Unit>().agent.Warp(hit.position);
                    currentUnit.GetComponent<Unit>().agent.enabled = true;
                    UnitsManager.Instance.numberOfPoints++;
                }
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
            default:
                Debug.Log("ERROR");
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

