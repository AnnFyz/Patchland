using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHealth : MonoBehaviour
{
    public Vector3 CenterOfBlock { get; set; }
    [SerializeField] int step = 3;
    public Transform[] generatedWaypoints;
    public float startHealth = 100f;
    public float currentHealth;
    public bool IsBlockDead = false;
    public float H_1; float difH_1;
    public float S_1; float difS_1;
    public float V_1; float difV_1;

    public float H_2; float difH_2;
    public float S_2; float difS_2;
    public float V_2; float difV_2;
    BlockPrefab block;
    public bool IsBlockInjuring = false;
    public bool HasDayingColor = false;
    public bool IsAttacking = false;
    public float ind_Vdif_1;
    public float ind_Sdif_1;
    public float ind_Vdif_2;
    public float ind_Sdif_2;
    private void Awake()
    {
        generatedWaypoints = new Transform[step];
        block = GetComponent<BlockPrefab>();
    }

    private void Start()
    {
        FillTheListOfWaypints();
        currentHealth = startHealth;
        Color.RGBToHSV(block.defaultColor, out H_1, out S_1, out V_1);
        Color.RGBToHSV(block.defaultBottomColor, out H_2, out S_2, out V_2);
        difH_1 = H_1;
        difS_1 = S_1;
        difV_1 = V_1;

        difH_2 = H_2;
        difS_2 = S_2;
        difV_1 = V_1;
    }

    public void FillTheListOfWaypints()
    {
        //Set Destination to generated waypoint in circle
        float angleStep = 360 / step;
        for (int i = 1; i < step + 1; i++)
        {
            GameObject generatedWaypoint = new GameObject();
            generatedWaypoint.transform.RotateAround(transform.position, Vector3.up, angleStep * i);
            Vector3 dir = (generatedWaypoint.transform.position - transform.position).normalized;
            generatedWaypoint.transform.position = transform.position + dir * 4;
            generatedWaypoints[i - 1] = generatedWaypoint.transform;
            generatedWaypoint.transform.SetParent(transform);
        }
    }

    public void Damage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, startHealth);
        V_1 -= 0.005f;
        S_1 -= 0.005f;
        S_2 -= 0.05f;
        V_2 += 0.05f;
        V_1 = Mathf.Clamp(V_1, 0.4f, 1f);
        S_1 = Mathf.Clamp(S_1, 0.01f, 0.75f);
        S_2 = Mathf.Clamp(S_2, 0.001f, 0.9f);
        V_2 = Mathf.Clamp(V_2, 0.025f, 0.75f);
        block.defaultColor = Color.HSVToRGB(H_1, S_1, V_1);
        block.defaultBottomColor = Color.HSVToRGB(H_2, S_2, V_2);
        if (currentHealth <= 0 ) { IsBlockDead = true; gameObject.GetComponent<MyGridBuildingSystem>().GetAllPlacedObjectsOnTheBlock(); }
        if(currentHealth < startHealth) { IsBlockInjuring = true; HasDayingColor = false; IsAttacking = true; }

        ind_Vdif_1 += 0.005f;
        ind_Sdif_1 += 0.005f;
        ind_Sdif_2 += 005f;
        ind_Vdif_2 += 0.5f;


        //ind_Vdif_1 = Mathf.Clamp(V_1, 0.4f, 1f);
        //ind_Sdif_1 = Mathf.Clamp(S_1, 0.01f, 0.75f); 
        //ind_Sdif_2 = Mathf.Clamp(S_2, 0.001f, 0.9f);
        //ind_Vdif_2 = Mathf.Clamp(V_2, 0.025f, 0.75f);
    }

    public void SetDyingColor()
    {
        if (!HasDayingColor && !IsAttacking)
        {
            Debug.Log("SetDyingColor!!!");

            difS_1 -= S_1;
            difV_1 -= V_1;

            difS_2 -= S_2;
            V_2 -= difV_2;
            //V_1 = Mathf.Clamp(V_1, 0.4f, 1f);
            //S_1 = Mathf.Clamp(S_1, 0.01f, 0.75f); 
            //S_2 = Mathf.Clamp(S_2, 0.001f, 0.9f);
            //V_2 = Mathf.Clamp(V_2, 0.025f, 0.75f);

            Color.RGBToHSV(block.defaultColor, out H_1, out S_1, out V_1);
            Color.RGBToHSV(block.defaultBottomColor, out H_2, out S_2, out V_2);

            S_1 -= difS_1;
            V_1 -= difV_1;
            S_2 -= difS_2;
            V_2 += difV_2;

            V_1 = Mathf.Clamp(V_1, 0.4f, 1f);
            S_1 = Mathf.Clamp(S_1, 0.01f, 0.75f);
            S_2 = Mathf.Clamp(S_2, 0.001f, 0.9f);
            V_2 = Mathf.Clamp(V_2, 0.025f, 0.75f);

            block.defaultColor = Color.HSVToRGB(H_1, S_1, V_1);
            block.defaultBottomColor = Color.HSVToRGB(H_2, S_2, V_2);
            HasDayingColor = true;
        }
        else if (HasDayingColor)
        {
            ConvertInDyingColor();
        }
    }

    void ConvertInDyingColor()
    {
        Debug.Log("ConvertInDyingColor");
        float H1; float S1; float V1;
        float H2; float S2; float V2;
        Color.RGBToHSV(block.defaultColor, out H1, out S1, out V1);
        Color.RGBToHSV(block.defaultBottomColor, out H2, out S2, out V2);
        S1 -= ind_Vdif_2;
        V1 -= ind_Vdif_1;

        S2 -= ind_Sdif_2;
        //V1 -= ind_Vdif_2;

        V1 = Mathf.Clamp(V_1, 0.4f, 1f);
        S1 = Mathf.Clamp(S_1, 0.01f, 0.75f);
        S2 = Mathf.Clamp(S_2, 0.001f, 0.9f);
        V2 = Mathf.Clamp(V_2, 0.025f, 0.75f);
        block.defaultColor = Color.HSVToRGB(H1, S1, V1);
        block.defaultBottomColor = Color.HSVToRGB(H2, S2, V2);
    }
}
