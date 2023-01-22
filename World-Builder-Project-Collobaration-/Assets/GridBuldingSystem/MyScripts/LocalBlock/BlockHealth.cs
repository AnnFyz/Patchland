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
    private void Awake()
    {
        generatedWaypoints = new Transform[step];
    }

    private void Start()
    {
        FillTheListOfWaypints();
        currentHealth = startHealth;
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
        if(currentHealth <= 0 ) { IsBlockDead = true; }
    }
}
