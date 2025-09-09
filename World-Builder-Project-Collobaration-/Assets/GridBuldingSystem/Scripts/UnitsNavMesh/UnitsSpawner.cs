using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using System.Linq;

public class UnitsSpawner : MonoBehaviour
{
    private LevelState levelState;
    private LocalLevelState localLevelState;

    GridBuildingSystem localBuildingSystem;
    GameObject currentUnit;
    private void Awake()
    {
        localBuildingSystem = GetComponent<GridBuildingSystem>();
        localLevelState = GetComponent<LocalLevelState>();
    }

    void OnEnable()
    {
        localBuildingSystem.OnObjectPlaced += SpawnUnits;
    }
    private void OnDisable()
    {
        localBuildingSystem.OnObjectPlaced -= SpawnUnits;
    }

    void SpawnUnits(Transform unitToSpawn, int placedObjId, PlacedObjectTypeSO.PlacedObjectName placedObjectName)
    {
        levelState = localLevelState.GetCurrentLevelState();
        if (levelState != LevelState.Pond)
        {
            Spawn(unitToSpawn, placedObjId, placedObjectName);
        }

        else
        {
            float y = localBuildingSystem.GetOriginOfGrid().y;
            float randomPosX = Random.Range(transform.position.x, transform.position.x + 0.5f);
            float randomPosZ = Random.Range(transform.position.z, transform.position.z + 0.5f);
            Bubble.Instance.CreatePopupText(new Vector3(randomPosX, y, randomPosZ), "I won't spawn here the Unit!");
            Debug.Log("I won't spawn here the Unit!");
        }
    }

    void Spawn(Transform unitToSpawn, int placedObjId, PlacedObjectTypeSO.PlacedObjectName placedObjectName)
    {
        float originY = localBuildingSystem.GetOriginOfGrid().y;
        float randomPosX = Random.Range(transform.position.x, transform.position.x + 0.5f);
        float randomPosZ = Random.Range(transform.position.z, transform.position.z + 0.5f);

        if (UnitsManager.Instance.GetAmountOfUnits(placedObjectName) >= UnitsManager.Instance.GetMaxUnits())
        {
            Bubble.Instance.CreatePopupText(new Vector3(randomPosX, originY, randomPosZ), "You have already created maximum creatures");
            Debug.Log("You have already created maximum creatures");
            return;
        }
        NavMeshHit hit;

        if (NavMesh.SamplePosition(new Vector3(randomPosX, originY, randomPosZ), out hit, 10f, NavMesh.AllAreas))
        {
            currentUnit = Instantiate(unitToSpawn.gameObject, Vector3.zero, Quaternion.identity);
            Unit unit = currentUnit.GetComponent<Unit>();
            List<Transform> waypointsList = UnitsManager.Instance.waypointsForPlacedObjects[placedObjectName];

            if (waypointsList != null && waypointsList.Count > 0)
            {
                var lastWaypoint = waypointsList[waypointsList.Count - 1];
                if (lastWaypoint != null)
                {
                    unit.StartPoint = lastWaypoint;
                    unit.PlacedObjTypeId = placedObjId;
                    unit.placedObjectName = placedObjectName;
                }
            }

            NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.Warp(hit.position);
                agent.enabled = true;
            }
            else
            {
                Debug.LogWarning("Spawned Unit has no NavMeshAgent.");
            }

            UnitsManager.Instance.SetAmountOfUnits(placedObjectName, 1);
        }


    }
}

