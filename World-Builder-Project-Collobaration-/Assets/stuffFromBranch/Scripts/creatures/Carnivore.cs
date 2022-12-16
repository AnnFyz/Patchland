using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Carnivore : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform centrePoint;

    public SphereCollider myCollider;

    public float lifeExpect = 65;
    public float maxLifeExpect = 150;
    public float lifeSpan = 0;
    public float hunger = 0f;
    public float moveSpeed = 1.2f;
    public float currMoveSpeed;
    public float reorientationTime = 0;
    public float hungerThreshold = 20;
    public float starveThreshold = 100;

    public GameObject CurrentTarget = null;
    public GameObject floatingComic;
    public GameObject floatingdeath;

    public bool hungry = false;
    // now writing the stuff for the ball ovr the head

    public bool isAlmostDead = false;
    public bool isOK = false;

    public bool isGreat = true;

    public bool isBaby = true;
    public bool isAdult = false;
    public bool isOld = false;

    public Material myMaterial;

    public GameObject myCondiction;
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



    void Start()
    {
        myCondiction.gameObject.GetComponent<Renderer>().material.color = new Color32(33, 190, 31, 255);
        myCollider = GetComponent<SphereCollider>();
        agent = GetComponent<NavMeshAgent>();
    }


    void Update()
    {
        lifeSpan += Time.deltaTime;
        reorientationTime += Time.deltaTime;
        hunger += Time.deltaTime * 1.5f;

        Healthbar.naturePoints += Time.deltaTime * 0.3f;

        currMoveSpeed = moveSpeed;

        if (hunger >= hungerThreshold)
        {
            hungry = true;
            StartCoroutine(ShowFloatingComic());

            if (CurrentTarget != null)
            {
                // currMoveSpeed = 4;
                //transform.position = Vector3.MoveTowards(transform.position, CurrentTarget.transform.position, currMoveSpeed * Time.deltaTime);
            }
            else
            {
                // currMoveSpeed = 3;
                //transform.Translate(0, 0, currMoveSpeed * Time.deltaTime);
                myCollider.radius = 80;

                if (reorientationTime >= 7)
                {
                    transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                    reorientationTime = 0;
                }
            }

        }

        if (hunger < hungerThreshold)
        {
            hungry = false;
            StopText();
            StopAllCoroutines();
            //  transform.Translate(0, 0, currMoveSpeed * Time.deltaTime);


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




        if (lifeSpan >= lifeExpect || hunger >= 100f)
        {
            Destroy(gameObject);
            Healthbar.darwinPoints += 10;
        }




        /*   if (CurrentTarget == null)
           {
               return;
           }*/
        //ESTABLISHING HOW OLD IS THE ANIMAL
        float actuallifeSpan = Mathf.Clamp(lifeSpan, 0, lifeExpect);
        if (actuallifeSpan < 30f)
        {
            isBaby = true;
        }
        if (actuallifeSpan > 30f && actuallifeSpan < lifeExpect - 15)
        {
            isAdult = true;
            isBaby = false;
        }
        if (actuallifeSpan > lifeExpect - 15)
        {
            isOld = true;
            isAdult = false;

        }

        // ESTABLISHING HOW HUNGRY IS THE ANIMAL
        float Hunger = Mathf.Clamp(hunger, 0, 101);
        if (Hunger <= 20)
        {
            if (isGreat != true)
                isGreat = true;
            if (isOK != false)
                isOK = false;
            if (isAlmostDead != false)
                isAlmostDead = false;
        }
        if (Hunger > 20 && Hunger < 80)
        {
            if (!isOK)
            {
                isOK = true;
                isGreat = false;
                if (isAlmostDead == true)
                    isAlmostDead = false;
            }
        }
        if (Hunger > 80)
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

    private void OnTriggerStay(Collider collision)
    {
        if (collision.TryGetComponent<Herbivore>(out Herbivore herbivore) || collision.TryGetComponent<SmallHerbivore>(out SmallHerbivore smallHerbivore) || collision.TryGetComponent<RunningBird>(out RunningBird runningBird))
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
        if (collision.TryGetComponent<Herbivore>(out Herbivore herbivore) || collision.TryGetComponent<SmallHerbivore>(out SmallHerbivore smallHerbivore) || collision.TryGetComponent<RunningBird>(out RunningBird runningBird))
        {
            if (CurrentTarget == collision.gameObject)
            {
                CurrentTarget = null;
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Herbivore") || other.gameObject.CompareTag("small herbivore") || other.gameObject.CompareTag("runningBird"))
        {
            if (hungry == true)
            {
                Destroy(other.gameObject);
                Healthbar.darwinPoints += 1;
                hunger = 0;

                if (lifeExpect < maxLifeExpect)
                {
                    lifeExpect += 5f;
                }
            }
        }
    }
    public IEnumerator ShowFloatingComic()
    {
        // Vector3 Offset = new Vector3(0, 2, 0);
        yield return new WaitForSeconds(2);
        floatingComic.gameObject.SetActive(true);
        // Instantiate(floatingComic, transform.position, Quaternion.identity, transform);
        if (isAlmostDead == true && isBaby || isAlmostDead == true && isAdult || isAlmostDead == true && isOld)
        {
            floatingdeath.gameObject.SetActive(true);
        }
        Invoke("StopText", 1f);
        //StartCoroutine(ShowFloatingComic());
        StopAllCoroutines();

    }
    void StopText()
    {
        // flaotingText.gameObject.SetActive(false);
        floatingComic.gameObject.SetActive(false);
        floatingdeath.gameObject.SetActive(false);

    }



}