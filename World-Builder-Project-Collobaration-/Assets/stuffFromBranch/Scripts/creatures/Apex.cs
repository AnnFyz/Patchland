using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apex : MonoBehaviour
{
    public float lifeExpect = 60f;
    public float maxLifeExpect = 300;
    public float lifeSpan = 0;
    public float hunger = 0f;
    public float moveSpeed = 1f;
    public float currMoveSpeed;
    public float reorientationTime = 0;
    public float hungerThreshold = 30;
    public float starveThreshold = 100;

    public GameObject CurrentTarget = null;

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



    void Start()
    {
       myCondiction.gameObject.GetComponent<Renderer>().material.color = new Color32(33,190,31,255);
    }


    void Update()
    {
        lifeSpan += Time.deltaTime;
        reorientationTime += Time.deltaTime;
        hunger += Time.deltaTime * 1.5f;

        Healthbar.naturePoints += Time.deltaTime * 1f;

        currMoveSpeed = moveSpeed;

        if (hunger >= hungerThreshold)
        {
            hungry = true;

            if (CurrentTarget != null)
            {
                currMoveSpeed = 5;
                transform.position = Vector3.MoveTowards(transform.position, CurrentTarget.transform.position, currMoveSpeed * Time.deltaTime);
            }
            else
            {
                currMoveSpeed = 3;
                transform.Translate(0, 0, currMoveSpeed * Time.deltaTime);

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
            transform.Translate(0, 0, currMoveSpeed * Time.deltaTime);


            if (reorientationTime >= 2)
            {
                transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                reorientationTime = 0;
            }
        }




        if (lifeSpan >= lifeExpect || hunger >= starveThreshold)
        {
            Destroy(gameObject);
        }




      /*  if (CurrentTarget == null)
        {
            return;
        }*/

        //ESTABLISHING HOW OLD IS THE ANIMAL
         float actuallifeSpan = Mathf.Clamp(lifeSpan, 0, lifeExpect);
      if(actuallifeSpan < 20f)
      {
       isBaby = true;
      }
       if(actuallifeSpan > 20f && actuallifeSpan < lifeExpect - 10)
      {
        isAdult = true;
        isBaby = false;
      }
       if(actuallifeSpan > lifeExpect - 10)
      {
        isOld = true;
        isAdult = false;
      
      }

      // ESTABLISHING HOW HUNGRY IS THE ANIMAL
       float Hunger = Mathf.Clamp(hunger, 0, 101);
        if(Hunger <= 30 )
      {
       if(isGreat != true)
        isGreat = true;
        if(isOK != false)
        isOK = false;
        if(isAlmostDead !=false)
        isAlmostDead = false;
      }
      if(Hunger > 30 && Hunger <80)
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

    private void OnTriggerStay(Collider collision)
    {
        if (collision.TryGetComponent<Herbivore>(out Herbivore herbivore) || collision.TryGetComponent<SmallHerbivore>(out SmallHerbivore smallHerbivore) || collision.TryGetComponent<Carnivore>(out Carnivore carnivore) || collision.TryGetComponent<TallHerbivore>(out TallHerbivore tallHerbivore) || collision.TryGetComponent<RunningBird>(out RunningBird runningBird) || collision.TryGetComponent<HugeHerbivore>(out HugeHerbivore hugeHerbivore))
        {
            if (CurrentTarget == null)
            {
                CurrentTarget = collision.gameObject;
            }
        }

    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.TryGetComponent<Herbivore>(out Herbivore herbivore) || collision.TryGetComponent<SmallHerbivore>(out SmallHerbivore smallHerbivore) || collision.TryGetComponent<Carnivore>(out Carnivore carnivore) || collision.TryGetComponent<TallHerbivore>(out TallHerbivore tallHerbivore) || collision.TryGetComponent<RunningBird>(out RunningBird runningBird) || collision.TryGetComponent<HugeHerbivore>(out HugeHerbivore hugeHerbivore))
        {
            if (CurrentTarget == collision.gameObject)
            {
                CurrentTarget = null;
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Herbivore") || other.gameObject.CompareTag("small herbivore") || other.gameObject.CompareTag("Carnivore") || other.gameObject.CompareTag("tall herbivore") || other.gameObject.CompareTag("runningBird") || other.gameObject.CompareTag("hugeHerbivore"))
        {
            if (hungry == true)
            {
                Destroy(other.gameObject);
                hunger = 0;

                if (lifeExpect < maxLifeExpect)
                {
                    lifeExpect += 10f;
                }
            }
        }
    }
}
