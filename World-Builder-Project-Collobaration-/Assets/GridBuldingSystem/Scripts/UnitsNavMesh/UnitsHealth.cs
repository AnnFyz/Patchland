using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

public enum UIState
{
    Healthy,
    Hungry,
    CriticalHungry,
    Zombi
}

/// <summary>
/// Represents the health system for a unit. 
/// Handles UI state, bubbles, gradual health gain/loss, and death.
/// </summary>
public class UnitsHealth : MonoBehaviour
{
    [SerializeField] private UIState currentUIState = UIState.Healthy; // Current UI state of the unit
    private GameObject stateFire; // UI element that represents the health state of the unit
    [SerializeField] float currentHealth;
    public float CurrentHealth => currentHealth;
    private float maxHealth;
    public float MaxHealth => maxHealth;
    private float damageToUnit;
    private float healthToUnit;
    public bool IsFoodAround;
    public Action OnUnitDeath;
    Unit unit;
    private readonly string[] openLines = new string[6];
    private readonly string[] linesForHungryState = new string[3];
    private readonly string[] linesForVeryHungryState = new string[3];
    bool wasBubbleForHungryStateCreated = false;
    bool wasBubbleForVeryHungryStateCreated = false;
    public bool isHealthLosing = false;
    private void Awake()
    {
        unit = GetComponentInParent<Unit>();
        stateFire = transform.GetChild(1).GetChild(0).gameObject;
        IsFoodAround = true;
        SetupUnitHealthFromConfiguration();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        CreateOpenLines();
        CreateLinesForHungryState();
        CreateLinesForVeryHungryState();
        Bubble.Instance.CreateBubble(transform.position, openLines[UnityEngine.Random.Range(0, openLines.Length)]);
    }

    void SetupUnitHealthFromConfiguration()
    {
        maxHealth = unit.UnitScriptableObject.maxHealth;
        damageToUnit = unit.UnitScriptableObject.damageToUnitWithoutFood;
        healthToUnit = unit.UnitScriptableObject.healthPointsFromFood;
    }

    // This method is called to switch the UI state based on the current health of the unit.
    void SwitchUIState()
    {
        void SetFire(int childIndex, UIState newState)
        {
            stateFire.SetActive(false);
            stateFire = transform.GetChild(1).GetChild(childIndex).gameObject;
            stateFire.SetActive(true);
            currentUIState = newState;
        }


        if (currentHealth > 80)
            SetFire(0, UIState.Healthy);
        else if (currentHealth <= 80 && currentHealth >= 50)
            SetFire(1, UIState.Hungry);
        else if (currentHealth < 50 && currentHealth > 0)
            SetFire(2, UIState.CriticalHungry);
        else if (currentHealth <= 0 && unit.CurrentUnitsState == UnitsState.Zombi)
            SetFire(3, UIState.Zombi);
    }

    /// <summary>Checks state and spawns bubbles appropriately.</summary>
    void CheckUIState()
    {
        SwitchUIState();

        if (currentUIState == UIState.Healthy)
        {
            wasBubbleForHungryStateCreated = false;
            wasBubbleForVeryHungryStateCreated = false;
        }

        if (currentUIState == UIState.Hungry)
        {
            if (!wasBubbleForHungryStateCreated)
            {
                Bubble.Instance.CreateBubble(transform.position, linesForHungryState[UnityEngine.Random.Range(0, linesForHungryState.Length - 1)]);
                wasBubbleForHungryStateCreated = true;
                wasBubbleForVeryHungryStateCreated = false;
            }
        }
        if (currentUIState == UIState.CriticalHungry)
        {
            if (!wasBubbleForVeryHungryStateCreated)
            {
                Bubble.Instance.CreateBubble(transform.position, linesForVeryHungryState[UnityEngine.Random.Range(0, linesForVeryHungryState.Length - 1)]);
                wasBubbleForVeryHungryStateCreated = true;
                wasBubbleForHungryStateCreated = false;
            }
        }
    }

    // This method is called to create the initial open lines for the unit's health UI.
    void CreateOpenLines()
    {
        openLines[0] = "  (•̤̀ᵕ•̤́)  ";
        openLines[1] = " 😊 ";
        openLines[2] = "  ( ˙˘˙) ";
        openLines[3] = "  ʕ•ᴥ•ʔ ";
        openLines[4] = "  ♥ ";
        openLines[5] = "(• ε •)";

    }

    // This method is called to create the lines for the hungry state of the unit's health UI.
    void CreateLinesForHungryState()
    {
        linesForHungryState[0] = " (╥ _ ╥) "; // (.•́ _•̀.)
        linesForHungryState[1] = " (.•́ _•̀.) ";  // (._.)#
        linesForHungryState[2] = " (._.) ";  // (._.)

    }

    // This method is called to create the lines for the critical state of the unit's health UI.
    void CreateLinesForVeryHungryState()
    {
        linesForVeryHungryState[0] = " (Ο_Ο) "; // (.•́ _•̀.)
        linesForVeryHungryState[1] = " (°0°) ";  // (._.)#
        linesForVeryHungryState[2] = " ☹ ";  // (._.)

    }

    /// <summary>Begins gradual health loss coroutine when no food is nearby.</summary>
    public void LoseHealth(float delayBeforeLosingHealth)
    {
        StartCoroutine(SubtractHealthGradually(delayBeforeLosingHealth));
    }

    IEnumerator SubtractHealthGradually(float delayBeforeLosingHealth)
    {
        yield return new WaitForSeconds(delayBeforeLosingHealth);
        while (currentHealth > 0 && !IsFoodAround)
        {
            currentHealth -= damageToUnit;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            CheckUIState();
            yield return new WaitForSeconds(1f);
        }
        if (currentHealth <= 0)
        {
            unit.CurrentUnitsState = UnitsState.Dead;
            OnUnitDeath?.Invoke();
        }

        CheckUIState();
    }

    /// <summary>Gradually restores health when food is nearby.</summary>
    public IEnumerator FillHealthGradually()
    {
        while (currentHealth < maxHealth && IsFoodAround)
        {
            currentHealth += healthToUnit;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            CheckUIState();
            yield return new WaitForSeconds(1f);
        }
    }
}
