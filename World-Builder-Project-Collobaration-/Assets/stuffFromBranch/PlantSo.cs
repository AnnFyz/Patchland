using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "newPlant", menuName = "plant")]

public class PlantSo : ScriptableObject
{
    public string plantname;
    public string description;
    public GameObject plantBody;

    public int Cost;

    public bool onSand;

    public bool onGrass;



    
}
