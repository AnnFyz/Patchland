using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    public float rotationSpeed = 70f;
    public float topdownRotationSpeed = 0.1f;
    public float verticalSpeed = 15f;
    public float ZoomSpeed = 10f;
    Quaternion startRotation;

    [SerializeField] private Transform target;
    [SerializeField] private Camera cam;
    private bool topview;


    void Start()
    {

        transform.position = target.position + new Vector3(0, 0, 0);
        topview = false;
    }

    void Update()
    {
        //transform.position = target.position + new Vector3(0, -0.5f, 0);
        if (Input.GetKey("right shift") || Input.GetKey("left shift"))
        {
            rotationSpeed = 140f;
            verticalSpeed = 30;
            ZoomSpeed = 20f;
            topdownRotationSpeed = 0.3f;
        }
        else
        {
            rotationSpeed = 70f;
            verticalSpeed = 15;
            ZoomSpeed = 10f;
            topdownRotationSpeed = 0.1f;
        }


        if (topview == false)
        {
            if (Input.GetKey("a") || Input.GetKey("left"))
            {
                transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            }

            if (Input.GetKey("d") || Input.GetKey("right"))
            {
                transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
            }

            if (Input.GetKey("w") || Input.GetKey("up"))
            {
                if (transform.position.y <= 30f)
                {
                    transform.position += new Vector3(0, verticalSpeed * Time.deltaTime, 0);
                }
            }

            if (Input.GetKey("s") || Input.GetKey("down"))
            {
                if (transform.position.y >= -30f)
                {
                    transform.position += new Vector3(0, -verticalSpeed * Time.deltaTime, 0);
                }
            }
        }

        if (topview == true)
        {
            if (Input.GetKey("a") || Input.GetKey("left"))
            {
                cam.transform.Rotate(0, 0, topdownRotationSpeed);
            }

            if (Input.GetKey("d") || Input.GetKey("right"))
            {
                cam.transform.Rotate(0, 0, -topdownRotationSpeed);
            }
        }




        if (Input.GetKeyDown("1") || Input.GetKey("[1]"))
        {
            Reposition();
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (Input.GetKeyDown("2") || Input.GetKey("[2]"))
        {
            Reposition();
            transform.rotation = Quaternion.Euler(0, 45, 0);
        }

        if (Input.GetKeyDown("3") || Input.GetKey("[3]"))
        {
            Reposition();
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }

        if (Input.GetKeyDown("4") || Input.GetKey("[4]"))
        {
            Reposition();
            transform.rotation = Quaternion.Euler(0, 135, 0);
        }

        if (Input.GetKeyDown("5") || Input.GetKey("[5]"))
        {
            Reposition();
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (Input.GetKeyDown("6") || Input.GetKey("[6]"))
        {
            Reposition();
            transform.rotation = Quaternion.Euler(0, 225, 0);
        }

        if (Input.GetKeyDown("7") || Input.GetKey("[7]"))
        {
            Reposition();
            transform.rotation = Quaternion.Euler(0, 270, 0);
        }

        if (Input.GetKeyDown("8") || Input.GetKey("[8]"))
        {
            Reposition();
            transform.rotation = Quaternion.Euler(0, 315, 0);
        }

        if (Input.GetKeyDown("9") || Input.GetKey("[9]"))
        {
            transform.rotation = Quaternion.Euler(56.25f, 0, 0);

            topview = true;
        }



        if (cam.orthographic)
        {
                cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;

        }
        else
        {
                cam.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
        }

    }

    void Reposition()
    {
        if (topview == true)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            cam.transform.rotation = Quaternion.Euler(33.75f, 0, 0);
            topview = false;
        }
    }
}
