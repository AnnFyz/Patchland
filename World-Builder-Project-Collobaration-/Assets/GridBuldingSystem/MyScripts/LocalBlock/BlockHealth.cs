using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHealth : MonoBehaviour
{
    public Vector3 CenterOfBlock { get; set; }
    Collider m_Collider;
    private void Awake()
    {
        m_Collider = GetComponentInChildren<Collider>();
    }

    private void Start()
    {
        CenterOfBlock = m_Collider.bounds.center;
    }

}
