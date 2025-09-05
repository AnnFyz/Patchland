using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static PlacedObjectTypeSO;



public class GridBuildingSystem : MonoBehaviour
{
    public MyGridXZ<MyGridObject> Grid { get; private set; }
    public MyGridXZ<MyGridObject> OldGrid { get; private set; }

    [SerializeField] private int gridWidth = 2;
    [SerializeField] private int gridHeight = 2;
    [SerializeField] private float cellSize = 5f;
    [SerializeField] private bool isGridOnCorner = false;

    private BlockPrefab blockPrefab;
    private Vector3 origin;
    private int currentHeight = 0;

    public event Action<Transform, int, PlacedObjectName> OnObjectPlaced;
    public static event Action OnChangedWaypoints;
    private void Awake()
    {
        origin = transform.position;
        blockPrefab = GetComponent<BlockPrefab>();
        blockPrefab.OnBlockHeightChanged += UpdateGrid;
        blockPrefab.OnBlockHeightChanged += DeleteOldObjectsAndWaypoints;

    }
    public void SetBlockGrid()
    {
        Grid = new MyGridXZ<MyGridObject>(
            gridWidth,
            gridHeight,
            cellSize,
            origin - BlockPrefab.Offset,
            (MyGridXZ<MyGridObject> g, int x, int y) => new MyGridObject(g, x, y),
            isGridOnCorner,
            blockPrefab.cornerBlock
            );
    }
    public void UpdateGrid(int newHeight)
    {
        currentHeight = newHeight;
        OldGrid = Grid;
        Grid = new MyGridXZ<MyGridObject>(
            gridWidth,
            gridHeight,
            cellSize,
            new Vector3(
                origin.x - BlockPrefab.Offset.x,
                (-newHeight * BlockPrefab.Offset.y) + BlockPrefab.Offset.y,
                origin.z - BlockPrefab.Offset.z),
            (MyGridXZ<MyGridObject> g, int x, int y) => new MyGridObject(g, x, y),
            isGridOnCorner,
            blockPrefab.cornerBlock
            );
    }

    public Vector3 GetOriginOfGrid()
    {
        return new Vector3(
            origin.x - BlockPrefab.Offset.x,
            (-currentHeight * BlockPrefab.Offset.y) + BlockPrefab.Offset.y,
            origin.z - BlockPrefab.Offset.z
            );
    }
    public void DeleteOldObjectsAndWaypoints(int newHeight)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                // 💡 simplified chained null checks using local variable
                var oldObj = OldGrid?.GetGridObject(x, z)?.GetPlacedObject();
                if (oldObj == null) continue;

                int placedObjectId = oldObj.placedObjectTypeSO.placedObjId;
                PlacedObjectName placedObjectName = oldObj.placedObjectTypeSO.placedObjectName;

                if (UnitsManager.Instance.waypointsForPlacedObjects[placedObjectName].Contains(oldObj.transform))
                {
                    Debug.Log($"Removing old waypoint for {placedObjectName} at {x}, {z}"); // 💡 string interpolation
                    UnitsManager.Instance.waypointsForPlacedObjects[placedObjectName].Remove(oldObj.transform);
                    OnChangedWaypoints?.Invoke();
                }

                if (BuildingManager.placedObjects[placedObjectId].Contains(oldObj))
                {
                    BuildingManager.placedObjects[placedObjectId].Remove(oldObj);
                }

