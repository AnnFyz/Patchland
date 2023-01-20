using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UnitsHealth : MonoBehaviour
{
    [SerializeField] Image levelBar;
    [SerializeField] float startValue = 100f;
    [SerializeField] float curretValue;
    const float maxValue = 100f;
    public bool isFoodAround = false;
    public Action OnUnitDeath;
    Unit unit;
    float damageToUnit;
    private void Awake()
    {
        levelBar = GetComponent<Image>();
        unit = GetComponentInParent<Unit>();
        damageToUnit = unit.unitScriptableObjects.damageToUnitWithoutFood;
    }
    private void Start()
    {
        levelBar.fillAmount = startValue;
        curretValue = startValue;
    }

    private void FixedUpdate()
    {
        if (!isFoodAround && curretValue > 0)
        {
            LoseHealth();
        }       
    }
    private void LoseHealth()
    {
        StartCoroutine(SubstractHealthGradually());
        if (curretValue <= 0)
        {
            unit.currentUnitsState = UnitsState.Dead; // then the dead unit have a change to comeback as a zombi, to write Zombi class
            if(unit.currentUnitsState != UnitsState.Zombi)
            {
                OnUnitDeath?.Invoke();
            }
        }
    }

    IEnumerator SubstractHealthGradually()
    {
        curretValue -= damageToUnit;
        curretValue = Mathf.Clamp(curretValue, 0, maxValue);
        levelBar.fillAmount = curretValue / maxValue;
        yield return new WaitForSeconds(1f);
    }
    public void FillHealth(float value)
    {
        curretValue += value;
        curretValue = Mathf.Clamp(curretValue, 0, maxValue);
        levelBar.fillAmount = curretValue / maxValue;
    }

    public IEnumerator FillHealthGradually()
    {
        FillHealth(0.5f);
        yield return new WaitForSeconds(1f);
    }
}
