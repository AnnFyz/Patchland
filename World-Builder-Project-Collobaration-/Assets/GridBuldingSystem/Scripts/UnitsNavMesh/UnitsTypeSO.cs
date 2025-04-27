using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu()] //fileName = "Unit Configuration", menuName = "ScriptableObject/Unit Configuration")]
public class UnitsTypeSO : ScriptableObject
{
    [Header("Health Settings")]
    public int unitId;
    public float damageToUnitWithoutFood = 1f;
    public ParticleSystem death_Particles;
    [Range(0.0f, 100.0f)]
    public float chanceToBecomeZombi;
    public Sprite UIHealthSp; // currently not in use
    [Header("NavMeshAgent Configs")]
    public float AIUpdateInterval = 0.1f;
    public float acceleration = 18f;
    public float angularSpeed = 120f;
    public int areaMask = -1; // -1 means everything
    public int avoidancePriority = 50;
    public float baseOffset = 0.5f;
    public float height = 1f;
    public ObstacleAvoidanceType obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    public float radius = 1.05f;
    public float speed = 13f;
    public float stoppingDistance = 0.5f;
    public float triggerRadius = 0.15f;  // currently not in use

}
