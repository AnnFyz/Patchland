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
    [SerializeField] int numberOfUnits = 1;
    List<GameObject> units = new List<GameObject>();
    MyGridBuildingSystem localBuildingSystem;
    Transform unitToSpawn;
    GameObject currentUnit;
    [SerializeField] LevelState levelState;
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
        levelState = GetComponent<LocalLevelState>().GetCurrentLevelState();
        //Debug.Log("GetCurrentLevelState: " + GetComponent<LocalLevelState>().GetCurrentLevelState());
        //Debug.Log("Level state: " + levelState);
        if (levelState != LevelState.Pond)
        {
            Spawn(placedObjId);
        }

        else
        {
            float randomPosX = Random.Range(transform.position.x, transform.position.x + 0.5f);
            float randomPosZ = Random.Range(transform.position.z, transform.position.z + 0.5f);
            Bubble.Instance.CreatePopupText(new Vector3(randomPosX, localBuildingSystem.GetOriginOfGrid().y, randomPosZ), "I won't spawn here the Unit!");
            Debug.Log("I won't spawn here the Unit!");
        }
        //levelState = GetComponent<LocalLevelState>().GetCurrentLevelState();
        //Debug.Log("GetCurrentLevelState: " + GetComponent<LocalLevelState>().GetCurrentLevelState());
        //Debug.Log("Level state: " + levelState);
        //if (levelState == LevelState.Desert || levelState == LevelState.Forest)
        //{
        //    Spawn(placedObjId);
        //}
        //else if (levelState == LevelState.Desert || levelState == LevelState.Forest || levelState == LevelState.Mountain)
        //{
        //    if (placedObjId == 3 || placedObjId == 4 || placedObjId == 5)
        //        Spawn(placedObjId);
        //    else 
        //    {
        //        float randomPosX = Random.Range(transform.position.x, transform.position.x + 0.5f);
        //        float randomPosZ = Random.Range(transform.position.z, transform.position.z + 0.5f);
        //        Bubble.Instance.CreatePopupText(new Vector3(randomPosX, localBuildingSystem.GetOriginOfGrid().y, randomPosZ), "I won't spawn here the Unit!");
        //        Debug.Log("I won't spawn here the Unit!");
        //    }
        //}

    }

    void Spawn(int placedObjId)
    {
        if (UnitsManager.Instance.GetAmountOfUnits(placedObjId) < UnitsManager.Instance.GetMaxUnits(placedObjId))
        {
            NavMeshHit hit;
            for (int i = 0; i < numberOfUnits; i++)
            {
                float randomPosX = Random.Range(transform.position.x, transform.position.x + 0.5f);
                float randomPosZ = Random.Range(transform.position.z, transform.position.z + 0.5f);
                //int vertexIndex = UnityEngine.Random.Range(transform.position, );
                if (NavMesh.SamplePosition(new Vector3(randomPosX, localBuildingSystem.GetOriginOfGrid().y, randomPosZ), out hit, 10f, groundMask))
                {
                    currentUnit = Instantiate(SelectUnitAndAmount(placedObjId).gameObject, Vector3.zero, Quaternion.identity);
                    if (currentUnit == null)
                    {
                        Debug.Log("currentUnit is null");
                        return;
                    }

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
                        UnitsManager.Instance.SetAmountOfUnits(placedObjId, 1);
                    }
                }
            }
        }
        else
        {
            float randomPosX = Random.Range(transform.position.x, transform.position.x + 0.5f);
            float randomPosZ = Random.Range(transform.position.z, transform.position.z + 0.5f);
            Bubble.Instance.CreatePopupText(new Vector3(randomPosX, localBuildingSystem.GetOriginOfGrid().y, randomPosZ), "You have already created maximum creatures");
            Debug.Log("You have already created maximum creatures");
        }
    }

    Transform SelectUnitAndAmount(int placedObjId)
    {
        switch (placedObjId)
        {
            case 0: // Pond - no unit 
                unitToSpawn = null;
                break;
            case 1: // Desert
                unitToSpawn = UnitsManager.Instance.GetListOfUnits()[1];
                break;
            case 2: // Forest
                unitToSpawn = UnitsManager.Instance.GetListOfUnits()[2];
                break;
            case 3: // Hill
                unitToSpawn = UnitsManager.Instance.GetListOfUnits()[3];
                break;
            case 4: // Mountain
                unitToSpawn = UnitsManager.Instance.GetListOfUnits()[4];
                break;
            case 5: // Snow Mountain
                unitToSpawn = UnitsManager.Instance.GetListOfUnits()[5];
                break;
            default:
                unitToSpawn = null;
                Debug.Log("ERROR");
                break;
        }
        return unitToSpawn;
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

