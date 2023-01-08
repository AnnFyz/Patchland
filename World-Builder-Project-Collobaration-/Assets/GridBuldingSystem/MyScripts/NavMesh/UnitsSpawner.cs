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
    [SerializeField] GameObject unitPrefab;
    [SerializeField] int numberOfUnits = 3;
    NavMeshSurface surface;
    NavMeshTriangulation triangulation;
    List<GameObject> units = new List<GameObject>();
    MyGridBuildingSystem localBuildingSystem;
    private void Awake()
    {
        localBuildingSystem = GetComponent<MyGridBuildingSystem>();
    }
    void OnEnable()
    {
        //mazeSpawner.OnMazeSpawned += SpawnUnits;
        //mazeSpawner.OnMazeDestroyed += DestroyUnits;
        localBuildingSystem.OnObjectPlaced += SpawnUnits;
    }

    private void Start()
    {
        surface = GridOfPrefabs.Instance.surface;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, unitMask))
            {
                Unit unit = hit.transform.GetComponent<Unit>();
                if (!selectedUnits.Contains(unit))
                {

                    if (unit != null)
                    {
                        selectedUnits.Add(unit);
                        unit.OnSelected();
                    }

                }
            }

            else
            {
                foreach (var unit in selectedUnits)
                {
                    if (unit != null)
                        unit.OnDeselected();
                }

                selectedUnits.Clear();
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, groundMask))
            {
                foreach (Unit unit in selectedUnits)
                {
                    if (unit != null)
                    unit.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(hit.point);
                }
            }
        }
    }

    void SpawnUnits()
    {
        triangulation = NavMesh.CalculateTriangulation();
        NavMeshHit hit;
        for (int i = 0; i < numberOfUnits; i++)
        {
            // int vertexIndex = UnityEngine.Random.Range(0, triangulation.vertices.Length);
            float randomPosX = Random.Range(transform.position.x, transform.position.x + 1.5f);
            float randomPosZ = Random.Range(transform.position.z, transform.position.z + 1.5f);
            //int vertexIndex = UnityEngine.Random.Range(transform.position, );
            if (NavMesh.SamplePosition(new Vector3(randomPosX - transform.localScale.x*0.5f, 0, randomPosZ - transform.localScale.z * 0.5f), out hit, 200f, groundMask))
            {
                GameObject currentUnit = Instantiate(unitPrefab, Vector3.zero, Quaternion.identity);
                units.Add(currentUnit);
                if (currentUnit.GetComponent<Unit>())
                {
                    currentUnit.GetComponent<Unit>().agent.Warp(hit.position);
                    currentUnit.GetComponent<Unit>().agent.enabled = true;
                }
            }
            else
            {
                Debug.LogError($"Unable to place NavMeshAgent on NavMesh. Tried to use"); //{triangulation.vertices[vertexIndex]}");
            }
        }
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
