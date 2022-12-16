using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    public float berrySpawnTime = 0;
    public float berrySpawnRate = 5;
    
    [SerializeField] private GameObject berrySpawn;

    [SerializeField] Transform berryParent;


    public int berryOffSpring = 5;

    public static int berryStartPopulation = 5;

    public int maxBerries = 15;
    public int currBerries;

    public float witherTime = 0;
    public float witherLimit = 100;
    public bool withering = false;
    public bool isOnSand;
    public bool isOnRock;

    void Start()
    {

    }

    
    void Update()
    {
        currBerries = (berryParent.transform.childCount);

        if (FertileGround.fertile == false)
        {
            witherTime += Time.deltaTime;

            if (withering == false)
            {
                if (Ground.sand == true)
                {
                    witherLimit = Random.Range(10, 30);
                    withering = true;
                }
                if (Ground.stone == true)
                {
                    witherLimit = Random.Range(5, 15);
                    withering = true;
                }
            }

        }


        if (FertileGround.fertile == true)
        {
            witherTime = 0;

            if (withering == true)
            {
                withering = false;
            }

            if (currBerries <= maxBerries)
            {
                berrySpawnTime += Time.deltaTime;

                if (berrySpawnTime >= berrySpawnRate)
                {
                    berrySpawnRate = Random.Range(5, 20);
                    berryOffSpring = Random.Range(1, 5);

                    for (int i = 0; i < berryOffSpring; i++)
                    {
                        Vector3 position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                        Vector3 rotation = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

                        Instantiate(berrySpawn, position, Quaternion.Euler(rotation), berryParent);

                    }
                    berrySpawnTime = 0;
                }
            }
        }




        if (witherTime >= witherLimit)
        {
            Destroy(gameObject);
            Debug.Log("tree died");
        }
    } 
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("sand"))
        {
            isOnSand = true;
        }
        if(other.gameObject.CompareTag("rock"))
        {
            isOnRock = true;
        }
        
    }
     void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("sand"))
        {
            isOnSand = false;
        }
          if(other.gameObject.CompareTag("rock"))
        {
            isOnRock = false;
        }
        
    }
}
