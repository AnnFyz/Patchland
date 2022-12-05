using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class navMeshManager : MonoBehaviour
{
    [SerializeField] NavMeshSurface[] surfaces;

    void Start()
    {
        for (int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].BuildNavMesh();
        }
    }


    void Update()
    {
        if (Input.GetKeyDown("m"))
        {
            for (int i = 0; i < surfaces.Length; i++)
            {
                surfaces[i].BuildNavMesh();
                Debug.Log("baked");
            }
        }
    }

    public void ReBakeMesh()
    {
        for (int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].BuildNavMesh();
            Debug.Log("baked");
        }
    }
}
