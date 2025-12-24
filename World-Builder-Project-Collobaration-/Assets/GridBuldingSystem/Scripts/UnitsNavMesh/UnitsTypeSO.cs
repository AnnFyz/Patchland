using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu()] //fileName = "Unit Configuration", menuName = "ScriptableObject/Unit Configuration")]
public class UnitsTypeSO : ScriptableObject
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float damageToUnitWithoutFood = 1f;
    public float healthPointsFromFood = 1f;
    public float delayBeforeHealthLoss = 1.5f;
    public GameObject death_Particles_Prefab;
    [Range(0.0f, 100.0f)]
    public float chanceToBecomeZombi;
    public float damageToBlock = 1f;
    public float attackDelay = 0.1f;
    [Header("NavMeshAgent Configs")]
    public int avoidancePriority = 50;
    public ObstacleAvoidanceType obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    [Header("Unit Settings")]
    public float minMovingToPointTimer = 2f;
    public float maxMovingToPointTimer = 5f;

}
