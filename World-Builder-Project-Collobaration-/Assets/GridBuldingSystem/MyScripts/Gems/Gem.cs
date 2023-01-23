using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
   public void CollectGem()
    {
        UIManager.Instance.CollectGem();
        Destroy(gameObject);
    }
}
