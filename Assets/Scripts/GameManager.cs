using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<PlayerGenerator> playerGrids = new List<PlayerGenerator>();
    public List<GridElement> playerGridElements;

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
        private GameObject destinationHole;

        private void Start()
        {
            // Find the destination hole object by tag (adjust the tag accordingly)
            destinationHole = GameObject.FindWithTag("Hole");

            if (destinationHole == null)
            {
                Debug.LogError("Destination hole not found in the scene!");
            }
        }

        private IEnumerator MovePlayerToDestination()
        {
            if (destinationHole == null)
            {
                Debug.LogError("Destination hole is not assigned or not found!");
                yield break;
            }

            // Get the position of the hole (destination object)
            Vector3 holePosition = destinationHole.transform.position;

            // Proceed with player movement towards the hole as described earlier
            foreach (GridElement gridElement in playerGridElements)
            {
                if (gridElement == null)
                {
                    Debug.LogWarning("GridElement is null, skipping...");
                    continue;
                }

                // Particle effects, obstacle deactivation, animations, and other logic remain the same...
            }
        }


    public void IsPlayerMovable(ColorEnum color, GameObject hole)
    {
        Debug.Log("kflkasih");
        foreach (PlayerGenerator generator in playerGrids)
        {
            if (generator.gridColor == color)
            {
                if (generator.isMovable)
                {
                    generator.movePlayerToHole(hole);
                    //CheckLevelComplete();
                }
            }
        }

    }
    //public void CheckLevelComplete()
    //{
    //    bool completed = true;

    //    foreach (PlayerGenerator _grid in playerGrids)
    //    {
    //        if(!_grid._moved)
    //        {
    //            completed = false;
    //            break;
    //        }
    //    }

        //if(completed)
        //{
        //    if(UiManager.instance != null)
        //    {
        //        UiManager.instance.LevelComplete();
        //    }
        //}
    
}
