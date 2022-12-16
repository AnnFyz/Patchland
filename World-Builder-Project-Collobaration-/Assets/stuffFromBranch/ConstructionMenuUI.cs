using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionMenuUI : MonoBehaviour
{
    public Animator anim;
   
    public bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   public  void OpenMenu()
    {
        anim.SetBool("isOpen", true);
       
        isOpen = true;
    }
   public  void CloseMenu()
    {
        anim.SetBool("isOpen", false);
      // anim.SetTrigger("Closed");
      
        isOpen = false;
    }
}
