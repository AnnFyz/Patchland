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
    string[] hungryLines = new string[3];
    string[] angryLines = new string[3];
    bool WasHungryBubbleCreated = false;
    bool WasAngryBubbleCreated = false;
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
            if (unit.currentUnitsState == UnitsState.Zombi)
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
        curretValue = startValue;
        whenAttacked_Particles.SetActive(false);
        CreateOpenLines();
        CreateHungryLines();
        CreateAngryLines();
        Bubble.Instance.CreateBubble(transform.position, openLines[UnityEngine.Random.Range(0, openLines.Length -1)]);
    }


    private void FixedUpdate()
    {
       
        SwitchUIState();
        if (!isFoodAround && curretValue > 0)
        {
            LoseHealth();
        }
        if(currentUIState == UIState.hungry)
        {
            Debug.Log("Hungry");
            if (!WasHungryBubbleCreated)
            {
                Bubble.Instance.CreateBubble(transform.position, hungryLines[UnityEngine.Random.Range(0, hungryLines.Length -1)]);
                WasHungryBubbleCreated = true;
            }
        }
        if (currentUIState == UIState.veryHungry)
        {
            Debug.Log("Very Hungry");
            if (!WasAngryBubbleCreated)
            {
                Bubble.Instance.CreateBubble(transform.position, angryLines[UnityEngine.Random.Range(0, angryLines.Length -1)]);
                WasAngryBubbleCreated = true;
            }
        }
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
