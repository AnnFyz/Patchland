using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField]GameObject cubePref;
    void Start()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
               Vector3 position =  new Vector3(x, 0, z);
               GameObject currentObj =  Instantiate(cubePref, position, Quaternion.identity); //cubePref.gameObject.GetComponent<Renderer>().bounds.size/2
               CubeGenerator currentCubeGenerator = currentObj.GetComponent<CubeGenerator>();

               float perlinValue = Mathf.PerlinNoise(position.x * 0.1f, position.y*0.1f) * 5;
               currentCubeGenerator.animSpeed = perlinValue;
               currentCubeGenerator.yAmplitude = perlinValue;
            }
        }
    }

}
