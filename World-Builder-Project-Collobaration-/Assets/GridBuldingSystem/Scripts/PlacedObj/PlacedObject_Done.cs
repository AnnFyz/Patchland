using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlacedObject_Done : MonoBehaviour
{

    [SerializeField] MeshRenderer[] materials;
    public Action onDestroyedPlacedObject;
    static int index;
    public PlacedObjectTypeSO placedObjectTypeSO { get; private set; }
    private Vector2Int origin;
    private PlacedObjectTypeSO.Dir dir;
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

    public void DestroySelf()
    {
        onDestroyedPlacedObject?.Invoke();
        Destroy(gameObject);
    }

    // when block is dead change material of placed object to dead material
    public void ChangeMaterialOfObject()
    {

    }

    private void FixedUpdate()
    {
        if (!UnitsManager.Instance.waypointsForPlacedObjects[placedObjectTypeSO.placedObjectName].Contains(this.gameObject.transform))
        {
            DestroySelf();
        }
    }
}
