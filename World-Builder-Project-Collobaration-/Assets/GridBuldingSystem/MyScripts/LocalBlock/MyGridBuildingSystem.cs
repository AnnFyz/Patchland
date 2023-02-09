using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class MyGridBuildingSystem : MonoBehaviour
{
    public MyGridXZ<MyGridObject> grid;
    public MyGridXZ<MyGridObject> oldGrid;
    [SerializeField] int gridWidth = 2;
    [SerializeField] int gridHeight = 2;
    [SerializeField] float cellSize = 5f;
    BlockPrefab blockPrefab;
    public Vector3 origin;
    public event Action<int> OnObjectPlaced;
    public static event Action OnChangedWaypoints;
    //public event EventHandler OnObjectPlaced; // for sound 
    //public event EventHandler OnSelectedChanged; // for ghost building
    int newHeight = 0;
    private void Awake()
    {
        origin = transform.position;
        blockPrefab = GetComponent<BlockPrefab>();
        grid = new MyGridXZ<MyGridObject>(gridWidth, gridHeight, cellSize, origin - BlockPrefab.offset, (MyGridXZ<MyGridObject> g, int x, int y) => new MyGridObject(g, x, y));
        blockPrefab.OnHeightChanged += UpdateGrid;
        blockPrefab.OnHeightChanged += DeleteOldObjectsAndWaypoints;
    }

    public void UpdateGrid(int newHeight)
    {
        this.newHeight = newHeight;
        oldGrid = grid;
        grid = new MyGridXZ<MyGridObject>(gridWidth, gridHeight, cellSize, new Vector3(origin.x - BlockPrefab.offset.x, (-newHeight * BlockPrefab.offset.y) + BlockPrefab.offset.y, origin.z - BlockPrefab.offset.z), (MyGridXZ<MyGridObject> g, int x, int y) => new MyGridObject(g, x, y));
    }

    public Vector3 GetOriginOfGrid()
    {
        return new Vector3(origin.x - BlockPrefab.offset.x, (-newHeight * BlockPrefab.offset.y) + BlockPrefab.offset.y, origin.z - BlockPrefab.offset.z);
    }
    public void DeleteOldObjectsAndWaypoints(int newHeight)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                if (oldGrid != null && oldGrid.GetGridObject(x, z) != null && oldGrid.GetGridObject(x, z).GetPlacedObject() != null)
                {
                    int placedObjectId = oldGrid.GetGridObject(x, z).GetPlacedObject().placedObjectTypeSO.placedObjId;
                    if (UnitsManager.Instance.waypoints[placedObjectId].Contains(oldGrid.GetGridObject(x, z).GetPlacedObject().transform))
                    {
                        UnitsManager.Instance.waypoints[placedObjectId].Remove(oldGrid.GetGridObject(x, z).GetPlacedObject().transform);
                        OnChangedWaypoints?.Invoke();
                    }

                    if (BuildingManager.placedObjects[placedObjectId].Contains(oldGrid.GetGridObject(x, z).GetPlacedObject()))
                    {
                        BuildingManager.placedObjects[placedObjectId].Remove(oldGrid.GetGridObject(x, z).GetPlacedObject());
                    }

                    oldGrid.GetGridObject(x, z).GetPlacedObject().DestroySelf();
                    grid.GetGridObject(x, z).ClearPlacedObject();

                }
            }
        }

    }

    public void GetAllPlacedObjectsOnTheBlock()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                if (grid.GetGridObject(x, z) != null && grid.GetGridObject(x, z).GetPlacedObject() != null)
                {
                    grid.GetGridObject(x, z).GetPlacedObject().ChangeMaterialOfObject();

                    int placedObjectId = grid.GetGridObject(x, z).GetPlacedObject().placedObjectTypeSO.placedObjId;
                    if (UnitsManager.Instance.waypoints[placedObjectId].Contains(grid.GetGridObject(x, z).GetPlacedObject().transform))
                    {
                        UnitsManager.Instance.waypoints[placedObjectId].Remove(grid.GetGridObject(x, z).GetPlacedObject().transform);
                        OnChangedWaypoints?.Invoke();
                    }
                }
            }
        }
    }
    public class MyGridObject
    {

        private MyGridXZ<MyGridObject> grid;
        private int x;
        private int y;
        public PlacedObject_Done placedObject;

        public MyGridObject(MyGridXZ<MyGridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            placedObject = null;
        }

        public override string ToString()
        {
            return x + ", " + y + "\n" + placedObject;
        }

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

        public PlacedObject_Done GetPlacedObject()
        {
            return placedObject;
        }

        public bool CanBuild()
        {
            return placedObject == null;
        }

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (BuildingManager.blockPrefab != null)
            {
                LevelState blockState = BuildingManager.blockPrefab.gameObject.GetComponent<LocalLevelState>().GetCurrentLevelState();
                Vector3 mousePosition = GetMouseWorldPosition();
                if (BuildingManager.Instance.placedObjectTypeSO != null && blockPrefab.IsThisBlockWasHighlighted && blockState != LevelState.Pond && blockState != LevelState.Hill && CheckIfFitBlock(blockState, mousePosition))
                {

                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        return;
                    }
                    //Vector3 mousePosition = GetMouseWorldPosition();
                    grid.GetXZ(mousePosition, out int x, out int z);

                    Vector2Int placedObjectOrigin = new Vector2Int(x, z);
                    placedObjectOrigin = grid.ValidateGridPosition(placedObjectOrigin);

                    // Test Can Build
                    List<Vector2Int> gridPositionList = BuildingManager.Instance.placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, BuildingManager.Instance.dir);

                    bool canBuild = true;
                    foreach (Vector2Int gridPosition in gridPositionList)
                    {
                        if (!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild() && grid.GetGridObject(gridPosition.x, gridPosition.y) != null)
                        {
                            canBuild = false;
                            break;
                        }
                    }

                    if (canBuild)
                    {
                        Vector2Int rotationOffset = BuildingManager.Instance.placedObjectTypeSO.GetRotationOffset(BuildingManager.Instance.dir);
                        Vector3 placedObjectWorldPosition = grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
                        PlacedObject_Done placedObject = PlacedObject_Done.Create(placedObjectWorldPosition, placedObjectOrigin, BuildingManager.Instance.dir, BuildingManager.Instance.placedObjectTypeSO);

                        foreach (Vector2Int gridPosition in gridPositionList)
                        {
                            grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                        }

                        //OnObjectPlaced?.Invoke(this, EventArgs.Empty); // for sound //
                        int placedObjectId = BuildingManager.Instance.placedObjectTypeSO.placedObjId; // to know which unit should be spawned
                        UnitsManager.Instance.waypoints[placedObjectId].Add(placedObject.transform);


                        BuildingManager.placedObjects[placedObjectId].Add(placedObject);
                        Debug.Log("placedObjects " + BuildingManager.placedObjects[placedObjectId].Count);
                        BuildingManager.Instance.DestroySurplusPlacedObjects();

                        OnObjectPlaced?.Invoke(placedObjectId);
                        OnChangedWaypoints?.Invoke();
                        BuildingManager.Instance.DeselectObjectType();
                    }

                    else
                    {
                        // Cannot build here
                        //UtilsClass.CreateWorldTextPopup("Cannot Build Here!", mousePosition);
                        //Debug.Log("Cannot Build Here!");
                        Bubble.Instance.CreatePopupText(mousePosition, "Cannot build here!");
                    }
                }

                if (BuildingManager.Instance.placedObjectTypeSO != null && blockPrefab.IsThisBlockWasHighlighted && blockState == LevelState.Pond)
                {
                    Bubble.Instance.CreatePopupText(mousePosition, "You can't build on a river...");
                }

                if (BuildingManager.Instance.placedObjectTypeSO != null && blockPrefab.IsThisBlockWasHighlighted && blockState == LevelState.Hill)
                {
                    Bubble.Instance.CreatePopupText(mousePosition, "Noway");
                }

            }
        }
    }

    bool CheckIfFitBlock(LevelState blockState,Vector3 mousePosition)
    {
        if (BuildingManager.Instance.placedObjectTypeSO != null)
        {
            if (blockPrefab.IsThisBlockWasHighlighted && blockState == LevelState.Desert && BuildingManager.Instance.placedObjectTypeSO.placedObjId != 0)
            {
                Bubble.Instance.CreatePopupText(mousePosition, "Noway");
                return false;
            }


        }

        if (BuildingManager.Instance.placedObjectTypeSO != null && blockPrefab.IsThisBlockWasHighlighted && blockState == LevelState.Forest)
        {
            if (BuildingManager.Instance.placedObjectTypeSO.placedObjId == 3 || BuildingManager.Instance.placedObjectTypeSO.placedObjId == 4 || BuildingManager.Instance.placedObjectTypeSO.placedObjId == 5)
            {
                Bubble.Instance.CreatePopupText(mousePosition, "Noway");
                return false;
            }
        }

        return true;
    }


    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
