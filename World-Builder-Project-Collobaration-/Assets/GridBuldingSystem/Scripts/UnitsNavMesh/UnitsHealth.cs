using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

public enum UIState
{
    healthy,
    hungry,
    veryHungry,
    zombi
}
public class UnitsHealth : MonoBehaviour
{
    private float maxValue = 100f;
    public float MaxValue => maxValue;
    private float curretValue;
    public float CurretValue => curretValue;

    public  bool IsFoodAround { get; set; }
    public Action OnUnitDeath;
    Unit unit;
    private float damageToUnit;
    private float healthToUnit;
    [SerializeField] UIState currentUIState = UIState.healthy;
    public GameObject stateFire;
    [SerializeField] GameObject whenAttacked_Particles;
    public GameObject whenDead_Particles;
    public bool isAttacked = false;
    string[] openLines = new string[7];
    string[] hungryLines = new string[3];
    string[] angryLines = new string[3];
    bool WasHungryBubbleCreated = false;
    bool WasAngryBubbleCreated = false;
    bool isHealthFilling = false;
    public bool isHealthLosing = false;
    private void Awake()
    {
        unit = GetComponentInParent<Unit>();
        damageToUnit = unit.UnitScriptableObject.damageToUnitWithoutFood;
        healthToUnit = unit.UnitScriptableObject.healthPointsFromFood;
        stateFire = gameObject.transform.GetChild(1).GetChild(0).gameObject;
        IsFoodAround = true;
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
            if (unit.CurrentUnitsState == UnitsState.Zombi)
            {
                currentUIState = UIState.zombi;
                stateFire.SetActive(false);
                stateFire = gameObject.transform.GetChild(1).GetChild(3).gameObject;
                stateFire.SetActive(true); 
            }
        }
    }

    void CheckUIState()
    {
        SwitchUIState();

        if (currentUIState == UIState.hungry)
        {
            if (!WasHungryBubbleCreated)
            {
                Bubble.Instance.CreateBubble(transform.position, hungryLines[UnityEngine.Random.Range(0, hungryLines.Length - 1)]);
                WasHungryBubbleCreated = true;
            }
        }
        if (currentUIState == UIState.veryHungry)
        {
            if (!WasAngryBubbleCreated)
            {
                Bubble.Instance.CreateBubble(transform.position, angryLines[UnityEngine.Random.Range(0, angryLines.Length - 1)]);
                WasAngryBubbleCreated = true;
            }
        }
    }

    private void Start()
    {
        curretValue = maxValue;
        whenAttacked_Particles.SetActive(false);
        CreateOpenLines();
        CreateHungryLines();
        CreateAngryLines();
        Bubble.Instance.CreateBubble(transform.position, openLines[UnityEngine.Random.Range(0, openLines.Length -1)]);
    }


    void CreateOpenLines()
    {
        openLines[0] = "  (•̤̀ᵕ•̤́)  "; 
        openLines[1] = " 😊 ";
        openLines[2] = "  ( ˙˘˙) "; 
        openLines[3] = "  ʕ•ᴥ•ʔ ";
        openLines[4] = "  ♥ ";  
        openLines[5] = "(• ε •)";

    }

    void CreateHungryLines()
    {
        hungryLines[0] = " (╥ _ ╥) "; // (.•́ _•̀.)
        hungryLines[1] = " (.•́ _•̀.) ";  // (._.)#
        hungryLines[1] = " (._.) ";  // (._.)

    }

    void CreateAngryLines()
    {
        angryLines[0] = " (Ο_Ο) "; // (.•́ _•̀.)
        angryLines[1] = " (°0°) ";  // (._.)#
        angryLines[1] = " ☹ ";  // (._.)

    }
    public void LoseHealth()
    {
        StartCoroutine(SubstractHealthGradually());
    }

    IEnumerator SubstractHealthGradually()
    {
        while (curretValue > 0 && !IsFoodAround)
        {
            curretValue -= damageToUnit;
            curretValue = Mathf.Clamp(curretValue, 0, maxValue);
            CheckUIState();
            yield return new WaitForSeconds(1f);
        }
        if (curretValue <= 0)
        {
            unit.CurrentUnitsState = UnitsState.Dead; 
            if (unit.CurrentUnitsState != UnitsState.Zombi)
            {
                OnUnitDeath?.Invoke();
            }
        }
        CheckUIState();
    }

    public IEnumerator FillHealthGradually()
    {
        while (curretValue < maxValue && IsFoodAround)
        {
            curretValue += healthToUnit;
            curretValue = Mathf.Clamp(curretValue, 0, maxValue);
            CheckUIState();
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (unit.CurrentUnitsState == UnitsState.Zombi) { return; }
        if (unit.CurrentUnitsState != UnitsState.Zombi && other.gameObject.GetComponent<Zombi>() && other.gameObject.GetComponent<Zombi>().currentState != ZombiState.None)
        {
            whenAttacked_Particles.SetActive(true);
            isAttacked = true;
            if (curretValue <= 0)
            {
                unit.CurrentUnitsState = UnitsState.Dead; // then the dead unit have a change to comeback as a zombi, to write Zombi class
                if (unit.CurrentUnitsState != UnitsState.Zombi)
                {
                    OnUnitDeath?.Invoke();
                }
            }
        }
    
        
        else if (unit.CurrentUnitsState != UnitsState.Zombi && other.gameObject.GetComponentInParent<BlockHealth>())
        {
            if (other.gameObject.GetComponentInParent<BlockHealth>().IsBlockDead)
            {
                whenAttacked_Particles.SetActive(true);
                isAttacked = true;
                if (curretValue <= 0)
                {
                    unit.CurrentUnitsState = UnitsState.Dead; // then the dead unit have a change to comeback as a zombi, to write Zombi class
                    if (unit.CurrentUnitsState != UnitsState.Zombi)
                    {
                        OnUnitDeath?.Invoke();
                    }
                }
            }
        }
  
    }

    void OnCollisionExit(Collision other)
    {
        if (unit.CurrentUnitsState == UnitsState.Zombi) { return; }
        if (other.gameObject.GetComponent<Zombi>() && other.gameObject.GetComponent<Zombi>().currentState != ZombiState.None)

        {
            //StopCoroutine(SubstractHealthGradually());
            whenAttacked_Particles.SetActive(false);
            isAttacked = false;
            if (curretValue <= 0)
            {
                unit.CurrentUnitsState = UnitsState.Dead; // then the dead unit have a change to comeback as a zombi, to write Zombi class
                if (unit.CurrentUnitsState != UnitsState.Zombi)
                {
                    OnUnitDeath?.Invoke();
                }
            }
        }

       else if (other.gameObject.GetComponentInParent<BlockHealth>())
        {

            if (other.gameObject.GetComponentInParent<BlockHealth>().IsBlockDead)
            {
                //StopCoroutine(SubstractHealthGradually());
                whenAttacked_Particles.SetActive(false);
                isAttacked = false;
                if (curretValue <= 0)
                {
                    unit.CurrentUnitsState = UnitsState.Dead; // then the dead unit have a change to comeback as a zombi, to write Zombi class
                    if (unit.CurrentUnitsState != UnitsState.Zombi)
                    {
                        OnUnitDeath?.Invoke();
                    }
                }
            }
    
        }
    }
}
