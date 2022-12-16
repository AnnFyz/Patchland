using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlacement : MonoBehaviour
{
   SOplantSpawner spawner;

   public void Awake()
   {
    spawner = GameObject.Find("SOplantSpawner").GetComponent<SOplantSpawner>();
   }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("sand"))
        {
            spawner.isSand = true;
        }

    }
     private void OnTriggerExit(Collider other)
     {
        if(other.CompareTag("sand"))
        {
            spawner.isSand = false;
        }
     }
}
