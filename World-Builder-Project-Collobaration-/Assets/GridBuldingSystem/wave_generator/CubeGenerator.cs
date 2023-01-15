using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGenerator : MonoBehaviour
{
    [Range(0f, 20)] public float animSpeed;
    [SerializeField][Range(0.5f,5)] float xSize;
    [Range(0.5f, 5)] public float yAmplitude;
    [SerializeField][Range(0.5f, 5)] float zSize;
    [SerializeField] Material myMaterial;
    MeshFilter myMeshFilter;
    MeshRenderer myRenderer;
    void Start()
    {
        myRenderer = gameObject.AddComponent<MeshRenderer>();
        myMeshFilter = gameObject.AddComponent<MeshFilter>();
        myMeshFilter.mesh = GenerateCube(xSize, yAmplitude,zSize);
        myRenderer.material = myMaterial;
    }

    private void FixedUpdate()
    {
        float t = (Mathf.Sin(Time.time * animSpeed) +1)/2 * 5;
        float yAnim = ((Mathf.Sin(t) +1)/2) * yAmplitude;
        myMeshFilter.mesh = GenerateCube(xSize, yAnim, zSize);
    }

    Mesh GenerateCube(float x, float y, float z)
    {
        Mesh myMesh = new Mesh();

        Vector3[] vL = new Vector3[8]
        {
            new Vector3 (0,0,0),
            new Vector3 (0,y,0),
            new Vector3 (x,y,0),
            new Vector3 (x,0,0),
            new Vector3 (0,0,z),
            new Vector3 (0,y,z),
            new Vector3 (x,y,z),
            new Vector3 (x,0,z),
        };

        Vector3 midPoint = new Vector3(x / 2, y, z / 2);
        Vector3 midPoint2 = Vector3.Lerp(vL[1], vL[6], 0.5f);

        float t = (Mathf.Sin(Time.time) + 1) / 2;
        vL[1] = Vector3.Lerp(vL[1], midPoint2, t);
        vL[2] = Vector3.Lerp(vL[2], midPoint2, t);
        vL[5] = Vector3.Lerp(vL[5], midPoint2, t);
        vL[6] = Vector3.Lerp(vL[6], midPoint2, t);

        Vector3[] myVertices = new Vector3[24]
        {
            //backside
            vL[0], vL[1], vL[2], vL[3], 
            //right
            //4     5       6      7
            vL[3], vL[2], vL[6], vL[7],
            //up
            //8     9      10     11
            vL[1], vL[5], vL[6], vL[2],
            //front
            vL[7], vL[6], vL[5], vL[4],
            //left
            vL[4], vL[5], vL[1], vL[0],
            //down
             vL[3], vL[7], vL[4], vL[0]
         };

        // assign vertices to cube 
        myMesh.vertices = myVertices;

        // recalculate normals

        int[] myTriangles = new int[36]
        {
            //Back
            0,1,2,
            2,3,0,
            // Right
            4,5,6,
            6,7,4,
            //up
            8,9,10,
            10,11,8,
            //Front
            12,13,14,
            14,15,12,
            //Left
            16,17,18,
            18,19,16,
            //Down
            20,21,22,
            22,23,20
        };

        //int[] myTriangles = new int[36]
        //{
        //    //Back
        //    0,1,2,
        //    2,3,0,
        //    // Right
        //    3,2,6,
        //    6,7,3,
        //    //Up
        //    2,1,6,
        //    6,1,5,
        //    //Front
        //    7,5,4,
        //    7,6,5,
        //    //Left
        //    5,1,0,
        //    0,4,5,
        //    //Down
        //    0,3,4,
        //    3,7,4
        //};

        //assign triangle

        myMesh.triangles = myTriangles;
        myMesh.RecalculateNormals();
        return myMesh;
        //myMeshFilter.mesh = myMesh;
        //myRenderer.material = myMaterial;


    }
}

