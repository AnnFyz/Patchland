using ChristinaCreatesGames.Animations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


public class Gem : MonoBehaviour
{
    private IObjectPool<Gem> gemOPool;    
    public IObjectPool<Gem> GemPool
    {
        set { gemOPool = value; }
    }
    public bool IsThisGemSpecial = false;
    private SquashAndStretch squashAndStretch;

    private void OnEnable()
    {
       // Debug.Log($"[Gem] OnEnable {name} pos={transform.position}", this);
    }

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
            // TO ADD AN EVENT ON SPECIAL GEM COLLECTION
        }

        GemManager.Instance.createdGems.Remove(this);
        gemOPool.Release(this);
    }


}
