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
    [SerializeField] float startValue = 100f;
    [SerializeField] float curretValue;
    const float maxValue = 100f;
    public bool isFoodAround = false;
    public Action OnUnitDeath;
    Unit unit;
    float damageToUnit;
    [SerializeField] UIState currentUIState = UIState.healthy;
    public GameObject stateFire;
    [SerializeField] GameObject whenAttacked_Particles;
    public GameObject whenDead_Particles;
    public bool isAttacked = false;
    string[] openLines = new string[7];
    private void Awake()
    {
        unit = GetComponentInParent<Unit>();
        damageToUnit = unit.unitScriptableObject.damageToUnitWithoutFood;
        stateFire = gameObject.transform.GetChild(1).GetChild(0).gameObject;
    }

    void SwitchUIState()
    {
        if (curretValue > 80)
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
            Bubble.Instance.CreateBubble(transform.position, "I'm tired \n 💚"); 

        }
        else if (curretValue < 50 && curretValue > 0)
        {
            currentUIState = UIState.veryHungry;
            stateFire.SetActive(false);
            stateFire = gameObject.transform.GetChild(1).GetChild(2).gameObject;
            stateFire.SetActive(true);
            Bubble.Instance.CreateBubble(transform.position, char.ConvertFromUtf32(0x1F92C));

        }
        else if (curretValue <= 0)
        {
            if (unit.currentUnitsState == UnitsState.Zombi)
            {
                currentUIState = UIState.zombi;
                stateFire.SetActive(false);
                stateFire = gameObject.transform.GetChild(1).GetChild(3).gameObject;
                stateFire.SetActive(true); //i want to come back
                Bubble.Instance.CreateBubble(transform.position, "oh no I'm dying");

            }
            else
            {
                stateFire.SetActive(false);
            }
        }
    }
    private void Start()
    {
        curretValue = startValue;
        whenAttacked_Particles.SetActive(false);
        whenDead_Particles.SetActive(false);
        OpenLines();
        Bubble.Instance.CreateBubble(transform.position, openLines[2]);
    }

    private void FixedUpdate()
    {
        SwitchUIState();
        if (!isFoodAround && curretValue > 0)
        {
            LoseHealth();
        }
    }

    void OpenLines()
    {
        openLines[2] = " (•̤̀ᵕ•̤́) "; 
        openLines[2] = " (•̤̀ᵕ•̤́) ";
    }
    private void LoseHealth()
    {
        StartCoroutine(SubstractHealthGradually());
        if (curretValue <= 0)
        {
            unit.currentUnitsState = UnitsState.Dead; // then the dead unit have a change to comeback as a zombi, to write Zombi class
            if (unit.currentUnitsState != UnitsState.Zombi)
            {
                OnUnitDeath?.Invoke();
            }
        }
    }

    IEnumerator SubstractHealthGradually()
    {
        curretValue -= damageToUnit;
        curretValue = Mathf.Clamp(curretValue, 0, maxValue);
        yield return new WaitForSeconds(1f);
    }
    public void FillHealth(float value)
    {
        curretValue += value;
        curretValue = Mathf.Clamp(curretValue, 0, maxValue);
        SwitchUIState();
    }

    public IEnumerator FillHealthGradually()
    {
        FillHealth(0.5f);
        yield return new WaitForSeconds(1f);
    }

    private void OnCollisionStay(Collision other)
    {

        if (unit.currentUnitsState != UnitsState.Zombi && other.gameObject.GetComponent<Zombi>() && other.gameObject.GetComponent<Zombi>().currentState != ZombiState.None)
        {
            Debug.Log("ANOTHER UNIT");
            StartCoroutine(SubstractHealthGradually());
            whenAttacked_Particles.SetActive(true);
            isAttacked = true;
            if (curretValue <= 0)
            {
                unit.currentUnitsState = UnitsState.Dead; // then the dead unit have a change to comeback as a zombi, to write Zombi class
                if (unit.currentUnitsState != UnitsState.Zombi)
                {
                    OnUnitDeath?.Invoke();
                }
            }
        }
    
        else if (unit.currentUnitsState != UnitsState.Zombi && other.gameObject.GetComponentInParent<BlockHealth>())
        {
            if (other.gameObject.GetComponentInParent<BlockHealth>().IsBlockDead)
            {
                Debug.Log("DeadBlock!");
                StartCoroutine(SubstractHealthGradually());
                whenAttacked_Particles.SetActive(true);
                isAttacked = true;
                if (curretValue <= 0)
                {
                    unit.currentUnitsState = UnitsState.Dead; // then the dead unit have a change to comeback as a zombi, to write Zombi class
                    if (unit.currentUnitsState != UnitsState.Zombi)
                    {
                        OnUnitDeath?.Invoke();
                    }
                }
            }
        }
  
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.GetComponent<Zombi>() && other.gameObject.GetComponent<Zombi>().currentState != ZombiState.None)

        {
            StopCoroutine(SubstractHealthGradually());
            whenAttacked_Particles.SetActive(false);
            isAttacked = false;
            if (curretValue <= 0)
            {
                unit.currentUnitsState = UnitsState.Dead; // then the dead unit have a change to comeback as a zombi, to write Zombi class
                if (unit.currentUnitsState != UnitsState.Zombi)
                {
                    OnUnitDeath?.Invoke();
                }
            }
        }

       else if (other.gameObject.GetComponentInParent<BlockHealth>())
        {

            if (other.gameObject.GetComponentInParent<BlockHealth>().IsBlockDead)
            {
                StopCoroutine(SubstractHealthGradually());
                whenAttacked_Particles.SetActive(false);
                isAttacked = false;
                if (curretValue <= 0)
                {
                    unit.currentUnitsState = UnitsState.Dead; // then the dead unit have a change to comeback as a zombi, to write Zombi class
                    if (unit.currentUnitsState != UnitsState.Zombi)
                    {
                        OnUnitDeath?.Invoke();
                    }
                }
            }
    
        }
    }
}
