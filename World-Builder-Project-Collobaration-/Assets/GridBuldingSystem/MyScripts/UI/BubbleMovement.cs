using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleMovement : MonoBehaviour
{

    void FixedUpdate()
    {
        transform.gameObject.GetComponent<RectTransform>().localPosition += Vector3.up * Time.fixedDeltaTime * 100f;
    }
}
