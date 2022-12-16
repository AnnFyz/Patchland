using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TallHerbivore : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform centrePoint;

    public SphereCollider myCollider;

    public float range;

    public float lifeExpect = 200;
    public float maxLifeExpect = 500;
    public float lifeSpan = 0;
    public float hunger = 0;
    public float hungerThreshold = 40;
    public float starveThreshold = 200;
    public float moveSpeed = 0.75f;
    public float reorientationTime = 0;


    public GameObject CurrentTarget = null;
    public GameObject floatingComic;
    public GameObject floatingdeath;

    public bool hungry = false;
    //STUFF FOR CONDICTION
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

    void Start()
    {
        myCollider = GetComponent<SphereCollider>();
        agent = GetComponent<NavMeshAgent>();
    }





    void Update()
    {
        lifeSpan += Time.deltaTime;
        reorientationTime += Time.deltaTime;
        hunger += Time.deltaTime * 3f;

        Healthbar.naturePoints += Time.deltaTime * 0.5f;


        if (hunger >= hungerThreshold)
        {
            hungry = true;
            StartCoroutine(ShowFloatingComic());

            if (CurrentTarget != null)
            {
                //transform.position = Vector3.MoveTowards(transform.position, CurrentTarget.transform.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                //transform.Translate(0, 0, moveSpeed * Time.deltaTime);

                if (reorientationTime >= 5)
                {
                    //transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                    reorientationTime = 0;
                }
            }
        }

        if (hunger < hungerThreshold)
        {
            hungry = false;
            StopText();
            StopAllCoroutines();
            //transform.Translate(0, 0, moveSpeed * Time.deltaTime);


            if (reorientationTime >= 2)
            {
                //transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                randomMovement();
            }
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
        if (actuallifeSpan < 70f)
        {
            isBaby = true;
        }
        if (actuallifeSpan > 70f && actuallifeSpan < lifeExpect - 70)
        {
            isAdult = true;
            isBaby = false;
        }
        if (actuallifeSpan > lifeExpect - 70)
        {
            isOld = true;
            isAdult = false;

        }

        // ESTABLISHING HOW HUNGRY IS THE ANIMAL
        float Hunger = Mathf.Clamp(hunger, 0, 201);
        if (Hunger <= 40)
        {
            if (isGreat != true)
                isGreat = true;
            if (isOK != false)
                isOK = false;
            if (isAlmostDead != false)
                isAlmostDead = false;
        }
        if (Hunger > 40 && Hunger < 130)
        {
            if (!isOK)
            {
                isOK = true;
                isGreat = false;
                if (isAlmostDead == true)
                    isAlmostDead = false;
            }
        }
        if (Hunger > 130)
        {
            isAlmostDead = true;
            isOK = false;

        }
        // CHANGING COLOR OF THE SPHERE ACCORDING TO THOSE PARAMETERS
        if (isGreat == true && isBaby || isGreat == true && isAdult)
        {
            Color32 lightGreen = new Color32(33, 190, 31, 255);
            myCondiction.gameObject.GetComponent<Renderer>().material.color = lightGreen;

        }
        if (isGreat == true && isOld)
        {
            Color32 Green = new Color32(8, 82, 7, 255);
            myCondiction.gameObject.GetComponent<Renderer>().material.color = Green;
        }
        if (isOK == true && isBaby || isOK == true && isAdult)
        {
            Color32 yello = new Color32(255, 202, 49, 255);
            myCondiction.gameObject.GetComponent<Renderer>().material.color = yello;

        }
        if (isOK == true && isOld)
        {
            Color32 Darkyello = new Color32(140, 99, 24, 255);
            myCondiction.gameObject.GetComponent<Renderer>().material.color = Darkyello;
        }
        if (isAlmostDead == true && isBaby || isAlmostDead == true && isAdult)
        {
            Color32 red = new Color32(255, 37, 37, 255);
            myCondiction.gameObject.GetComponent<Renderer>().material.color = red;
        }
        if (isAlmostDead == true && isOld)
        {
            Color32 darkred = new Color32(159, 29, 30, 255);
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
        if (collision.TryGetComponent<Leaves>(out Leaves leaves))
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
        if (collision.TryGetComponent<Leaves>(out Leaves leaves))
        {
            if (CurrentTarget == collision.gameObject)
            {
                CurrentTarget = null;
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("leaves"))
        {
            if (hungry == true)
            {
                Destroy(other.gameObject);
                hunger = 0;
                hungry = false;

                if (lifeExpect <= maxLifeExpect)
                {
                    lifeExpect += 30f;
                }

            }
        }
    }
    public IEnumerator ShowFloatingComic()
    {
        // Vector3 Offset = new Vector3(0, 2, 0);
        yield return new WaitForSeconds(2);
        floatingComic.gameObject.SetActive(true);

        if (isAlmostDead == true && isBaby || isAlmostDead == true && isAdult || isAlmostDead == true && isOld)
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