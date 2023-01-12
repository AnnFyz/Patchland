using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList = null;
    public PlacedObjectTypeSO placedObjectTypeSO;
    public PlacedObjectTypeSO lastSelectedObjToPlaceTypeSO;
    public PlacedObjectTypeSO.Dir dir;
    public event EventHandler OnSelectedChanged; // for ghost building
    public event EventHandler OnObjectPlaced; // for sound 
    public static MyGridXZ<MyGridBuildingSystem.MyGridObject> grid;
    public static BlockPrefab blockPrefab;
    public List<Material> levelsMaterials = new List<Material>();
    private void Awake()
    {
        Instance = this;
        placedObjectTypeSO = null;// placedObjectTypeSOList[0];
    }
    private void OnEnable()
    {
      
    }
    public int GetNumberOfPlacedObjTypes()
    {
        return placedObjectTypeSOList.Count;
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            DeselectObjectType();
            lastSelectedObjToPlaceTypeSO = null;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); DeselectObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); DeselectObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { placedObjectTypeSO = placedObjectTypeSOList[2]; RefreshSelectedObjectType(); DeselectObjectType(); }
    }

    public void SelectResource(int resourceId)
    {
        placedObjectTypeSO = placedObjectTypeSOList[resourceId]; 
        RefreshSelectedObjectType();
        DeselectObjectType();
    }
    public void DeselectObjectType()
    {
        placedObjectTypeSO = null;
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RefreshSelectedObjectType()
    {
        if(placedObjectTypeSO == null)
        {
            placedObjectTypeSO = lastSelectedObjToPlaceTypeSO;
        }

        else if(placedObjectTypeSO != null)
        {
            lastSelectedObjToPlaceTypeSO = placedObjectTypeSO;
        }

        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }


    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        grid.GetXZ(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public Vector3 GetMouseWorldSnappedPosition()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        if(grid == null) return mousePosition;
        grid.GetXZ(mousePosition, out int x, out int z);

        if (placedObjectTypeSO != null)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        }
        else
        {
            return mousePosition;
        }
    }

    public Quaternion GetPlacedObjectRotation()
    {
        if (placedObjectTypeSO != null)
        {
            return Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0);
        }
        else
        {
            return Quaternion.identity;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO()
    {
        return placedObjectTypeSO;
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
