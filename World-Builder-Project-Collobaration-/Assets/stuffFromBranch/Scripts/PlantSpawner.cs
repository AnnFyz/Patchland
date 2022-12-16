using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpawner : MonoBehaviour
{
    
    
    [SerializeField] private GameObject grassSpawn;
    [SerializeField] private GameObject bushSpawn;
    [SerializeField] private GameObject treeSpawn;
    [SerializeField] private GameObject cactusSpawn;
    [SerializeField] private GameObject cactusTreeSpawn;
    [SerializeField] private GameObject mossSpawn;

    [SerializeField] Transform grassParent;
    [SerializeField] Transform bushParent;
    [SerializeField] Transform treeParent;
    [SerializeField] Transform cactusParent;
    [SerializeField] Transform cactusTreeParent;
    [SerializeField] Transform mossParent;

    public GameObject[] aliveGrass;
    public GameObject[] aliveBush;
    public GameObject[] aliveTree;
    public GameObject[] aliveCactus;
    public GameObject[] aliveCactusTree;
    public GameObject[] aliveCactusFlower;
    public GameObject[] aliveMoss;

    public int grassOffSpring;
    public int bushOffSpring = 1;
    public int treeOffSpring = 1;
    public int cactusOffSpring = 1;
    public int cactusTreeOffSpring = 1;
    public int mossOffSpring = 1;

    public static int currGrassPopulation;
    public static int currBushPopulation;
    public static int currTreePopulation;
    public static int currCactusPopulation;
    public static int currCactusTreePopulation;
    public static int currCactusFlowerPopulation;
    public static int currMossPopulation;

    public Transform[] spawnpoints;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        populationCheck();

        

        Grass();

        Bush();

        Tree();

        Cactus();

        CactusTree();

        Moss();
    }

    public void Grass()
    {
        if (Ground.grass == true)
        {
            grassOffSpring = 3;
        }
        if (Ground.sand == true)
        {
            grassOffSpring = 1;
        }

        if (Ground.stone == false)
        {
            if (Healthbar.naturePoints >= 10)
            {
                if (Input.GetKeyDown("1"))
                {
                    for (int i = 0; i < grassOffSpring; i++)
                    {
                        Vector3 position = new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
                        Vector3 rotation = new Vector3(0, Random.Range(0f, 360f), 0);

                        Instantiate(grassSpawn, position, Quaternion.Euler(rotation), grassParent);

                    }
                    Debug.Log(grassOffSpring + " patches of Grass spawned for 10 nature Points");
                    Healthbar.naturePoints -= 10f;
                }
            }
        }
        else
        {
            return;
        }
    }

    public void Bush()
    {
        if (Ground.grass == true)
        {
            if (Healthbar.naturePoints >= 20)
            {
                if (Input.GetKeyDown("2"))
                {
                    for (int i = 0; i < bushOffSpring; i++)
                    {
                        Vector3 position = new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
                        Vector3 rotation = new Vector3(0, Random.Range(0f, 360f), 0);

                        Instantiate(bushSpawn, position, Quaternion.Euler(rotation), bushParent);

                    }
                    Debug.Log(bushOffSpring + " bushe spawned for 20 nature Points");
                    Healthbar.naturePoints -= 20f;
                }
            }
        }
        else
        {
            return;
        }
    }

    public void Tree()
    {
        if (Ground.grass == true)
        {
            if (Healthbar.naturePoints >= 30)
            {
                if (Input.GetKeyDown("3"))
                {
                    for (int i = 0; i < treeOffSpring; i++)
                    {
                        Vector3 position = new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
                        Vector3 rotation = new Vector3(0, Random.Range(0f, 360f), 0);

                        Instantiate(treeSpawn, position, Quaternion.Euler(rotation), treeParent);

                    }
                    Debug.Log(treeOffSpring + " Tree spawned for 20 nature Points");
                    Healthbar.naturePoints -= 30f;
                }
            }
        }
        else
        {
            return;
        }
    }

    public void Cactus()
    {
        if (Ground.sand == true)
        {
            if (Healthbar.naturePoints >= 25)
            {
                if (Input.GetKeyDown("4"))
                {
                    for (int i = 0; i < cactusOffSpring; i++)
                    {
                        Vector3 position = new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
                        Vector3 rotation = new Vector3(0, Random.Range(0f, 360f), 0);

                        Instantiate(cactusSpawn, position, Quaternion.Euler(rotation), cactusParent);

                    }
                    Debug.Log(cactusOffSpring + " Cactus spawned for 25 nature Points");
                    Healthbar.naturePoints -= 25f;
                }
            }
        }
        else
        {
            return;
        }
    }

    public void CactusTree()
    {
        if (Ground.sand == true)
        {
            if (Healthbar.naturePoints >= 35)
            {
                if (Input.GetKeyDown("5"))
                {
                    for (int i = 0; i < cactusTreeOffSpring; i++)
                    {
                        Vector3 position = new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
                        Vector3 rotation = new Vector3(0, Random.Range(0f, 360f), 0);

                        Instantiate(cactusTreeSpawn, position, Quaternion.Euler(rotation), cactusTreeParent);

                    }
                    Debug.Log(cactusTreeOffSpring + " Cactus Tree spawned for 35 nature Points");
                    Healthbar.naturePoints -= 35f;
                }
            }
        }
        else
        {
            return;
        }
    }

    public void Moss()
    {
        if (Ground.grass == true)
        {
            mossOffSpring = 5;
        }
        if (Ground.sand == true)
        {
            mossOffSpring = 3;
        }

        if (Ground.sand == false)
        {
            if (Healthbar.naturePoints >= 5)
            {
                if (Input.GetKeyDown("6"))
                {
                    for (int i = 0; i < mossOffSpring; i++)
                    {
                        Vector3 position = new Vector3(Random.Range(-50f, 50f), 0, Random.Range(-50f, 50f));
                        Vector3 rotation = new Vector3(0, Random.Range(0f, 360f), 0);

                        Instantiate(mossSpawn, position, Quaternion.Euler(rotation), mossParent);

                    }
                    Debug.Log(mossOffSpring + " Moss spawned for 5 nature Points");
                    Healthbar.naturePoints -= 5f;
                }
            }
        }
        else
        {
            return;
        }
    }

    public void populationCheck()
    {
        aliveGrass = GameObject.FindGameObjectsWithTag("Plant");
        currGrassPopulation = aliveGrass.Length;

        aliveBush = GameObject.FindGameObjectsWithTag("bush");
        currBushPopulation = aliveBush.Length;

        aliveTree = GameObject.FindGameObjectsWithTag("tree");
        currTreePopulation = aliveTree.Length;

        aliveCactus = GameObject.FindGameObjectsWithTag("Cactus");
        currCactusPopulation = aliveCactus.Length;

        aliveCactusTree = GameObject.FindGameObjectsWithTag("CactusTree");
        currCactusTreePopulation = aliveCactusTree.Length;

        aliveCactusFlower = GameObject.FindGameObjectsWithTag("Cactus Flower");
        currCactusFlowerPopulation = aliveCactusFlower.Length;

        aliveMoss = GameObject.FindGameObjectsWithTag("Moss");
        currMossPopulation = aliveMoss.Length;
    }
}

