using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterfly : MonoBehaviour
{
    public float lifeExpect = 50;
    public float maxLifeExpect = 80;
    public float lifeSpan = 0;
    public float hunger = 0;
    public float hungerThreshold = 1;
    public float starveThreshold = 30;
    public float moveSpeed = 3f;
    public float reorientationTime = 0;
    public float altitude = 1f;


    public GameObject CurrentTarget = null;

    public bool hungry = false;

    void Start()
    {

    }

    void Update()
    {
        lifeSpan += Time.deltaTime;
        reorientationTime += Time.deltaTime;
        hunger += Time.deltaTime * 3f;

        Healthbar.naturePoints += Time.deltaTime * 3f;




        if (hunger >= hungerThreshold)
        {
            hungry = true;

            if (CurrentTarget != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, CurrentTarget.transform.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(0, altitude * Time.deltaTime, moveSpeed * Time.deltaTime);

                if (reorientationTime >= 5)
                {
                    transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                    reorientationTime = 0;
                }
            }
        }



        if (transform.position.y >= 15)
        {
            altitude = Random.Range(-1f, -5f);
        }

        if (transform.position.y <= 0)
        {
            altitude = Random.Range(1f, 5f);
        }



        if (hunger < hungerThreshold)
        {
            hungry = false;
            transform.Translate(0, altitude * Time.deltaTime, moveSpeed * Time.deltaTime);


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


        if (CurrentTarget == null)
        {
            return;
        }
    }

    void OnTriggerStay(Collider collision)
    {
        if (collision.TryGetComponent<Flower>(out Flower flower))
        {
            if (CurrentTarget == null)
            {
                CurrentTarget = collision.gameObject;
            }
        }

    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.TryGetComponent<Flower>(out Flower flower))
        {
            if (CurrentTarget == collision.gameObject)
            {
                CurrentTarget = null;
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Cactus Flower"))
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
}
