using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusTree : MonoBehaviour
{
    public float flowerSpawnTime = 0;
    public float flowerSpawnRate = 90;

    [SerializeField] private GameObject flowerSpawn;

    [SerializeField] Transform flowerParent;

    public int flowerOffSpring = 1;

    public static int flowerStartPopulation = 5;

    public int maxFlower = 1;
    public int currFlower;

    public float witherTime = 0;
    public float witherLimit = 100;
    public bool withering = false;

     public bool isOnSand;

    public bool isOnRock;

    public bool isOnGrass;

  



    private void Start()
    {
        flowerSpawnRate = Random.Range(90, 300);
       // spawner = GameObject.Find("PlantSpawnerSO").GetComponent<SOplantSpawner>();
        
    }

    void Update()
    {
        currFlower = (flowerParent.transform.childCount);


        if (FertileGround.fertile == false) //Ground.sand
        {
            witherTime += Time.deltaTime;

            if (withering == false)
            {
                if (isOnGrass == true) // Ground.grass
                {
                    witherLimit = Random.Range(5, 60);
                    withering = true;
                }
                if (isOnRock == true) //Ground.stone
                {
                    witherLimit = Random.Range(5, 90);
                    withering = true;
                }
            }

        }

        if (FertileGround.fertile == true) // Ground.sand
        {
            witherTime = 0;

            if (withering == true)
            {
                withering = false;
            }

            if (currFlower < maxFlower)
            {
                flowerSpawnTime += Time.deltaTime;

                if (flowerSpawnTime >= flowerSpawnRate)
                {
                    flowerSpawnRate = Random.Range(90, 300);

                    for (int i = 0; i < flowerOffSpring; i++)
                    {
                        Vector3 position = new Vector3(transform.position.x, 11, transform.position.z);
                        Vector3 rotation = new Vector3(0, 0, 0);

                        Instantiate(flowerSpawn, position, Quaternion.Euler(rotation), flowerParent);

                    }
                    flowerSpawnTime = 0;
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
        if(other.gameObject.CompareTag("grass"))
        {
            isOnGrass = true;
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
        if(other.gameObject.CompareTag("grass"))
        {
            isOnGrass = false;
        }
        
        
    }
   
}
