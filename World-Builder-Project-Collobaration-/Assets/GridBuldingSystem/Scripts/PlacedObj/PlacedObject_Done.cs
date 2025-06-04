using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlacedObject_Done : MonoBehaviour {

   [SerializeField] Material material;
   static Transform visual;
   public Action onDestroyedPlacedObject;
    public static PlacedObject_Done Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO) {
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.placedObjectPrefab, worldPosition, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir), 0));
        PlacedObject_Done placedObject = placedObjectTransform.GetComponent<PlacedObject_Done>();
        placedObject.Setup(placedObjectTypeSO, origin, dir);
        visual = placedObjectTypeSO.visualForGhostPlacedObject;
        return placedObject;
    }




    public PlacedObjectTypeSO placedObjectTypeSO;
    private Vector2Int origin;
    private PlacedObjectTypeSO.Dir dir;

    private void Setup(PlacedObjectTypeSO placedObjectTypeSO, Vector2Int origin, PlacedObjectTypeSO.Dir dir) {
        this.placedObjectTypeSO = placedObjectTypeSO;
        this.origin = origin;
        this.dir = dir;
        material = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Renderer>().material;
    }

    public List<Vector2Int> GetGridPositionList() {
        return placedObjectTypeSO.GetGridPositionList(origin, dir);
    }

    public void DestroySelf() {
        onDestroyedPlacedObject?.Invoke();
        Destroy(gameObject);
    }

    public void ChangeMaterialOfObject()
    {
        material = placedObjectTypeSO.materialForDeadObj;
        transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Renderer>().material = material;
        transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!UnitsManager.Instance.waypoints[placedObjectTypeSO.placedObjId].Contains(this.gameObject.transform))
        {
            DestroySelf();
        }
    }
}
