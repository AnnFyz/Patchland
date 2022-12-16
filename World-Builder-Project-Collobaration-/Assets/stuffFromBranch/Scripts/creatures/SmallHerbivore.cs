using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SmallHerbivore : MonoBehaviour
{
    public float lifeExpect = 50;
    public float maxLifeExpect = 80;
    public float lifeSpan = 0;
    public float hunger = 0;
    public float hungerThreshold = 10;
    public float starveThreshold = 100;
    public float moveSpeed = 3f;
    public float currMoveSpeed;
    public float reorientationTime = 0;

    public NavMeshAgent agent;
    public Transform centrePoint;
    public float range;


    public GameObject CurrentTarget = null;
    public GameObject floatingComic;
    public GameObject floatingdeath;
    public SphereCollider myCollider;

    public bool hungry = false;
      // ADDING CONDICTION STUFF
    public bool isAlmostDead = false;
    public bool isOK = false;

    public bool isGreat = true;

    public bool isBaby = true;
    public bool isAdult = false;
    public bool isOld = false;

    public Material myMaterial;

    public GameObject myCondiction;

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
        lifeSpan += Time.deltaTime;
        reorientationTime += Time.deltaTime;
        hunger += Time.deltaTime * 3f;

        Healthbar.naturePoints += Time.deltaTime * 0.01f;

        currMoveSpeed = moveSpeed;

        if (hunger >= hungerThreshold)
        {
            hungry = true;
            if (hungry == true)
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


        if (lifeSpan >= lifeExpect || hunger >= starveThreshold)
        {
            Destroy(gameObject);
        }


       /* if (CurrentTarget == null)
        {
            return;
        }*/
          //HOW OLD IS THE BIRD?  
      float actuallifeSpan = Mathf.Clamp(lifeSpan, 0, lifeExpect);
      if(actuallifeSpan < 15f)
      {
       isBaby = true;
      }
       if(actuallifeSpan > 15f && actuallifeSpan < lifeExpect - 8)
      {
        isAdult = true;
        isBaby = false;
      }
       if(actuallifeSpan > lifeExpect - 8)
      {
        isOld = true;
        isAdult = false;
      
      }

      // ESTABLISHING HOW HUNGRY IS THE ANIMAL
       float Hunger = Mathf.Clamp(hunger, 0, 101);
        if(Hunger <= 10 )
      {
       if(isGreat != true)
        isGreat = true;
        if(isOK != false)
        isOK = false;
        if(isAlmostDead !=false)
        isAlmostDead = false;
      }
      if(Hunger > 10 && Hunger <70)
      {
        if(!isOK)
        {
            isOK = true;
            isGreat = false;
             if(isAlmostDead == true)
        isAlmostDead = false;
        }
      }
       if(Hunger > 70)
      {
        isAlmostDead = true;
        isOK = false;
        
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

      }
      if(isOK == true &&  isOld )
      {
         Color32 Darkyello = new Color32(140,99,24,255);
         myCondiction.gameObject.GetComponent<Renderer>().material.color = Darkyello;
      }
      if(isAlmostDead == true && isBaby ||isAlmostDead == true && isAdult)
      {
         Color32 red = new Color32(255,37,37,255);
         myCondiction.gameObject.GetComponent<Renderer>().material.color = red;
      }
      if(isAlmostDead == true && isOld)
      {
        Color32 darkred = new Color32(159,29,30,255);
         myCondiction.gameObject.GetComponent<Renderer>().material.color = darkred;
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
            if (CurrentTarget == null)
            {
                CurrentTarget = collision.gameObject;
            }
        }

    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.TryGetComponent<GrassPatches>(out GrassPatches plant))
        {
            if (CurrentTarget == collision.gameObject)
            {
                CurrentTarget = null;
            }
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
                myCollider.radius = 20;

                if (lifeExpect <= maxLifeExpect)
                {
                    lifeExpect += 3f;
                }

            }
        }
    }
    void Start()
    {
         myCondiction.gameObject.GetComponent<Renderer>().material.color = new Color32(33,190,31,255);
    }
    public IEnumerator ShowFloatingComic()
    {
      // Vector3 Offset = new Vector3(0, 2, 0);
         yield return new WaitForSeconds(2);
         floatingComic.gameObject.SetActive(true);
        
        if(isAlmostDead == true && isBaby ||isAlmostDead == true && isAdult || isAlmostDead == true && isOld)
        {
          floatingdeath.gameObject.SetActive(true);
        }
         Invoke("StopText", 1f);
        
         StopAllCoroutines();

    }
     void StopText()
    {
       // flaotingText.gameObject.SetActive(false);
        floatingComic.gameObject.SetActive(false);
        floatingdeath.gameObject.SetActive(false);
         
    }
}
