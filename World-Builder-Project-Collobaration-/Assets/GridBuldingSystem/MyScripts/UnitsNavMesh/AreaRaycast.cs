using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaRaycast : MonoBehaviour
{
    public LayerMask unitMask;
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, unitMask))
            {
               
                    Debug.Log("RAY IN UNIT!");
                
            }
        }
    }
}
