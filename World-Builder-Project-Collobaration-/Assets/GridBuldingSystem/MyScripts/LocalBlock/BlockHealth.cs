using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHealth : MonoBehaviour
{
    public Vector3 CenterOfBlock { get; set; }
    [SerializeField] int step = 3;
    public Transform[] generatedWaypoints;
    private void Awake()
    {
        generatedWaypoints = new Transform[step];
    }

    private void Start()
    {
        FillTheListOfWaypints();
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
}
