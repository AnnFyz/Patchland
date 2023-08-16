using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitsManager : MonoBehaviour
{
    public static UnitsManager Instance { get; private set; }
    public List<Unit> selectedUnits = new List<Unit>();
    //List<GameObject> units = new List<GameObject>();
    public LayerMask unitMask;
    public LayerMask groundMask;
    public List<List<Transform>> waypoints = new List<List<Transform>>(); // to make them for each type of building and unit
    public Action TimeToMoveAutomatically;
    [SerializeField] List<Transform> unitsPrefabs = new List<Transform>();
    public int numberOfPoints;
    public event Action OnChangedGlobalOrder;
    public int maxUnits_0 = 5;
    public int maxUnits_1 = 4;
    public int maxUnits_2 = 3;
    public int maxUnits_3 = 5;
    public int maxUnits_4 = 4;
    public int maxUnits_5 = 3;
    int maxUnits;
    public List<int> amountOfUnits;
    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        MyGridBuildingSystem.OnChangedWaypoints += OnChangedGlobalOrderM;
    }
    private void Start()
    {

        for (int i = 0; i < BuildingManager.Instance.GetNumberOfPlacedObjTypes(); i++) // to make a list for each type of placedObj
        {
           waypoints.Insert(i, new List<Transform>());
           amountOfUnits.Insert(i, 0);
        }
    }
    void Update()
    {
        ToControlUnitsManually();
    }

    public void SetAmountOfUnits(int placedObjId, int a)
    {

        amountOfUnits[placedObjId] += a;
    }
    public int GetAmountOfUnits(int placedObjId)
    {
        return amountOfUnits[placedObjId];
    }
     public int GetMaxUnits(int placedObjId)
    {
        switch (placedObjId)
        {
            case 0:
                maxUnits = maxUnits_0;
                break;
            case 1:
                maxUnits = maxUnits_1;
                break;
            case 2:
                maxUnits = maxUnits_2;
                break;
            case 3:
                maxUnits = maxUnits_3;
                break;
            case 4:
                maxUnits = maxUnits_4;
                break;
            case 5:
                maxUnits = maxUnits_5;
                break;
            default:
                Debug.Log("ERROR");
                break;
        }
        return maxUnits;
    }
    void ToControlUnitsManually()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, unitMask))
            {

                Unit unit = hit.collider.transform.GetComponentInParent<Unit>();
                if (!selectedUnits.Contains(unit))
                {

                    if (unit != null)
                    {
                        if(unit.currentUnitsState != UnitsState.Dead && unit.currentUnitsState != UnitsState.Zombi )
                        {
                            selectedUnits.Add(unit);
                            unit.OnSelected();
                            unit.currentMovemenetState = UnitsMovementState.ControlledFromPlayer;
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
                        unit.currentMovemenetState = UnitsMovementState.Autopilot;
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
                    {
                        if(unit.currentUnitsState != UnitsState.Dead && unit.currentUnitsState != UnitsState.Zombi)
                        {
                            unit.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(hit.point);
                        }
                        
                    }
                      
                }
            }
        }
    }
    public List<Transform> GetListOfUnits()
    {
        return unitsPrefabs;
    }

    void OnChangedGlobalOrderM()
    {
        OnChangedGlobalOrder?.Invoke();
        foreach(var item in waypoints[0])
        {
            GameObject generatedWaypoint = new GameObject();
            if (generatedWaypoint.transform != null && item != null)
            {
                generatedWaypoint.transform.position = item.position;
                generatedWaypoint.transform.SetParent(transform);
            }
      
        }
    }
}
