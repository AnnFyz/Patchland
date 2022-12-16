using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class animControllerUI : MonoBehaviour
{
    public ConstructionMenuUI menu;
    
    public GameObject on;

    public GameObject off;

    // Start is called before the first frame update
    void Start()
    {
        off.SetActive(false);
         
    }

    // Update is called once per frame
    void Update()
    {
        if(menu.isOpen == true)
        {
             on.SetActive(false);
       off.SetActive(true);
        }
        if(menu.isOpen == false)
        {
            on.SetActive(true);
            off.SetActive(false);
        }
        
    }
}
