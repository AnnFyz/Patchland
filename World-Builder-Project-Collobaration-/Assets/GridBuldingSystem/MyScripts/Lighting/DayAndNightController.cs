using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayAndNightController : MonoBehaviour
{
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
        StartCoroutine(ChangeSky());
 
    }
    IEnumerator ChangeSky()
    {
        while (true)
        {
            Debug.Log("CHANGE COLOR");
            DynamicGI.UpdateEnvironment();
            V_1 -= 0.05f;
            S_1 -= 0.05f;
            H_1 -= 0.05f;

            S_2 -= 0.05f;
            V_2 -= 0.05f;
            H_2 -= 0.05f;

            V_1 = Mathf.Clamp(V_1, 0.001f, 0.9f);
            S_1 = Mathf.Clamp(S_1, 0.001f, 0.9f);
            H_1 = Mathf.Clamp(S_1, 0.001f, 0.9f);
            S_2 = Mathf.Clamp(S_2, 0.001f, 0.9f);
            V_2 = Mathf.Clamp(V_2, 0.001f, 0.9f);
            H_2 = Mathf.Clamp(V_2, 0.001f, 0.9f);

            topColor = Color.HSVToRGB(H_1, S_1, V_1);
            bottomColor = Color.HSVToRGB(H_2, S_2, V_2);
            skyboxOnRunTime.SetColor((Shader.PropertyToID("_Color1")), topColor);
            skyboxOnRunTime.SetColor((Shader.PropertyToID("_Color2")), bottomColor);
            yield return new WaitForSeconds(0.5f);
        }

    }

    //Restore skybox material back to the Default values
    void OnDisable()
    {
        DynamicGI.UpdateEnvironment();
    }

    Material makeSkyboxBackUp()
    {
        return new Material(RenderSettings.skybox);
    }

}
