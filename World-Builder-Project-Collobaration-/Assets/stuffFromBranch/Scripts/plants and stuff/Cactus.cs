using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cactus : MonoBehaviour
{
    public float fruitSpawnTime = 0;
    public float fruitSpawnRate = 5;

    [SerializeField] private GameObject fruitSpawn;

    [SerializeField] Transform fruitParent;

    public int fruitOffSpring = 1;

    public static int fruitStartPopulation = 5;

    public int maxFruits = 5;
    public int currFruits;

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

        currFruits = (fruitParent.transform.childCount);

        if (FertileGround.fertile == false) // Ground.sand
        {
            witherTime += Time.deltaTime;

            if (withering == false)
            {
                if ( isOnGrass == true) // Ground.grass
                {
                    witherLimit = Random.Range(5, 60);
                    withering = true;
                }
                if (  isOnRock == true) //Ground.stone
                {
                    witherLimit = Random.Range(5, 90);
                    withering = true;
                }
            }

        }

        if (FertileGround.fertile == true) //Ground.sand == true
        {
            witherTime = 0;

            if (withering == true)
            {
                withering = false;
            }

            if (currFruits <= maxFruits)
            {
                fruitSpawnTime += Time.deltaTime;

                if (fruitSpawnTime >= fruitSpawnRate)
                {
                    fruitSpawnRate = Random.Range(60, 90);
                    fruitOffSpring = Random.Range(1, 2);

                    for (int i = 0; i < fruitOffSpring; i++)
                    {
                        Vector3 position = new Vector3(Random.Range(transform.position.x - 0.5f, transform.position.x + 0.5f), Random.Range(transform.position.y + 1f, transform.position.y + 5f), Random.Range(transform.position.z - 0.5f, transform.position.z + 0.5f));
                        Vector3 rotation = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

                        Instantiate(fruitSpawn, position, Quaternion.Euler(rotation), fruitParent);

                    }
                    fruitSpawnTime = 0;
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
