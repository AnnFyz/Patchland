using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class DayAndNightController : MonoBehaviour
{
    public static DayAndNightController Instance { get; private set; }
    [SerializeField] Material skyboxOnRunTime;
    [SerializeField] Color topColor;
    [SerializeField] Color bottomColor;
    [SerializeField] Material backUpMaterial;
    [SerializeField] Color oldTopColor;
    [SerializeField] Color oldBottomColor;
    public float H_1; 
    public float S_1; 
    public float V_1;

    public float H_2; 
    public float S_2; 
    public float V_2;
    public bool isSunrise = false;
    public bool isSunset = false;
    public Action isTimeToSpawnGems;
    [SerializeField] float timeOfDay = 20;

    [SerializeField] float timeOfNight = 0;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        //backUpMaterial = makeSkyboxBackUp();
        Material originalSkybox = RenderSettings.skybox;
        skyboxOnRunTime = Instantiate(originalSkybox);
        RenderSettings.skybox = skyboxOnRunTime;
        topColor  = skyboxOnRunTime.GetColor(Shader.PropertyToID("_Color1"));
        bottomColor = skyboxOnRunTime.GetColor(Shader.PropertyToID("_Color2"));
        oldTopColor = topColor;
        oldBottomColor = bottomColor;

        Color.RGBToHSV(topColor, out H_1, out S_1, out V_1);
        Color.RGBToHSV(bottomColor, out H_2, out S_2, out V_2);
        StartCoroutine(Sunset());
 
    }
    private void Update()
    {
        if (isSunrise)
        {
            StartCoroutine(Sunrise());
        }
        if (isSunset)
        {
            StartCoroutine(Sunset());
        }

    }

    IEnumerator Sunset()
    {
      
            while (timeOfNight < 20)
            {
                isSunset = false;
                DynamicGI.UpdateEnvironment();
                V_1 -= 0.01f;
                V_2 -= 0.01f;

                V_1 = Mathf.Clamp(V_1, 0.35f, 0.85f);
                V_2 = Mathf.Clamp(V_2, 0.35f, 0.9f);

                timeOfNight += 0.25f;
                if(timeOfNight == 10) { isTimeToSpawnGems?.Invoke(); }
                topColor = Color.HSVToRGB(H_1, S_1, V_1);
                bottomColor = Color.HSVToRGB(H_2, S_2, V_2);
                skyboxOnRunTime.SetColor((Shader.PropertyToID("_Color1")), topColor);
                skyboxOnRunTime.SetColor((Shader.PropertyToID("_Color2")), bottomColor);
                yield return new WaitForSeconds(0.5f);
            }

        isSunrise = true;
        timeOfNight = 0;
    }

    IEnumerator Sunrise()
    {

        while (timeOfDay > 0)
        {
            isSunrise = false;
            DynamicGI.UpdateEnvironment();
            V_1 += 0.005f;
            V_2 += 0.005f;

            V_1 = Mathf.Clamp(V_1, 0.35f, 0.85f);
            V_2 = Mathf.Clamp(V_2, 0.35f, 0.9f);


            timeOfDay -= 0.25f;

            topColor = Color.HSVToRGB(H_1, S_1, V_1);
            bottomColor = Color.HSVToRGB(H_2, S_2, V_2);
            skyboxOnRunTime.SetColor((Shader.PropertyToID("_Color1")), topColor);
            skyboxOnRunTime.SetColor((Shader.PropertyToID("_Color2")), bottomColor);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(30f);
        isSunset = true;
        timeOfDay = 20;
    }


}
