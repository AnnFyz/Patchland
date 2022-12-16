using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
   public float DestroyTime;
   public Vector3 Offset = new Vector3(0, 2, 0);
   public Vector3 RandomizeIntesity = new Vector3(0.5f,0,0);
    void Start()
    {
       // Destroy(gameObject, DestroyTime);
        Invoke("HideText", 3f);
        transform.localPosition += Offset;
        transform.localPosition += new Vector3(Random.Range(-RandomizeIntesity.x, RandomizeIntesity.x), Random.Range(-RandomizeIntesity.y, RandomizeIntesity.y), Random.Range(-RandomizeIntesity.z, RandomizeIntesity.z));
    }

    // Update is called once per frame
    void HideText()
    {
         gameObject.SetActive(false);
    }
}
