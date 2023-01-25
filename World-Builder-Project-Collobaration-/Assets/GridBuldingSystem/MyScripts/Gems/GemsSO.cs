using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GemsSO : ScriptableObject
{
    public int weight;
    public GameObject gemPrefab;
    public bool IsThisGemSpecial = false;
}
