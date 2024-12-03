using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<PlayerGridGenerator> playerGrids = new List<PlayerGridGenerator>();


    public void Awake()
    {
        Instance = this;
    }
    public void IsPlayerMovable(ColorEnum color,GameObject hole)
    {
       
        foreach (PlayerGridGenerator generator in playerGrids)
        {
            if(generator.gridColor == color)
            {
               if(generator.isMovable)
                {
                    generator.movePlayerToHole(hole);
                }
            }
        }

    }
}
