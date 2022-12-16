using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float x;
     private float y;
     private Vector3 rotateValue;

     public float speed = 10f;
   
    //public Camera m_OrthographicCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    
   void Update ()
{
     
    // -------------------Code for Zooming Out------------
    if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (Camera.main.fieldOfView<=125)
                Camera.main.fieldOfView +=2;
          //  if (Camera.main.orthographicSize<=20)
             //  Camera.main.orthographicSize +=0.5;
 
        }
    // ---------------Code for Zooming In------------------------
     if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.fieldOfView>2)
                Camera.main.fieldOfView -=2;
           // if (Camera.main.orthographicSize>=1)
               //  Camera.main.orthographicSize -=0.5;
        }
       
    // -------Code to switch camera between Perspective and Orthographic--------
     if (Input.GetKeyUp(KeyCode.B ))
    {
        if (Camera.main.orthographic==true)
            Camera.main.orthographic=false;
        else
            Camera.main.orthographic=true;
    }
    Vector3 inputDir = new Vector3(0, 0, 0);
     if(Input.GetKey(KeyCode.D))
     {
        inputDir.x = +1f;
        // transform.Translate(new Vector3(speed * Time.deltaTime,0,0));
     }
     if(Input.GetKey(KeyCode.A))
     {
        inputDir.x = -1f;
        // transform.Translate(new Vector3(-speed * Time.deltaTime,0,0));
     }
     if(Input.GetKey(KeyCode.S))
     {
        inputDir.z = -1f;
        // transform.Translate(new Vector3(0,-speed * Time.deltaTime,0));
        // transform.position -= Vector3.forward * speed * Time.deltaTime ;
     }
     if(Input.GetKey(KeyCode.W))
     {
        inputDir.z = +1f;
        // transform.Translate(new Vector3(0,speed * Time.deltaTime,0));
        //transform.position += Vector3.forward * speed * Time.deltaTime;
     }
     Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
     transform.position += moveDir * speed * Time.deltaTime;
     y = Input.GetAxis("Mouse X");
         x = Input.GetAxis("Mouse Y");
        // Debug.Log(x + ":" + y);
         rotateValue = new Vector3(x, y * -1, 0);
         transform.eulerAngles = transform.eulerAngles - rotateValue;
}
}
