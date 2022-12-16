using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassPatches : MonoBehaviour
{
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
        if (FertileGround.fertile == false) // Ground.stone
        {
            witherTime += Time.deltaTime;

            if (withering == false)
            {
                witherLimit = Random.Range(10, 90);
                withering = true;
            }
        }

        if (FertileGround.fertile == true) //Ground.stone
        {
            witherTime = 0;

            if (withering == true)
            {
                withering = false;
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
