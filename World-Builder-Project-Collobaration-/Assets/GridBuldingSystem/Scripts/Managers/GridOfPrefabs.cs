using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using Unity.AI.Navigation;
using UnityEngine.EventSystems;


public class GridOfPrefabs : MonoBehaviour
{
    [SerializeField] GameObject blockPrefabMain;
    [SerializeField] GameObject blockPrefabForCorners;
    private GameObject prefabToCreate;
    private Quaternion prefabRotation;
    [SerializeField] float cellSize = 15f;
    [SerializeField] int width = 3;
    [SerializeField] int height = 5;
    public int Width => width;
    public int Height => height;

    [SerializeField] float amountScale = 40.0f;
    [SerializeField] float xScale = 16.0f;
    public static GridOfPrefabs Instance { get; private set; }
    public static bool IsValidGridPos = false;
    GridXZ<PrefabGridObject> globalGrid;
    private NavMeshSurface[] navMeshSurfaces;
    public static event Action OnGridReady;
    public static Bounds Bounds;
    public GameObject[] points = new GameObject[4];
    Vector3 p1, p2, p3, p4;
    private void Awake()
    {
        // Singleton pattern to ensure only one instance of GridOfPrefabs exists
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        navMeshSurfaces = GetComponents<NavMeshSurface>();
    }
    private void OnEnable()
    {
        UIManager.Instance.OnChangedGrid += RebuildNavMesh;
    }

    private void Start()
    {
        BuildGrid();
        RebuildNavMesh();
    }


    private void RebuildNavMesh()
    {
        for (int i = 0; i < navMeshSurfaces.Length; i++)
        {
            navMeshSurfaces[i].BuildNavMesh();
        }

    }

