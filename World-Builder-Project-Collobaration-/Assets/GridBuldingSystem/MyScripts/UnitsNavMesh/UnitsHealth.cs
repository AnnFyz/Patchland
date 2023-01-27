using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum UIState
{
    healthy,
    hungry,
    veryHungry,
    zombi
}
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
    [SerializeField] UIState currentUIState = UIState.healthy;
    public GameObject stateFire;
    private void Awake()
    {
        //levelBar = GetComponent<Image>();
        unit = GetComponentInParent<Unit>();
        damageToUnit = unit.unitScriptableObjects.damageToUnitWithoutFood;
        stateFire = gameObject.transform.GetChild(1).GetChild(0).gameObject;
    }

    void SwitchUIState()
    {
       if(curretValue > 80)
        {
           
            currentUIState = UIState.healthy;
            stateFire.SetActive(false);
            stateFire = gameObject.transform.GetChild(1).GetChild(0).gameObject;
            stateFire.SetActive(true);

        }
       else if (curretValue <= 80 && curretValue >= 50)
        {
            currentUIState = UIState.hungry;
            stateFire.SetActive(false);
            stateFire = gameObject.transform.GetChild(1).GetChild(1).gameObject;
            stateFire.SetActive(true);

        }
        else if (curretValue < 50 && curretValue > 0)
        {
            currentUIState = UIState.veryHungry;
            stateFire.SetActive(false);
            stateFire = gameObject.transform.GetChild(1).GetChild(2).gameObject;
            stateFire.SetActive(true);

        }
       else if (curretValue <= 0)
        {
            if(unit.currentUnitsState == UnitsState.Zombi)
            {
                currentUIState = UIState.zombi;
                stateFire.SetActive(false);
                stateFire = gameObject.transform.GetChild(1).GetChild(3).gameObject;
                stateFire.SetActive(true);

            }
            else
            {
                stateFire.SetActive(false);
            }
        }
    }
    private void Start()
    {
        levelBar.fillAmount = startValue;
        curretValue = startValue;
    }

    private void FixedUpdate()
    {
        SwitchUIState();
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
        SwitchUIState();
    }

    public IEnumerator FillHealthGradually()
    {
        FillHealth(0.5f);
        yield return new WaitForSeconds(1f);
    }
}
