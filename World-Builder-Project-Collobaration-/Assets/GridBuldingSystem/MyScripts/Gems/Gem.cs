using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public bool IsThisGemSpecial = false;
    public void CollectGem()
    {
        UIManager.Instance.CollectGem();
        if (IsThisGemSpecial)
        {
            UIManager.Instance.CollectSpecialGem();
        }
        Destroy(gameObject);
    }


}
