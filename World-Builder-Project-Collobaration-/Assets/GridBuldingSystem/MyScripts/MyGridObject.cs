using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGridObject : MonoBehaviour
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

