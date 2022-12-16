using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altitude : MonoBehaviour
{
    public Vector3 movement;
    public GameObject bird;
    public float defaultPos;

    private void Start()
    {
        movement = new Vector3(0, 1, 0);
        defaultPos = bird.transform.position.y;
    }

    void Update()
    {

        if (Bird.goingDown == true)
        {

            if (bird.transform.position.y >= defaultPos - 10f)
            {
                bird.transform.position -= movement * Time.deltaTime;
            }
        }

        if (Bird.goingDown == false)
        {

            if (bird.transform.position.y <= defaultPos)
            {
                bird.transform.position += movement * Time.deltaTime;
            }
        }
    }
}
