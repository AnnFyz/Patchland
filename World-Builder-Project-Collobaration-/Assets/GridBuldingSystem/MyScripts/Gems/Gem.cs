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
            GemManager.Instance.createdSpecialGems.Remove(this.gameObject.transform);
        }

        GemManager.Instance.createdGems.Remove(gameObject.transform);
        Destroy(gameObject);
    }


}
