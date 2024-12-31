using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<PlayerGridGenerator> playerGrids = new List<PlayerGridGenerator>();

    private void Awake()
    { 
        if (Instance == null)
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }
    public void Start()
    {
        Vibration.Init();
    }
    //public void IsPlayerMovable(ColorEnum color,GameObject hole)
    //{
    //    Debug.Log("kflkasih");
    //    foreach (PlayerGridGenerator generator in playerGrids)
    //    {
    //        if(generator.gridColor == color)
    //        {
    //            if(generator.isMovable)
    //            {
    //                generator.movePlayerToHole(hole);
    //                CheckLevelComplete();
    //            }
    //        }
    //    }

    //}
    public void IsPlayerMovable(ColorEnum color, GameObject hole)
    {
        Debug.Log("kflkasih");
        if (playerGrids == null || playerGrids.Count == 0)
        {
            Debug.LogError("playerGrids is not initialized or empty.");
            return;
        }

        foreach (PlayerGridGenerator generator in playerGrids)
        { 
            if (generator == null)
            {
                Debug.LogWarning("A PlayerGridGenerator is null in the playerGrids list.");
                continue; 
            }

            if (generator.gridColor == color)
            {
                if (generator.isMovable)
                {
                    generator.movePlayerToHole(hole);
                    CheckLevelComplete();
                }
            }
        }
    }
    public void CheckLevelComplete()
    {
        bool completed = true;

        foreach (PlayerGridGenerator _grid in playerGrids)
        {
            if(!_grid.moved)
            {
                completed = false;
                break;
            }
        }

        if(completed)
        {
            if(UiManager.instance != null)
            {
                UiManager.instance.LevelComplete();
            }
        }
    }
}