                oldObj.DestroySelf();
                Grid.GetGridObject(x, z).ClearPlacedObject();
            }
        }
    }

    public void GetAllPlacedObjectsOnTheBlock()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                var placedObj = Grid.GetGridObject(x, z)?.GetPlacedObject();
                if (placedObj == null) continue;

                placedObj.ChangeMaterialOfObject();

                int placedObjectId = placedObj.placedObjectTypeSO.placedObjId;
                PlacedObjectName placedObjectName = placedObj.placedObjectTypeSO.placedObjectName;

                if (UnitsManager.Instance.waypointsForPlacedObjects[placedObjectName].Contains(placedObj.transform))
                {
                    UnitsManager.Instance.waypointsForPlacedObjects[placedObjectName].Remove(placedObj.transform);
                    OnChangedWaypoints?.Invoke();
                }
            }
        }
    }
    public class MyGridObject
    {

        private readonly MyGridXZ<MyGridObject> grid;
        private readonly int x;
        private readonly int y;
        public PlacedObject_Done placedObject;

        public MyGridObject(MyGridXZ<MyGridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public override string ToString() => $"{x}, {y}\n{placedObject}";

        public void SetPlacedObject(PlacedObject_Done placedObject)
        {
            this.placedObject = placedObject;
            grid.TriggerGridObjectChanged(x, y); // 
        }

        public void ClearPlacedObject()
        {
            placedObject = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public PlacedObject_Done GetPlacedObject() => placedObject;
        public bool CanBuild() => placedObject == null;

    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (BuildingManager.Instance.currentBlockPrefab == null) return;


        LevelState blockState = BuildingManager.Instance.currentBlockPrefab
            .gameObject.GetComponent<LocalLevelState>().GetCurrentLevelState();


        Vector3 mousePosition = GetMouseWorldPosition();


        if (BuildingManager.Instance.currentObjectTypeSO != null && blockPrefab.isHighlighted)
        {

            if (EventSystem.current.IsPointerOverGameObject()) return;
            
            Grid.GetXZ(mousePosition, out int x, out int z);

            Vector2Int placedObjectOrigin = new Vector2Int(x, z);
            placedObjectOrigin = Grid.ValidateGridPosition(placedObjectOrigin);

            // Test Can Build
            List<Vector2Int> gridPositionList = 
                BuildingManager.Instance.currentObjectTypeSO.GetGridPositionList(
                    placedObjectOrigin, BuildingManager.Instance.dir);

            bool canBuild = true;

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                if (Grid == null || Grid.GetGridObject(gridPosition.x, gridPosition.y) == null ||
                     !Grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                {
                    canBuild = false;
                    break;
                }
            }

            if (canBuild)
            {
                Vector2Int rotationOffset = 
                    BuildingManager.Instance.currentObjectTypeSO.GetRotationOffset(BuildingManager.Instance.dir);

                Vector3 placedObjectWorldPosition = 
                    Grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + 
                    new Vector3(rotationOffset.x, 0, rotationOffset.y) * Grid.GetCellSize();

                PlacedObject_Done placedObject = PlacedObject_Done.Create(
                    placedObjectWorldPosition,
                    placedObjectOrigin, 
                    BuildingManager.Instance.dir, 
                    BuildingManager.Instance.currentObjectTypeSO);

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    Grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                }

                BuildingManager.Instance.audio.PlayOneShot(BuildingManager.Instance.placedSound);
                Transform unitToCreate = BuildingManager.Instance.currentObjectTypeSO.unitToCreate; // to know which unit should be spawned


                int placedObjectId = BuildingManager.Instance.currentObjectTypeSO.placedObjId;
                PlacedObjectName placedObjectName = placedObject.placedObjectTypeSO.placedObjectName;

                if (!UnitsManager.Instance.waypointsForPlacedObjects[placedObjectName].Contains(placedObject.transform))
                {
                    UnitsManager.Instance.waypointsForPlacedObjects[placedObjectName].Add(placedObject.transform);
                    Debug.Log($"Waypoint added for {placedObjectName}: {placedObject.transform.position}");
                }


                BuildingManager.placedObjects[placedObjectId].Add(placedObject);
                Debug.Log($"Placed object added: {placedObjectId} - {placedObjectName}");
                BuildingManager.Instance.DestroySurplusPlacedObjects(BuildingManager.Instance.currentObjectTypeSO);


                OnObjectPlaced?.Invoke(unitToCreate, placedObjectId, placedObjectName);
                OnChangedWaypoints?.Invoke();
                BuildingManager.Instance.DeselectObjectType();
            }

            else
            {
                Bubble.Instance.CreatePopupText(mousePosition, "Cannot build here!");
            }
        }   
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit, 999f) ? hit.point : Vector3.zero;
    }
}
