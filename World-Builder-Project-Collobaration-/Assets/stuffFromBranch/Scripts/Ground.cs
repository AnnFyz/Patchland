using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public static bool grass;
    public static bool sand;
    public static bool stone;

    private Renderer ground;

    Color grassColor = new Color(0.42f, 0.79f, 0.54f, 1.0f);
    Color sandColor = new Color(0.93f, 0.99f, 0.51f, 1.0f);
    Color stoneColor = new Color(0.65f, 0.737f, 0.73f, 1.0f);

    

    void Start()
    {
        grass = true;
        sand = false;
        stone = false;

        var ground = GetComponent<Renderer>();

        ground.material.SetColor("_Color", grassColor);
    }


    private void Awake()
    {
        
    }

    void Update()
    {
        var ground = GetComponent<Renderer>();

        if (Healthbar.naturePoints >= 50 && grass == false)
        {
            if (Input.GetKeyDown("q"))
            {
                ground.material.SetColor("_Color", grassColor);
                grass = true;
                sand = false;
                stone = false;

                Healthbar.naturePoints -= 50;
            }
        }

        if (Healthbar.naturePoints >= 50 && sand == false)
        {
            if (Input.GetKeyDown("w"))
            {
                ground.material.SetColor("_Color", sandColor);
                grass = false;
                sand = true;
                stone = false;

                Healthbar.naturePoints -= 50;
            }
        }

        if (Healthbar.naturePoints >= 50 && stone == false)
        {
            if (Input.GetKeyDown("e"))
            {
                ground.material.SetColor("_Color", stoneColor);
                grass = false;
                sand = false;
                stone = true;

                Healthbar.naturePoints -= 50;
            }
        }
    }
}
