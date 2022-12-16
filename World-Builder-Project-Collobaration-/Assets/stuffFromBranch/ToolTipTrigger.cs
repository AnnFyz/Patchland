using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  
   
    [Multiline()]
    public string content;

    public string header;
   public void OnPointerExit(PointerEventData eventData)
   {
    //throw new System.NotImplementedException();
    StopAllCoroutines();
    ToolTipSystem.Hide();
   }
   public void OnPointerEnter(PointerEventData eventData)
   {
    //throw new System.NotImplementedException();
    StartCoroutine(ShowTip());
   
   }
   IEnumerator ShowTip()
   {
     yield return new WaitForSeconds(0.8f);
      ToolTipSystem.Show(header, content);
   }  

   
}
