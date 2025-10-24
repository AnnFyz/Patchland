using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles creation, positioning, and destruction of a placed object.
/// </summary>
public class PlacedObject_Done : MonoBehaviour
{
    [SerializeField] private Transform waypointCenter;
    public Action onDestroyedPlacedObject;
    static int index;
    public PlacedObjectTypeSO placedObjectTypeSO { get; private set; }
    public Vector2Int origin { get; private set; }
    private PlacedObjectTypeSO.Dir dir;
    public bool isDead = false;
    public static PlacedObject_Done Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO)
    {
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.placedObjectPrefab, worldPosition, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));
        placedObjectTransform.name += "_" + index.ToString();
        PlacedObject_Done placedObject = placedObjectTransform.GetComponent<PlacedObject_Done>();
        placedObject.Setup(placedObjectTypeSO, origin, dir);
        index++;
        return placedObject;
    }

    private void Setup(PlacedObjectTypeSO placedObjectTypeSO, Vector2Int origin, PlacedObjectTypeSO.Dir dir)
    {
        this.placedObjectTypeSO = placedObjectTypeSO;
        this.origin = origin;
        this.dir = dir;
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return placedObjectTypeSO.GetGridPositionList(origin, dir);
    }

    public Transform GetWaypointCenterWorldPosition()
    {
        return waypointCenter;
    }

    public void DestroySelf()
    {
        if(isDead) return;
        onDestroyedPlacedObject?.Invoke();
        Destroy(gameObject);
        isDead = true;
    }

    // when block is dead the dead version of the placed object is created
    public void CreateDeadCopyOfAnPlacedObject()
    {
        if (isDead) return;
        Transform deadCopy = Instantiate(placedObjectTypeSO.deadVersionOfplacedObject, transform.position, transform.rotation);
        Debug.Log("Created dead copy of placed object at position: " + transform.position);
        DestroySelf();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(waypointCenter.position, 0.2f);
    }

}