    void BuildGrid()
    {
        globalGrid = new GridXZ<PrefabGridObject>(width, height, cellSize, Vector3.zero, (GridXZ<PrefabGridObject> g, int x, int y) => new PrefabGridObject(g, x, y), false, CornerBlock.None);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 && y == 0)
                {
                    prefabToCreate = blockPrefabForCorners;
                    prefabRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    prefabToCreate.GetComponent<BlockPrefab>().cornerBlock = CornerBlock.BottomLeft;
                }
                else if (x == 0 && y == height - 1)
                {
                    prefabToCreate = blockPrefabForCorners;
                    prefabRotation = Quaternion.Euler(new Vector3(0, -90, 0));
                    prefabToCreate.GetComponent<BlockPrefab>().cornerBlock = CornerBlock.TopLeft;
                }
                else if (x == width - 1 && y == height - 1)
                {
                    prefabToCreate = blockPrefabForCorners;
                    prefabRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    prefabToCreate.GetComponent<BlockPrefab>().cornerBlock = CornerBlock.TopRight;
                }
                else if (x == width - 1 && y == 0)
                {
                    prefabToCreate = blockPrefabForCorners;
                    prefabRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    prefabToCreate.GetComponent<BlockPrefab>().cornerBlock = CornerBlock.BottomRight;
                }
                else
                {
                    prefabToCreate = blockPrefabMain;
                    prefabRotation = Quaternion.identity;
                }
                BlockPrefab blockPrefab = BlockPrefab.Create(globalGrid.GetWorldPosition(x, y), prefabToCreate, prefabRotation);
                BuildingManager.Instance.blockList.AddCreatedHealtyBlock(blockPrefab.gameObject);
                blockPrefab.blockId = new Vector2(x, y);
                blockPrefab.GetComponent<GridBuildingSystem>().SetBlockGrid();
                blockPrefab.currentBlocksAmount = 1;
                blockPrefab.DeactivateStackOfBlocks();
                blockPrefab.gameObject.transform.parent = gameObject.transform;
                globalGrid.GetGridObject(x, y).SetPlacedObject(blockPrefab);
                blockPrefab.gameObject.transform.parent = gameObject.transform;
                globalGrid.GetGridObject(x, y).SetPlacedObject(blockPrefab);
                float amount = amountScale * Mathf.PerlinNoise(UnityEngine.Random.Range(0.1f, 10) * xScale, 0.0f);
                for (int i = 0; i < Mathf.RoundToInt(amount); i++)
                {
                    blockPrefab.InitializeBlockHeight(1);
                }
            }
        }

        Vector3 offset = BlockPrefab.Offset / 2f;

        // bottom-left corner of the grid
        p1 = globalGrid.GetWorldPosition(0, 0) - offset;

        // top-left outer corner
        p2 = globalGrid.GetWorldPosition(0, 0) + new Vector3(0f, 0f, height * cellSize) - offset;


        // top-right outer corner
        p3 = globalGrid.GetWorldPosition(0, 0) + new Vector3(width * cellSize, 0f, height * cellSize) - offset;

        // bottom-right outer corner
        p4 = globalGrid.GetWorldPosition(width - 1, 0) - offset;

        points[0].transform.position = p1;
        points[1].transform.position = p2;
        points[2].transform.position = p3;
        points[3].transform.position = p4;

        CalculateBoundsFromPoints(p1, p2, p3, p4);
        OnGridReady?.Invoke();
    }
    public Vector3 GetCenterOnGridSurface()
    {
        Vector3 centerOfGrid = new Vector3(GetCenterObjInGrid().position.x, GetCenterObjInGrid().position.y + (BlockPrefab.Offset.y * -1) + 0.25f, GetCenterObjInGrid().position.z);
        Debug.Log("Center of Grid on Surface: " + centerOfGrid);
        return centerOfGrid;
    }
    public Transform GetCenterObjInGrid()
    {
        int halfWidth = Mathf.RoundToInt(width / 2);
        int halfHeight = Mathf.RoundToInt(height / 2);
        return globalGrid.GetGridObject(halfWidth, halfHeight).GetPlacedObject().transform;
    }

    private Bounds CalculateBoundsFromPoints(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        Bounds = new Bounds(p1, Vector3.zero);
        Bounds.Encapsulate(p2);
        Bounds.Encapsulate(p3);
        Bounds.Encapsulate(p4);
        return Bounds;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 9999f))
            {

                if (raycastHit.collider.gameObject.GetComponentInParent<BlockPrefab>())
                {
                    BlockPrefab placedObject = raycastHit.collider.gameObject.GetComponentInParent<BlockPrefab>();
                    for (int x = 0; x < width; x++)
                    {
                        for (int z = 0; z < height; z++)
                        {
                            globalGrid.GetGridObject(x, z).GetPlacedObject().isSelected = false;
                            UIManager.Instance.HidePanels();
                            placedObject.GetComponent<RaycastHandler>().SetOutline(false);
                        }
                    }
                    if (!placedObject.GetComponent<BlockHealth>().IsBlockDead)
                    {
                        placedObject.isSelected = true;
                        placedObject.GetComponent<RaycastHandler>().SetOutline(true);
                        UIManager.Instance.ShowPanels();
                        UIManager.Instance.prefabsState = placedObject.GetComponent<LocalLevelState>();
                        UIManager.Instance.LocalSetupUIIcons();
                    }
                    else
                    {
                        Bubble.Instance.CreatePopupText(GetMouseWorldPosition(), " I am dead.. ☹ ");
                    }

                }
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f))
        {

            IsValidGridPos = true;
            return raycastHit.point;
        }
        else
        {
            IsValidGridPos = false;
            return Vector3.zero;

        }
    }

    public class PrefabGridObject
    {

        private const int MIN = 0;
        private const int MAX = 255;

        private GridXZ<PrefabGridObject> grid;
        private int x;
        private int y;
        private int value;

        private BlockPrefab blockPrefab;
        public PrefabGridObject(GridXZ<PrefabGridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            blockPrefab = null;
        }

        public void ChangeValue(int addValue)
        {
            value += addValue;
            value = Mathf.Clamp(value, MIN, MAX);
            grid.TriggerGridObjectChanged(x, y);
        }


        public float GetValueNormalized()
        {
            return (float)value / MAX;
        }

        public override string ToString()
        {
            return x + ", " + y + "\n" + blockPrefab;
        }

        public void SetPlacedObject(BlockPrefab blockPrefab)
        {
            this.blockPrefab = blockPrefab;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void ClearPlacedObject()
        {
            blockPrefab = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public BlockPrefab GetPlacedObject()
        {
            return blockPrefab;
        }

        public bool CanBuild()
        {
            return blockPrefab == null;
        }
    }


}