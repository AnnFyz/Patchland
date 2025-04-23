using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MyGridBuildingSystem : MonoBehaviour
{
    public MyGridXZ<MyGridObject> grid;
    public MyGridXZ<MyGridObject> oldGrid;
    [SerializeField] int gridWidth = 3;
    [SerializeField] int gridHeight = 3;
    [SerializeField] float cellSize = 5f;
    BlockPrefab blockPrefab;
    public Vector3 origin;
    public event EventHandler OnObjectPlaced; // for sound 
    //public event EventHandler OnSelectedChanged; // for ghost building
    private void Awake()
    {
        origin = transform.position;
        blockPrefab = GetComponent<BlockPrefab>(); 
        grid = new MyGridXZ<MyGridObject>(gridWidth, gridHeight, cellSize, origin - BlockPrefab.offset, (MyGridXZ<MyGridObject> g, int x, int y) => new MyGridObject(g, x, y));
        blockPrefab.OnHeightChanged += UpdateGrid;
        blockPrefab.OnHeightChanged += DeleteOldObjects;
    }

    public void UpdateGrid(int newHeight)
    {
        oldGrid = grid;
        grid = new MyGridXZ<MyGridObject>(gridWidth, gridHeight, cellSize, new Vector3(origin.x - BlockPrefab.offset.x, (-newHeight * BlockPrefab.offset.y) + BlockPrefab.offset.y, origin.z - BlockPrefab.offset.z), (MyGridXZ<MyGridObject> g, int x, int y) => new MyGridObject(g, x, y));
    }

    public void DeleteOldObjects(int newHeight)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
               if (oldGrid.GetGridObject(x, z) != null && oldGrid.GetGridObject(x, z).GetPlacedObject() != null)
                {
                    oldGrid.GetGridObject(x, z).GetPlacedObject().DestroySelf();
                    grid.GetGridObject(x, z).ClearPlacedObject();
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

        if (Input.GetMouseButtonDown(0) && BuildingManager.Instance.placedObjectTypeSO != null && blockPrefab.IsThisBlockWasSelected) 
        {
            Vector3 mousePosition = GetMouseWorldPosition();
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

                //OnObjectPlaced?.Invoke(this, EventArgs.Empty); // for sound

                //DeselectObjectType();
            }
            else
            {
                // Cannot build here
                //UtilsClass.CreateWorldTextPopup("Cannot Build Here!", mousePosition);
                Debug.Log("Cannot Build Here!");
            }
        }

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    dir = PlacedObjectTypeSO.GetNextDir(dir);
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); Debug.Log("First org form selected"); }
        //if (Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); Debug.Log("Second building selected"); }
        //if (Input.GetKeyDown(KeyCode.Alpha3)) { placedObjectTypeSO = placedObjectTypeSOList[2]; RefreshSelectedObjectType(); Debug.Log("Third building selected"); }
    }

    //private void DeselectObjectType()
    //{
    //    placedObjectTypeSO = null; RefreshSelectedObjectType();
    //}

    //private void RefreshSelectedObjectType()
    //{
    //    OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    //}


    //public Vector2Int GetGridPosition(Vector3 worldPosition)
    //{
    //    grid.GetXZ(worldPosition, out int x, out int z);
    //    return new Vector2Int(x, z);
    //}

    //public Vector3 GetMouseWorldSnappedPosition()
    //{
    //    Vector3 mousePosition = GetMouseWorldPosition();
    //    grid.GetXZ(mousePosition, out int x, out int z);

    //    if (placedObjectTypeSO != null)
    //    {
    //        Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
    //        Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
    //        return placedObjectWorldPosition;
    //    }
    //    else
    //    {
    //        return mousePosition;
    //    }
    //}

    //public Quaternion GetPlacedObjectRotation()
    //{
    //    if (placedObjectTypeSO != null)
    //    {
    //        return Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
    //    }
    //    else
    //    {
    //        return Quaternion.identity;
    //    }
    //}

    //public PlacedObjectTypeSO GetPlacedObjectTypeSO()
    //{
    //    return placedObjectTypeSO;
    //}

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
