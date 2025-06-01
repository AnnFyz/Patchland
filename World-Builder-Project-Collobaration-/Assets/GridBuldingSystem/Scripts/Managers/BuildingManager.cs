using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable] 
public class BlockList
{
    public List<GameObject> healthyBlocks = new List<GameObject>();
    public List<GameObject> deadBlocks = new List<GameObject>();
}
public class BuildingManager : MonoBehaviour
{
    [SerializeField] LayerMask blockLayer;
    public BlockList blockList; // MAKE IT READ ONLY to keep track of healthy and dead blocks
    public AudioClip placedSound;
    public AudioSource audio;
    public static BuildingManager Instance { get; private set; }
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList = null;
    public Material deadBlockMaterial; // TO DO READ ONLY
    public PlacedObjectTypeSO placedObjectTypeSO;
    public PlacedObjectTypeSO lastSelectedObjToPlaceTypeSO;
    public PlacedObjectTypeSO.Dir dir;
    public event EventHandler OnSelectedChanged; // for ghost building
    public event EventHandler OnObjectPlaced; // for sound 
    public static MyGridXZ<MyGridBuildingSystem.MyGridObject> localGrid;
    public BlockPrefab currentBlockPrefab;
    public BlockPrefab lastBlockPrefab;
    public List<Material> levelsMaterials = new List<Material>();
    List<List<GridOfPrefabs.PrefabGridObject>> prefabGridObjects;
    public static List<List<PlacedObject_Done>> placedObjects = new List<List<PlacedObject_Done>>();
    public static bool CanBuildSelected = false;

    public void AddCreateHealtyBlock(GameObject block)
    {
        if (!blockList.healthyBlocks.Contains(block))
        {
            blockList.healthyBlocks.Add(block);
        }
    }
    public void TransferBlockToDeadList(GameObject block){
       if(blockList.healthyBlocks.Contains(block))
        {
            blockList.healthyBlocks.Remove(block);
        }
        if (!blockList.deadBlocks.Contains(block))
        {
            blockList.deadBlocks.Add(block);
        }      
    }
    private void Awake()
    {
        Instance = this;
        placedObjectTypeSO = null;// placedObjectTypeSOList[0];
        audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        for (int i = 0; i < GetNumberOfPlacedObjTypes(); i++) // to make a list for each type of placedObj
        {
            placedObjects.Insert(i, new List<PlacedObject_Done>());
        }
    }
    private void OnEnable()
    {


    }
    public void DestroySurplusPlacedObjects()
    {
        if (placedObjects[0].Count >= 5)
        {

            if (UnitsManager.Instance.waypoints[0].Contains(placedObjects[0][0].transform))
            {
                UnitsManager.Instance.waypoints[0].Remove(placedObjects[0][0].transform);
            }
            // ADD PARTICLES IN DESTROYSELF
            placedObjects[0][0].DestroySelf();
            placedObjects[0].RemoveAt(0);
        }

        if (placedObjects[1].Count >= 4)
        {

            if (UnitsManager.Instance.waypoints[1].Contains(placedObjects[1][0].transform))
            {
                UnitsManager.Instance.waypoints[1].Remove(placedObjects[1][0].transform);
            }
            // ADD PARTICLES IN DESTROYSELF
            placedObjects[1][0].DestroySelf();
            placedObjects[1].RemoveAt(0);
        }

        if (placedObjects[2].Count >= 3)
        {

            if (UnitsManager.Instance.waypoints[1].Contains(placedObjects[2][0].transform))
            {
                UnitsManager.Instance.waypoints[1].Remove(placedObjects[2][0].transform);
            }
            // ADD PARTICLES IN DESTROYSELF
            placedObjects[2][0].DestroySelf();
            placedObjects[2].RemoveAt(0);
        }

        if (placedObjects[3].Count >= 5)
        {

            if (UnitsManager.Instance.waypoints[1].Contains(placedObjects[2][0].transform))
            {
                UnitsManager.Instance.waypoints[1].Remove(placedObjects[2][0].transform);
            }
            // ADD PARTICLES IN DESTROYSELF
            placedObjects[3][0].DestroySelf();
            placedObjects[3].RemoveAt(0);
        }
        if (placedObjects[4].Count >= 5)
        {

            if (UnitsManager.Instance.waypoints[1].Contains(placedObjects[2][0].transform))
            {
                UnitsManager.Instance.waypoints[1].Remove(placedObjects[2][0].transform);
            }
            // ADD PARTICLES IN DESTROYSELF
            placedObjects[4][0].DestroySelf();
            placedObjects[3].RemoveAt(0);
        }
        if (placedObjects[5].Count >= 5)
        {

            if (UnitsManager.Instance.waypoints[1].Contains(placedObjects[2][0].transform))
            {
                UnitsManager.Instance.waypoints[1].Remove(placedObjects[2][0].transform);
            }
            // ADD PARTICLES IN DESTROYSELF
            placedObjects[4][0].DestroySelf();
            placedObjects[3].RemoveAt(0);
        }
    }


