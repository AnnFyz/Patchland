using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu]
public class GemsSO : ScriptableObject
{
    public int weight;
    public Gem gemPrefab;
    public bool isSpecialGem;
    [Range(0, 120f), SuffixLabel("s", true)] public float lifeTime = 60f;
}
