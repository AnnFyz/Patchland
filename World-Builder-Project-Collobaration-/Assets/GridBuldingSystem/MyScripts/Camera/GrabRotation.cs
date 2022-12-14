using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabRotation : MonoBehaviour
{
    public float offsetX;
    public float offsetY;
    public float sensitivityRotation = 1f;
    [SerializeField] Transform rotator;

    private void Start()
    {
        rotator.transform.rotation = Quaternion.Euler(-20f, -20f, 0);
    }
    public void Dragging()
    {
        offsetX = Input.GetAxis("Mouse X");
        offsetY = Input.GetAxis("Mouse Y");
        //rotator.transform.Rotate(Vector3.down, offsetX);
        //rotator.transform.Rotate(Vector3.right, offsetY);
        rotator.transform.eulerAngles += sensitivityRotation * new Vector3(-offsetY, offsetX, 0);

    }



}