    public int GetNumberOfPlacedObjTypes()
    {
        return placedObjectTypeSOList.Count;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            DeselectObjectTypeOnSelectedAnotherBlockType();
            //DeselectObjectType();
            //lastSelectedObjToPlaceTypeSO = null;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            dir = PlacedObjectTypeSO.GetNextDir(dir);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && currentBlockPrefab.GetComponent<LocalLevelState>().GetCurrentLevelState() != LevelState.Pond) { placedObjectTypeSO = placedObjectTypeSOList[0]; RefreshSelectedObjectType(); DeselectObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LevelState levelState = currentBlockPrefab.GetComponent<LocalLevelState>().GetCurrentLevelState();
            if (levelState != LevelState.Pond && levelState != LevelState.Desert)
            {
                placedObjectTypeSO = placedObjectTypeSOList[1]; RefreshSelectedObjectType(); DeselectObjectType();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            LevelState levelState = currentBlockPrefab.GetComponent<LocalLevelState>().GetCurrentLevelState();
            if (levelState != LevelState.Pond && levelState != LevelState.Desert)
            {
                placedObjectTypeSO = placedObjectTypeSOList[2]; RefreshSelectedObjectType(); DeselectObjectType();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            LevelState levelState = currentBlockPrefab.GetComponent<LocalLevelState>().GetCurrentLevelState();
            if (levelState == LevelState.Mountain || levelState == LevelState.SnowMountain)
            {
                placedObjectTypeSO = placedObjectTypeSOList[3]; RefreshSelectedObjectType(); DeselectObjectType();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            LevelState levelState = currentBlockPrefab.GetComponent<LocalLevelState>().GetCurrentLevelState();
            if (levelState == LevelState.Mountain || levelState == LevelState.SnowMountain)
            {
                placedObjectTypeSO = placedObjectTypeSOList[4]; RefreshSelectedObjectType(); DeselectObjectType();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            LevelState levelState = currentBlockPrefab.GetComponent<LocalLevelState>().GetCurrentLevelState();
            if (levelState == LevelState.Mountain || levelState == LevelState.SnowMountain)
            {
                placedObjectTypeSO = placedObjectTypeSOList[5]; RefreshSelectedObjectType(); DeselectObjectType();
            }
        }

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

    public void DeselectObjectTypeOnSelectedAnotherBlockType()
    {
        placedObjectTypeSO = null;
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
        lastSelectedObjToPlaceTypeSO = null;
    }

    public void RefreshSelectedObjectType()
    {
        if (placedObjectTypeSO == null)
        {
            placedObjectTypeSO = lastSelectedObjToPlaceTypeSO;
        }

        else if (placedObjectTypeSO != null)
        {
            lastSelectedObjToPlaceTypeSO = placedObjectTypeSO;
        }

        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }


    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        localGrid.GetXZ(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

    public Vector3 GetMouseWorldSnappedPosition()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        if (localGrid == null) return mousePosition;
        localGrid.GetXZ(mousePosition, out int x, out int z);
        x = Math.Clamp(x, 0, localGrid.GetWidth() - 1);
        z = Math.Clamp(z, 0, localGrid.GetHeight() - 1);

        if (placedObjectTypeSO != null)
        {
            Vector2Int rotationOffset = placedObjectTypeSO.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = localGrid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * localGrid.GetCellSize();
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
            //Debug.Log(raycastHit.collider.name);
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;

        }
    }
}
