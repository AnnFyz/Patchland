using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class Herbivore : MonoBehaviour
{
    public GameObject flaotingText; // TextMeshProUGUI GameObject
    public NavMeshAgent agent;
    public Transform centrePoint;
    public Rigidbody rb;

    public GameObject floatingComic;
    public GameObject floatingdeath;
    public float lifeExpect = 80;
    public float maxLifeExpect = 200;
    public float lifeSpan = 0;
    public float hunger = 0;
    public static float currHunger = 0;

    public static float currMaxLifeExpect;

    public static  float currentAge;

    public static float starveLimit;
    public float hungerThreshold = 50;
    public float starveThreshold = 100;
    public float moveSpeed = 1f;
    public float reorientationTime = 5;
   
   public Material myMaterial;

    public GameObject myCondiction;

    public Renderer myRenderer;


    public GameObject CurrentTarget = null;

    private SelectManager selectManager;
    public SphereCollider myCollider;
    //private Vector3 forceDown;
    

    

    public bool hungry = false;

  

    public bool isAlmostDead = false;
    public bool isOK = false;

    public bool isGreat = true;

    public bool isBaby = true;
    public bool isAdult = false;
    public bool isOld = false;

    public bool imSelected = false;
    public float range;
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }




    void Update()
    {

       // gameObject.transform.position += forceDown * Time.deltaTime;

        lifeSpan += Time.deltaTime;
        reorientationTime += Time.deltaTime;
        hunger += Time.deltaTime * 3f;
        currHunger = hunger;
        currentAge = lifeSpan;
        currMaxLifeExpect = lifeExpect;
        starveLimit = starveThreshold;
        Healthbar.naturePoints += Time.deltaTime * 0.2f;
         if(Input.GetMouseButtonDown(1))
      {
        imSelected = false;
        // infoPanel.SetActive(false);
      }
       


        if (hunger >= hungerThreshold)
        {
            hungry = true;
            if(hungry == true)
            {
                //activate pop up text Im hungry!
               StartCoroutine(ShowFloatingComic()); 
            }

            if (CurrentTarget != null)
            {
                // transform.position = Vector3.MoveTowards(transform.position, CurrentTarget.transform.position, moveSpeed * Time.deltaTime);
                
            }
            else
            {

                myCollider.radius = 100;
                //randomMovement();

                /*
                transform.Translate(0, 0, moveSpeed * Time.deltaTime);

                if (reorientationTime >= 5)
                {
                    transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                    reorientationTime = 0;
                }
                */
            }
        }

        if (hunger < hungerThreshold)
        {
            hungry = false;
            StopText();
            StopAllCoroutines();
            myCollider.radius = 20;
            //transform.Translate(0, 0, moveSpeed * Time.deltaTime);


            /* if (reorientationTime >= 2)
             {
                 transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                 reorientationTime = 0;
             }*/

            if (/*agent.remainingDistance <= agent.stoppingDistance || */ reorientationTime >= 5) //done with path
            {
                randomMovement();
            }
        }
       // myRenderer.sharedMaterial = myMaterial[0];
        //  float actuallifeSpan = Mathf.Clamp(lifeSpan, 39f, 70f); // var actualLifeExpect = lifeExpect - (lifeExpect / 2);
        if (lifeSpan >= lifeExpect || hunger >= starveThreshold)
        {
            Destroy(gameObject);
            Healthbar.darwinPoints += 10;
        }
        /* if (CurrentTarget == null)
        {
            return;
        }*/

     // ESTABLISHING HOW OLD IS THE ANIMAL
      float actuallifeSpan = Mathf.Clamp(lifeSpan, 0, lifeExpect);
      if(actuallifeSpan < 40f)
      {
       isBaby = true;
      }
       if(actuallifeSpan > 40f && actuallifeSpan < lifeExpect - 15)
      {
        isAdult = true;
        isBaby = false;
      }
       if(actuallifeSpan > lifeExpect - 15)
      {
        isOld = true;
        isAdult = false;
      
      }
      // ESTABLISHING HOW hungry THEY ARE
      float Hunger = Mathf.Clamp(hunger, 0, 101);
     // float condiction = Mathf.Clamp(Hunger/actuallifeSpan, 0, 10);
      //float Imhunger = Mathf.Clamp(hunger, 50, 75);
      //float starving = Mathf.Clamp(hunger, 75, 101);
      if(Hunger <= 59 )
      {
       if(isGreat != true)
        isGreat = true;
        if(isOK != false)
        isOK = false;
        if(isAlmostDead !=false)
        isAlmostDead = false;
      }
      if(Hunger > 60 && Hunger <80)
      {
        if(!isOK)
        {
            isOK = true;
            isGreat = false;
             if(isAlmostDead == true)
        isAlmostDead = false;
        }
      }
       if(Hunger > 80)
      {
        isAlmostDead = true;
        isOK = false;
        //StartCoroutine(ShowFloatingDeath());
        
      }
      // CHANGING COLOR OF THE SPHERE ACCORDING TO THOSE PARAMETERS
      if(isGreat == true && isBaby ||isGreat == true &&  isAdult )
      {
         Color32 lightGreen = new Color32(33,190,31,255);
        myCondiction.gameObject.GetComponent<Renderer>().material.color = lightGreen;

      }
      if(isGreat == true && isOld )
      {
         Color32 Green = new Color32(8,82,7,255);
        myCondiction.gameObject.GetComponent<Renderer>().material.color = Green;
      }
      if(isOK == true &&  isBaby || isOK == true && isAdult  )
      {
        Color32 yello = new Color32(255,202,49,255);
        myCondiction.gameObject.GetComponent<Renderer>().material.color = yello;
         //floatingdeath.gameObject.SetActive(false);

      }
      if(isOK == true &&  isOld )
      {
         Color32 Darkyello = new Color32(140,99,24,255);
         myCondiction.gameObject.GetComponent<Renderer>().material.color = Darkyello;
         // floatingdeath.gameObject.SetActive(false);
      }
      if(isAlmostDead == true && isBaby ||isAlmostDead == true && isAdult)
      {
         Color32 red = new Color32(255,37,37,255);
         myCondiction.gameObject.GetComponent<Renderer>().material.color = red;
       //  Invoke("StartmyText", 0f);
        //  StartCoroutine(ShowFloatingDeath());
       //  floatingdeath.gameObject.SetActive(true);
        
      }
      if(isAlmostDead == true && isOld)
      {
        Color32 darkred = new Color32(159,29,30,255);
         myCondiction.gameObject.GetComponent<Renderer>().material.color = darkred;
        //StartCoroutine(ShowFloatingDeath());
       //  floatingdeath.gameObject.SetActive(true); 
      }

      
      
  
    }

    void randomMovement()
    {
        Vector3 point;
        if (RandomPoint(centrePoint.position, range, out point)) //pass in our centre point and radius of area
        {
            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
            agent.SetDestination(point);
            reorientationTime = 0f;
        }
    }
    


    void OnTriggerStay(Collider collision)
    {
        if (collision.TryGetComponent<GrassPatches>(out GrassPatches plant))
        {
            Vector3 target;
            if (CurrentTarget == null && hungry == true)
            {
                CurrentTarget = collision.gameObject;
                target = CurrentTarget.transform.position;
                agent.SetDestination(target);
            }
        }

    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.TryGetComponent<GrassPatches>(out GrassPatches plant) )
        {
            if (CurrentTarget == collision.gameObject)
            {
                CurrentTarget = null;
            }
        }

        if(collision.gameObject.CompareTag("HerbivoreRock"))
        {
          moveSpeed = 1f;

        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Plant"))
        {
            if (hungry == true)
            {
               
                Destroy(other.gameObject);
                hunger = 0;
                hungry = false;
                Healthbar.darwinPoints += 1;
                myCollider.radius = 20;

                if (lifeExpect <= maxLifeExpect)
                {
                    lifeExpect += 10f;
                }
                
            }
        }
        
    }
    void OnCollisionEnter(Collider other)
    {
      /*
        if(other.gameObject.CompareTag("HerbivoreRock"))
        {
          // float speedIncrease = gameObject.GetComponent<HerbivorBuilding>().speedIncrease;
          // IncreaseSpeed(speedIncrease);
          moveSpeed = 2.5f;

        }
      */

        if (other.gameObject.CompareTag("Ground"))
        {
            Debug.Log("ground hit");
            //forceDown = new Vector3(0, 0, 0);
        }
    }
    public void IncreaseSpeed(float speedIncrease)
    {
      
          moveSpeed += speedIncrease;
    }

    public IEnumerator ShowFloatingText()
    {
         Vector3 Offset = new Vector3(0, 2, 0);
         yield return new WaitForSeconds(3);
         flaotingText.gameObject.SetActive(true);
       Instantiate(flaotingText, transform.position + Offset, Quaternion.identity, transform);
     //  flaotingText.gameObject.GetComponent<TextMesh>().color;
       //go.GetComponent<TextMesh>().text = hungry.ToString();
      // StartCoroutine(ShowFloatingText());
      
    }
    public IEnumerator ShowFloatingComic()
    {
      // Vector3 Offset = new Vector3(0, 2, 0);
         yield return new WaitForSeconds(2);
         floatingComic.gameObject.SetActive(true);
        // Instantiate(floatingComic, transform.position, Quaternion.identity, transform);
        if(isAlmostDead == true && isBaby ||isAlmostDead == true && isAdult || isAlmostDead == true && isOld)
        {
          floatingdeath.gameObject.SetActive(true);
        }
         Invoke("StopText", 1f);
         //StartCoroutine(ShowFloatingComic());
         StopAllCoroutines();

    }
     public IEnumerator ShowFloatingDeath()
    {
      // Vector3 Offset = new Vector3(0, 2, 0);
         yield return new WaitForSeconds(2);
         floatingdeath.gameObject.SetActive(true);
        // Instantiate(floatingdeath, transform.position, Quaternion.identity, transform);
         Invoke("StopText", 1f);
         //StartCoroutine(ShowFloatingComic());
        // StopAllCoroutines();

    }
    void StartmyText()
    {
       floatingdeath.gameObject.SetActive(true);
     //  Invoke("StopmyText", 2f);
    }
    void StopText()
    {
        flaotingText.gameObject.SetActive(false);
        floatingComic.gameObject.SetActive(false);
        floatingdeath.gameObject.SetActive(false);
         
    }
     void StopmyText()
     {
       
      // Invoke("StartmyText", 2f);
     }
    void Start()
    {
      myCondiction.gameObject.GetComponent<Renderer>().material.color = new Color32(33,190,31,255);
      
        myCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        rb.isKinematic = true;
        myCollider.radius = 1;
       //forceDown = new Vector3(0, -10, 0);
       
       
       
    }
   void OnMouseDown()
   {
     imSelected = true;
   // selectManager =  gameObject.GetComponent<SelectManager>();
    /* infoPanel = selectManager.objUI;
     infoPanel.SetActive(true);
     objstatAge.text = selectManager.objstatHunger.text;
     objstatAge.text = lifeSpan.ToString("");*/
     //hunger = gameObject.GetComponent<SelectManager>().objHunger;
     
     
   }
}
