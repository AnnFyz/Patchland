using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PlacedObjectTypeSO;
using UnityEngine.Rendering;
using AYellowpaper.SerializedCollections;
using System.Linq;


public class UnitsManager : MonoBehaviour
{
    public static UnitsManager Instance { get; private set; }
    public List<Unit> selectedUnits = new List<Unit>();
    //List<GameObject> units = new List<GameObject>();
    public LayerMask unitMask;
    public LayerMask groundMask;
    public LayerMask gemMask;
    //NEW to make them for each type of building and unit
    [SerializedDictionary("PlacedObjects", "Waypoints")]
    public AYellowpaper.SerializedCollections.SerializedDictionary<PlacedObjectName, List<Transform>> waypointsForPlacedObjects = new AYellowpaper.SerializedCollections.SerializedDictionary<PlacedObjectName, List<Transform>>();
    //OLD
    public List<List<Transform>> waypoints = new List<List<Transform>>();
    public Action TimeToMoveAutomatically;
    public event Action OnChangedGlobalOrder;
    //int maxUnits;
    //NEW
    public AYellowpaper.SerializedCollections.SerializedDictionary<PlacedObjectName, int> amountOfUnitsForPlacedObject = new AYellowpaper.SerializedCollections.SerializedDictionary<PlacedObjectName, int>();
    //OLD
    public List<int> amountOfUnits;
    private void Awake()
    {
        // Singleton pattern to ensure only one instance of UnitsManager exists
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    private void OnEnable()
    {
        GridBuildingSystem.OnChangedWaypoints += OnChangedGlobalOrderM;
    }
    private void Start()
    {
        var placedObjectTypeSOList = BuildingManager.Instance.GetPlacedObjectTypeSOList();
        if (placedObjectTypeSOList != null)
        {
            foreach (var placedObjectTypeSO in placedObjectTypeSOList)
            {
                waypointsForPlacedObjects.Add(placedObjectTypeSO.placedObjectName, new List<Transform>());
                amountOfUnitsForPlacedObject.Add(placedObjectTypeSO.placedObjectName, 0);
            }
        }
        else
        {
            Debug.LogError("PlacedObjectTypeSO list is null in BuildingManager.");
        }
    }
    void Update()
    {
        ControlUnitManually();
    }

    public void SetAmountOfUnits(PlacedObjectName placedObjectName, int a)
    {
        amountOfUnitsForPlacedObject[placedObjectName] += a;
    }
    public int GetAmountOfUnits(PlacedObjectName placedObjectName)
    {
        return amountOfUnitsForPlacedObject[placedObjectName];
    }
    public int GetMaxUnits()
    {
        if (BuildingManager.Instance.currentObjectTypeSO == null)
        {
            Debug.LogError("PlacedObjectTypeSO is not set in BuildingManager.");
            return 0;
        }
        return BuildingManager.Instance.currentObjectTypeSO.maxAmountOfUnits;

    }
    void ControlUnitManually()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // if the player clicks on the unit, then select it
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, unitMask))
            {
                // if the unit is already selected, then deselect it
                Unit unit = hit.collider.transform.GetComponentInParent<Unit>();
                if (!selectedUnits.Contains(unit))
                {

                    if (unit != null)
                    {
                        if (unit.CurrentUnitsState != UnitsState.Dead && unit.CurrentUnitsState != UnitsState.Zombi)
                        {
                            selectedUnits.Add(unit);
                            unit.OnSelected();
                            unit.CurrentMovementState = UnitsMovementState.ControlledFromPlayer;
                        }
                    }
                }
            }

            else
            {
                foreach (var unit in selectedUnits)
                {
                    if (unit != null)
                        unit.OnDeselected();
                    unit.CurrentMovementState = UnitsMovementState.Autopilot;
                }

                selectedUnits.Clear();
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            // Combine masks so the ray can hit either ground or gems
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Combine masks so the ray can hit either ground or gems
            LayerMask combinedMask = groundMask | gemMask;

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, combinedMask))
            {
                // Default destination: hit point
                Vector3 destination = hit.point;

                // If we hit a gem layer, use the gem object's transform position instead
                if (((1 << hit.collider.gameObject.layer) & gemMask) != 0)
                {
                    destination = hit.collider.transform.position; // or hit.collider.GetComponentInParent<Gem>().transform.position
                    Debug.Log("Gem clicked, setting destination to gem position.");
                }

                foreach (Unit unit in selectedUnits)
                {
                    if (unit == null) continue;
                    if (unit.CurrentUnitsState == UnitsState.Dead || unit.CurrentUnitsState == UnitsState.Zombi) continue;


                    var agent = unit.GetComponent<UnityEngine.AI.NavMeshAgent>();
                    if (agent != null)
                        agent.SetDestination(destination);

                }
            }
        }
    }

    void OnChangedGlobalOrderM()
    {
        OnChangedGlobalOrder?.Invoke();
        Debug.Log("OnChangedGlobalOrderM called in UnitsManager");
    }
}
