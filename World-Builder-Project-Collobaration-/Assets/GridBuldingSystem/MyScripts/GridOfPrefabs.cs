using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridOfPrefabs : MonoBehaviour
{
    [SerializeField] GameObject blockPrefabObj;
    [SerializeField] int width = 3;
    [SerializeField] int height = 5;
    //public static GridOfPrefabs Instance { get; private set; }
    public static bool IsValidGridPos = false;
    private MyGridXZ<PrefabGridObject> grid;

    private void Awake()
    {
        //Instance = this;

    }

    private void Start()
    {
        grid = new MyGridXZ<PrefabGridObject>(width, height, 15f, Vector3.zero, (MyGridXZ<PrefabGridObject> g, int x, int y) => new PrefabGridObject(g, x, y));
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                BlockPrefab blockPrefab = BlockPrefab.Create(grid.GetWorldPosition(x, y), blockPrefabObj);
                grid.GetGridObject(x, y).SetPlacedObject(blockPrefab);
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            if (grid.GetGridObject(mousePosition) != null && IsValidGridPos)
            {
                // Valid Grid Position
                BlockPrefab placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
                if (placedObject != null)
                {
                    // Demolish
                    placedObject.DestroySelf();
                    grid.GetGridObject(mousePosition).ClearPlacedObject();

                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            if (grid.GetGridObject(mousePosition) != null && IsValidGridPos)
            {

                BlockPrefab placedObject = grid.GetGridObject(mousePosition).GetPlacedObject();
                if (placedObject != null)
                {
                    placedObject.ChangeHeight(3);
                    grid.GetGridObject(mousePosition).SetPlacedObject(placedObject);

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

        private MyGridXZ<PrefabGridObject> grid;
        private int x;
        private int y;
        private int value;

        private BlockPrefab blockPrefab;
        public PrefabGridObject(MyGridXZ<PrefabGridObject> grid, int x, int y)
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
            //return value.ToString();
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