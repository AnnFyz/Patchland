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
    float H_1; float oldH_1;
    float S_1; float oldS_1;
    float V_1; float oldV_1;

    float H_2; float oldH_2;
    float S_2; float oldS_2;
    float V_2; float oldV_2;
    BlockPrefab block;
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
        if (currentHealth <= 0 ) { IsBlockDead = true; }
    }
}
