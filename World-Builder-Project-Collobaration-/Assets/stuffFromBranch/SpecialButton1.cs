using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialButton1 : MonoBehaviour
{
    public List<GameObject> objectToSpawn = new List<GameObject>();

    public bool isRandomized;
    public float degrees = 360f;
    public float radius = 10f;
    public float numberofSpawn = 10f;
    public Vector3 Offset = new Vector3(0, -1, 0);

    public float price1 = 50;
    public float price2 = 100;
    void Awake()
    {
        isRandomized = true;
    }

    public void SpawnObjects()
    {
        //
        int index = isRandomized ? Random.Range(0, objectToSpawn.Count) : 0;
        Vector3 randomSpawnPoint = new Vector3(Random.Range(-20, 20), -20, Random.Range(-20,20));
        if(objectToSpawn.Count > 0)
        {
            Instantiate(objectToSpawn[index], randomSpawnPoint, Quaternion.identity);
        }

    }
    public void SpawnAtRadius()
    {
        
      float arclenght = (degrees/360) * 2 * Mathf.PI;  
      float nextAngle = arclenght / numberofSpawn;
      float angle = 0;
      for (int i = 0; i < numberofSpawn; i++)
      {
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sign(angle) * radius;
         int index = isRandomized ? Random.Range(0, objectToSpawn.Count) : 0;
        Vector3 randomSpawnPoint = new Vector3(Random.Range(-20, 20), 3, Random.Range(-20,20));
         Instantiate(objectToSpawn[index], randomSpawnPoint + Offset, Quaternion.identity);
       
        

       // var obj = Instantiate(objectToSpawn[index], )
        angle += nextAngle;

      }
      
    }
}
