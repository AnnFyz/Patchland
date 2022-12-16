using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectManager : MonoBehaviour
{
    //  public List<Herbivore> selectedHerbivor = new List<Herbivore>();

      public LayerMask herbivorMask;
      public LayerMask groundMask;
      public GameObject selectedObject;

    //  public Herbivore herb;
       public TextMeshProUGUI objNameText;
       public TextMeshProUGUI objstatHunger;
       public float objHunger;
       public TextMeshProUGUI objstatAge;
       public TextMeshProUGUI objstatMaxAge;
       public TextMeshProUGUI objsatMaxHunger;

       

   // private float objHunger;
   
    public GameObject objUI;
    //  if(hit.collider.gameObject.CompareTag("Herbivore")) && objstatHunger != null && objstatHunger.text != null
    //  Select(hit.collider.gameObject); //if(selectedObject != hit.collider.gameObject)
    //  if(hit.collider.gameObject.GetComponent("Herbivore"))
    // Start is called before the first frame update .Find("Herbivore") //  Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    void Update()
    {
      if(selectedObject != null)
      { 
        if(selectedObject.GetComponent("Herbivore") )
       { 
        objstatHunger.text = selectedObject.GetComponent<Herbivore>().hunger.ToString("0");
        objstatAge.text = selectedObject.GetComponent<Herbivore>().lifeSpan.ToString("0");
        objstatMaxAge.text = selectedObject.GetComponent<Herbivore>().lifeExpect.ToString("0");
        objsatMaxHunger.text = selectedObject.GetComponent<Herbivore>().starveThreshold.ToString("0");
       }
       else
       {
        objstatHunger.text = "null";
       }
       
      }
      
        if(Input.GetMouseButtonDown(0))
       {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if(Physics.Raycast(ray, out hit, 1000, herbivorMask))
        {      
                if(hit.collider.gameObject.CompareTag("Herbivore"))
                {                  
                  selectedObject = hit.collider.gameObject;
                  if(selectedObject.GetComponent("Herbivore") && selectedObject != null)
                  {
                  // objstatHunger.text = selectedObject.GetComponent<Herbivore>().hunger.ToString("0");//StartCoroutine(WorkedCoroutine(hit.collider.gameObject));
                   objUI.SetActive(true);
                  } 
                }
        }

      } 

        
       if(Input.GetMouseButtonDown(1)) // selectedHerbivor.Clear(); //  foreach ( var herb in selectedHerbivor)
       {
        Deselect();
        //StopAllCoroutines();
        
       }
        if(selectedObject == null)
        {
          //StopAllCoroutines(); 
        }
    }

IEnumerator WorkedCoroutine(GameObject selectedObj)
{
  objstatHunger.text = selectedObject.GetComponent<Herbivore>().hunger.ToString("0");
  Debug.Log("Hunger of selected Obj " + selectedObject.GetComponent<Herbivore>().hunger);
  yield return new WaitForSeconds(0);
}
    public void Select(GameObject obj)
    {
      if(obj == selectedObject)
      return;
      if(selectedObject != null) Deselect();
      Outline outline = obj.GetComponent<Outline>();
      if(outline == null) obj.AddComponent<Outline>();
      else outline.enabled = true;
      selectedObject = obj;
      objUI.SetActive(true);
      objNameText.text = obj.name;
      
    
      

     
      
    }

    public void Deselect()
    {
        objUI.SetActive(false);
         if(selectedObject != null)
         {
            selectedObject.GetComponent<Outline>().enabled = false;
      
            selectedObject = null;
         }
       
    }
    public void Start()
    {
        // objstatHunger.text =  objHunger.ToString(); 
        selectedObject = null;
    }
    public IEnumerator UpdateUi()
    {
        yield return new WaitForSeconds(0);
        if(selectedObject.GetComponent("Herbivore") && selectedObject != null)
        {
         //gameObject.GetComponent<Herbivore>();

         // selectedObject.GetComponent<Herbivore>().hunger;
            
          // if(selectedObject.name.Contains("Clone") && selectedObject != null) Herbivore.currHunger;
         // float objHunger = Herbivore.currHunger;
          //objstatHunger.text = objHunger.ToString("0"); 
        //  float objAge = Herbivore.currentAge;
         // objstatAge.text = objAge.ToString("0");
          /*float currMaxLifeExpect = Herbivore.currMaxLifeExpect;
          objstatMaxAge.text = currMaxLifeExpect.ToString("0");
          float objMaxHunger = Herbivore.starveLimit;
          objsatMaxHunger.text = objMaxHunger.ToString("0");*/
          
        }
       
       
        StartCoroutine(UpdateUi());
    }
  
}
