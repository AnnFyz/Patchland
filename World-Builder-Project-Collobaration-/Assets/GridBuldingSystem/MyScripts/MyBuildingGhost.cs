using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBuildingGhost : MonoBehaviour
{
    private Transform visual;
    private PlacedObjectTypeSO placedObjectTypeSO;

    private void Start()
    {
        RefreshVisual();

        BuildingManager.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    private void Instance_OnSelectedChanged(object sender, System.EventArgs e)
    {
        RefreshVisual();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("visual.localScale.y " + visual.localScale.y);
            Debug.Log("visual.localPosition " + visual.localPosition);
            Debug.Log("BuildingManager.blockPrefab.GetStartScale() " + BuildingManager.blockPrefab.GetStartScale());
        }
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = BuildingManager.Instance.GetMouseWorldSnappedPosition();
        targetPosition.y = 1f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);

        transform.rotation = Quaternion.Lerp(transform.rotation, BuildingManager.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);
    }

    private void RefreshVisual()
    {
        if (visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
        }

        PlacedObjectTypeSO placedObjectTypeSO = BuildingManager.Instance.GetPlacedObjectTypeSO();

        if (placedObjectTypeSO != null)
        {
            visual = Instantiate(placedObjectTypeSO.visual, Vector3.zero, Quaternion.identity);
            visual.parent = transform;
            //visual.localPosition = GhostBildingPosition();
            float newGhostBuldingsPos = (visual.localPosition.y * BuildingManager.blockPrefab.GetNewHeight() * BuildingManager.blockPrefab.GetStartScale() * -1) - BuildingManager.blockPrefab.GetStartScale();
            if(BuildingManager.blockPrefab.GetNewHeight() == 0)
            {
                visual.localPosition = Vector3.zero;
            }
            else
            {
                visual.localPosition = new Vector3(0, newGhostBuldingsPos, 0);
            }
            visual.localEulerAngles = Vector3.zero;
            SetLayerRecursive(visual.gameObject, 11);
        }
    }

    private void SetLayerRecursive(GameObject targetGameObject, int layer)
    {
        targetGameObject.layer = layer;
        foreach (Transform child in targetGameObject.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

  

}
