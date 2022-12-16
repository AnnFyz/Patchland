using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SOplantSpawner : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    public PlantSo[] plant;
    public GameObject selectedObject;
    public int currentCost;
    private Vector3 posi;
    private RaycastHit hit;
    public LayerMask layermask;
     public LayerMask layermask2;

    public bool isSand = false;
    public bool grassSelected;

    public GameObject underMenu1;
    private bool isActive1;
     public GameObject underMenu2;
     private bool isActive2;
      public GameObject underMenu3;
      private bool isActive3;
       public GameObject underMenu4;
      private bool isActive4;
    private Vector3 offset;

   

    public void Start()
    {
           //     Vector3 offset = new Vector3(0, 0, 0);
    }

    //public TextMeshProUGUI resourcetext;

    private void FixedUpdate()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycasthit, 99000, layermask)) 
        {
            posi = raycasthit.point;
           // return (success: true, posi: hit.point);
        }
        /*
         Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layermask2)) 
        {
            posi = hit.point;
        }
        */
    }

     public void ChooseObject(int index)
     {
        if(currentCost <= Healthbar.naturePoints)
        {
           selectedObject = Instantiate(plant[index].plantBody, posi , transform.rotation);
           currentCost = plant[index].Cost;
           selectedObject.name = plant[index].plantname;
          // myCollider = selectedObject.AddComponent<BoxCollider>();
           Healthbar.naturePoints -= currentCost;
        }

     }
     public void Update()
     {
         if(selectedObject == null)
        {
          currentCost = 0;
          
        }
        if (selectedObject != null)
        {
            selectedObject.transform.position = posi;
            


        }

        if (Input.GetMouseButtonDown(0) && selectedObject != null)
        {
            if(selectedObject.name == "Grass")
            {
                grassSelected = true;
                PositionateObject();
     
            }
            else
            //grassSelected = false;
             PositionateObject();
        }
        if (Input.GetMouseButtonDown(1) && selectedObject != null)
        {
             Healthbar.naturePoints += currentCost;
            Destroy(selectedObject);
           
        }
        if(Input.GetMouseButtonDown(1))
        {
          if(isActive1 == true)
          {
            hideUnderMenu();
          }
          if(isActive2 == true)
          {
             hideUnderMenu2();
          }
          if(isActive3 == true)
          {
            hideUnderMenu3();
          }
           if(isActive4 == true)
          {
            hideUnderMenu4();
          }
           
           
          
        }
     }

     public void PositionateObject()
     {
        if(selectedObject.name != "Grass")
        selectedObject = null;
        if(selectedObject.name == "Grass")
        {
            selectedObject = null;
            StartCoroutine(myGrass());
            
        }
        
        
     }
   
    public IEnumerator myGrass()
    {
        yield return new WaitForSeconds(0);
        selectedObject = Instantiate(plant[3].plantBody, posi, transform.rotation);
        currentCost = plant[3].Cost;
        Healthbar.naturePoints -= currentCost;
        selectedObject.name = plant[3].plantname;
       // StartCoroutine(myGrass());

    }
    public void showUnderMenu()
    {
        if(isActive2 != true || isActive3 != true || isActive4 != true)
        {
           underMenu1.SetActive(true);
           isActive1 = true;
          // isActive3 = false;
        }
     
    }
    public void hideUnderMenu()
    {
      underMenu1.SetActive(false);
      isActive1 = false;
    }
    public void showUnderMenu2()
    {
      if(isActive1 != true || isActive3 != true || isActive4 != true)  
      {underMenu2.SetActive(true);
      isActive2 = true; }
    }
    public void hideUnderMenu2()
    {
      underMenu2.SetActive(false);
      isActive2 = false;
    }
    public void showUnderMenu3()
    {
      if(isActive1 != true || isActive2 != true || isActive4 != true)
      {
        underMenu3.SetActive(true);
        isActive3 = true;
      }
    }
    public void hideUnderMenu3()
    {
      underMenu3.SetActive(false);
      isActive3 = false;
    }
    public void hideUnderMenu4()
    {
      underMenu4.SetActive(false);
      isActive4 = false;
    }
    public void showUnderMenu4()
    {
      if(isActive1 != true || isActive2 != true || isActive3 != true)
      underMenu4.SetActive(true);
      isActive4 = true;
    }
}
