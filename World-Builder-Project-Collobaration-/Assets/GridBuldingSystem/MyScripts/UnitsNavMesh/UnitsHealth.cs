using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitsHealth : MonoBehaviour
{
    [SerializeField] Image levelBar;
    [SerializeField] float startValue = 100f;
    [SerializeField] float curretValue;
    const float maxValue = 100f;
    public bool isFoodAround = true;
    private void Awake()
    {
        levelBar = GetComponent<Image>();
    }
    private void Start()
    {
        levelBar.fillAmount = startValue;
        curretValue = startValue;
    }

    private void Update()
    {
        if (!isFoodAround)
        {
            LoseHealth(0.01f);
        }
       
    }
    private void LoseHealth(float value)
    {
        curretValue -= value;
        curretValue = Mathf.Clamp(curretValue, 0, maxValue);
        levelBar.fillAmount = curretValue / maxValue;
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
