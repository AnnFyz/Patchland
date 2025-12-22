using ChristinaCreatesGames.Animations;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


public class Gem : MonoBehaviour
{
    [SerializeField]
    GameObject gemModel, particleObject;
    private IObjectPool<Gem> gemOPool;
    public IObjectPool<Gem> GemPool
    {
        set { gemOPool = value; }
    }

    private SquashAndStretch squashAndStretch;
    private bool IsThisGemSpecial;
    [ReadOnly] private float lifeTime;
    [ReadOnly] private float currentLifetime;
    private Coroutine lifetimeRoutine;
    private WorldUIHandler worldUIHandler;
    private void Awake()
    {
        squashAndStretch = GetComponentInChildren<SquashAndStretch>();
        worldUIHandler = GetComponent<WorldUIHandler>();
        particleObject.SetActive(false);
    }

    public void Setup(GemsSO gemSO)
    {
        StopAllCoroutines();
        gemModel.SetActive(true);
        particleObject.SetActive(false);
        IsThisGemSpecial = gemSO.isSpecialGem;
        lifeTime = gemSO.lifeTime;
        currentLifetime = lifeTime;
        worldUIHandler.SetText(currentLifetime.ToString(), false);
        lifetimeRoutine = StartCoroutine(StartLifeTimeCountdown(lifeTime));
    }
    public void SquashAndStretch()
    {
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
        squashAndStretch.PlaySquashAndStretch();
    }
    public void CollectGem()
    {
        StopAllCoroutines();
        StartCoroutine(CollectGemRoutine());
    }

    void ToggleGemVisibility(bool isVisible)
    {
        gemModel.SetActive(isVisible);

    }

    void ToggleParticles(bool toPlay)
    {
        particleObject.SetActive(true);
        if (toPlay)
        {
            particleObject.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            particleObject.GetComponent<ParticleSystem>().Stop();
            particleObject.GetComponent<ParticleSystem>().Clear();
            particleObject.SetActive(false);

        }
    }

    IEnumerator HandleLifeTimeExpired()
    {
        ToggleGemVisibility(false);
        ToggleParticles(true);
        yield return new WaitForSeconds(5f);
        ToggleParticles(false);

        GemManager.Instance.createdGems.Remove(this);
        gemOPool.Release(this);
    }
    private IEnumerator CollectGemRoutine()
    {
        currentLifetime = 0f;

        UIManager.Instance.CollectGem();

        if (IsThisGemSpecial)
        {
            UIManager.Instance.CollectSpecialGem();
        }

        yield return StartCoroutine(PlayAnimation());
        StartCoroutine(HandleLifeTimeExpired());
    }


    IEnumerator StartLifeTimeCountdown(float lifeTime)
    {

        while (currentLifetime > 0f)
        {
            currentLifetime -= Time.deltaTime;
            worldUIHandler.SetText(Mathf.CeilToInt(currentLifetime).ToString(), false);
            yield return null;
        }

        currentLifetime = 0f;
        worldUIHandler.SetText(Mathf.CeilToInt(currentLifetime).ToString(), true);
        yield return StartCoroutine(PlayAnimation());
        StartCoroutine(HandleLifeTimeExpired());
    }


}
