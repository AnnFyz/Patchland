using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public float leafSpawnTime = 0;
    public float leafSpawnRate = 5;

    [SerializeField] private GameObject leafSpawn;

    [SerializeField] Transform leafParent;

    public int leafOffSpring = 1;

    public static int leafStartPopulation = 5;

    public float leaves;

    public int maxLeaves = 5;
    public int currLeaves;

    public float witherTime = 0;
    public float witherLimit = 100;
    public bool withering = false;
     public bool isOnSand;

    public bool isOnRock;

    public bool isOnGrass;

    void Start()
    {
        
    }

    void Update()
    {

        currLeaves = (leafParent.transform.childCount);

        if (FertileGround.fertile == false) // Ground.grass
        {
            witherTime += Time.deltaTime;

            if (withering == false)
            {
                if ( isOnSand == true) //Ground.sand
                {
                    witherLimit = Random.Range(15, 60);
                    withering = true;
                }
                if (isOnRock == true) //Ground.stone
                {
                    witherLimit = Random.Range(5, 30);
                    withering = true;
                }
            }
            
        }

        if (FertileGround.fertile == true) //Ground.grass
        {
            witherTime = 0;

            if (withering == true)
            {
                withering = false;
            }

            if (currLeaves <= maxLeaves)
            {
                leafSpawnTime += Time.deltaTime;

                if (leafSpawnTime >= leafSpawnRate)
                {
                    leafSpawnRate = Random.Range(10, 30);
                    leafOffSpring = Random.Range(1, 3);

                    for (int i = 0; i < leafOffSpring; i++)
                    {
                        Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                        Vector3 rotation = new Vector3(0, Random.Range(0f, 360f), 0);

                        Instantiate(leafSpawn, position, Quaternion.Euler(rotation), leafParent);

                    }
                    leafSpawnTime = 0;
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
