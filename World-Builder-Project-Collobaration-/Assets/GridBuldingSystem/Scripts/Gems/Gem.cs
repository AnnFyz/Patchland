using ChristinaCreatesGames.Animations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public bool IsThisGemSpecial = false;
    SquashAndStretch squashAndStretch;

    private void Awake()
    {
        squashAndStretch = GetComponent<SquashAndStretch>();
    }

    public void SquashAndStretch()
    {
        squashAndStretch = GetComponent<SquashAndStretch>();
        StartCoroutine(PlayAnimation());

    }

    IEnumerator PlayAnimation()
    {
        squashAndStretch.PlaySquashAndStretch();
        yield return new WaitForSeconds(0.75f);
        if (IsThisGemSpecial)
        {
            StartLoopAnimation();
        }
    }

    void StartLoopAnimation()
    {
        squashAndStretch.SetAnimation(.5f, true, 1.2f);
    }
    public void CollectGem()
    {
        UIManager.Instance.CollectGem();
        if (IsThisGemSpecial)
        {
            UIManager.Instance.CollectSpecialGem();
            GemManager.Instance.createdSpecialGems.Remove(this.gameObject.transform);
        }

        GemManager.Instance.createdGems.Remove(gameObject);
        Destroy(gameObject);
    }


}
