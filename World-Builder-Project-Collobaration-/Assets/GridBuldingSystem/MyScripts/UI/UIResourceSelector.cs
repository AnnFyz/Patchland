using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIResourceSelector : MonoBehaviour
{
    public void SelectResource(int resourceId)
    {
        BuildingManager.Instance.SelectResource(resourceId);
    }
}
