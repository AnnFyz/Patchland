using ChristinaCreatesGames.Animations;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


public class Gem : MonoBehaviour
{
    [SerializeField]
    GameObject gemModel, onPickUp_VFX, onTimeExpired_VFX, UIText;
    private IObjectPool<Gem> gemOPool;
    public IObjectPool<Gem> GemPool
    {
        set { gemOPool = value; }
    }

    private SquashAndStretch squashAndStretch;
    private bool isSpecialGem;
    [ReadOnly] private float lifeTime;
    [ReadOnly] private float currentLifetime;
    private Coroutine lifetimeRoutine;
    private WorldUIHandler worldUIHandler;
    [SerializeField] bool isCollected = false;
    private void Awake()
    {
        squashAndStretch = GetComponentInChildren<SquashAndStretch>();
        worldUIHandler = GetComponent<WorldUIHandler>();
        onPickUp_VFX.SetActive(false);
    }

    public void Setup(GemsSO gemSO)
    {
        StopAllCoroutines();
        gemModel.SetActive(true);
        onPickUp_VFX.SetActive(false);
        onTimeExpired_VFX.SetActive(false);
        UIText.SetActive(true);
        isSpecialGem = gemSO.isSpecialGem;
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
        if (isSpecialGem)
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
        if(isCollected) return;
        Debug.Log("Gem Collected");
        isCollected = true;
        StopAllCoroutines();
        StartCoroutine(CollectGemRoutine());
    }

    void ToggleGemVisibility(bool isVisible)
    {
        gemModel.SetActive(isVisible);

    }

    void ToggleParticles(bool toPlay, bool onCollected)
    {
        GameObject particlesToPlay = onCollected ? onPickUp_VFX : onTimeExpired_VFX;
        particlesToPlay.SetActive(true);
        if (toPlay)
        {
            particlesToPlay.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            particlesToPlay.GetComponent<ParticleSystem>().Stop();
            particlesToPlay.GetComponent<ParticleSystem>().Clear();
            particlesToPlay.SetActive(false);

        }
    }

    IEnumerator HandleLifeTimeExpired(bool onCollected)
    {
        ToggleGemVisibility(false);
        UIText.SetActive(false);
        ToggleParticles(true, onCollected);
        yield return new WaitForSeconds(5f);
        ToggleParticles(false, onCollected);
        GemManager.Instance.createdGems.Remove(this);
        gemOPool.Release(this);
    }
    private IEnumerator CollectGemRoutine()
    {
        currentLifetime = 0f;

        UIManager.Instance.CollectGem();

        if (isSpecialGem)
        {
            UIManager.Instance.CollectSpecialGem();
        }
        else
        {
            UIManager.Instance.CollectGem();
        }

        yield return new WaitForSeconds(0f);
        StartCoroutine(HandleLifeTimeExpired(true));
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
        StartCoroutine(HandleLifeTimeExpired(false));
    }


}
