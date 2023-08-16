using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SceneRotator : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public float offsetX;
    public float offsetY;
    public float sensitivityRotation = 1f;
    [SerializeField] Transform rotator;
    [SerializeField] Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        image.enabled = false;
        rotator.transform.rotation = Quaternion.Euler(-20f, -20f, 0);
    }
    void Update()
    {
        if (Input.GetMouseButton(2))
        {
            image.enabled = true;
            Dragging();
        }
        else
        {
            image.enabled = false;
        }
    }
    public void Dragging()
    {
        offsetX = Input.GetAxis("Mouse X");
        offsetY = Input.GetAxis("Mouse Y");
        //rotator.transform.Rotate(Vector3.down, offsetX);
        //rotator.transform.Rotate(Vector3.right, offsetY);
        rotator.transform.eulerAngles += sensitivityRotation * new Vector3(-offsetY, offsetX, 0);

    }

   public void OnDrag(PointerEventData eventData)
    {
        
        Dragging();
        Debug.Log("DRAGGING");
    }

    public void OnEndDrag(PointerEventData eventData)
    {

       image.enabled = false;
    }
}
