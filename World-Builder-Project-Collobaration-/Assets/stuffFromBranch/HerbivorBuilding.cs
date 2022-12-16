using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerbivorBuilding : MonoBehaviour
{
  
  //public List<GameObject> herbivores = new List<GameObject>();

   public float speedIncrease = 2f;

  

   


    void Start()
    {
        
    }

    // Update is called once per frame herbivores.Add(other.gameObject);
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Herbivore"))
        {
          //float moveSpeed = gameObject.GetComponent<Herbivore>().moveSpeed;
       
        }
    }
   
}
