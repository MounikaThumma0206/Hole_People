using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class PlayerGridGenerator : MonoBehaviour
{
    public static PlayerGridGenerator Instance;

    [Header("Moves Configuration")]
    public int maxMoves = 5; 
    private int remainingMoves; 
    private bool isGameplayActive = true; 

    [Header("UI Elements")]
    public TextMeshProUGUI movesText;
    private GameObject retryUI;


    public GameObject[,] PlayerGrid;
    public List<GridElement> playerGridElements = new List<GridElement>();

    public List<Transform> PlayerPositions = new List<Transform>();
    [Header("Generate Grid")]
    public int rows;
    public int columns;
    public float Column_spacing;
    public float Row_spacing;
    public GameObject Tile;
    public int TileCount;
    public ColorManager colorManager;
    public ColorEnum gridColor;
    private Material gridMaterial;
    // public GameObject TilePrefab;
    // public GameObject BlockedTilePrefab;
    // public GameObject GaragePrefab;
    public bool Generated;
    [Header("Player Movement")]
    public bool isMovable = false;
    public List<PlayerGridGenerator> blockingGrid = new List<PlayerGridGenerator>();
    private List<PlayerGridGenerator> unBlockingGrid = new List<PlayerGridGenerator>();
    public NavMeshObstacle obstacle;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        remainingMoves = maxMoves;
    }

    void Start()
    {
        
        UpdateMovesUI();
        if (retryUI != null)
        {
            retryUI.SetActive(false); 
        }

        AddElementsToGrid();
        foreach (PlayerGridGenerator generator in blockingGrid)
        {
            generator.unBlockingGrid.Add(this);
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerGrids.Add(this);
        }
        if (obstacle != null)
        {
            obstacle.enabled = true;
        }
        else
        {
            obstacle = transform.GetComponent<NavMeshObstacle>();
            if (obstacle != null)
            {
                obstacle.enabled = true;
            }
            else
            {
                Debug.LogError("NavMeshObstacle component missing on " + gameObject.name);
            }
        }

    }

    [ContextMenu("Generate Grid")]

    public void GeneratePlayerGrid()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        string gridName = transform.gameObject.name;
        transform.gameObject.name = gridColor + " " + gridName;
        // Set the gridMaterial based on the selected color
        foreach (ColorMaterial colorMaterial in colorManager.colorMaterials)
        {
            if (colorMaterial.colorEnum == gridColor)
            {
                gridMaterial = colorMaterial.material;
                break; // Exit the loop once we find the matching color
            }
        }

        if (gridMaterial == null)
        {
            Debug.LogError("Grid material is not set! Make sure a valid color is selected.");
            return; // Exit if no material was found
        }

        // Clear any previously generated grid positions
        if (PlayerPositions.Count > 0)
        {
            foreach (Transform Tile in PlayerPositions)
            {
                DestroyImmediate(Tile.gameObject);
            }
            PlayerPositions.Clear();
        }

        // Generate the grid
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
#if UNITY_EDITOR
                Vector3 position = new Vector3((j * Column_spacing) + x, y, (i * -Row_spacing) + z);
                GameObject cube = (GameObject)PrefabUtility.InstantiatePrefab(Tile, transform);
                cube.transform.position = position;

                // Ensure the cube has a GridElement component
                GridElement gridElement = cube.GetComponent<GridElement>();
                if (gridElement != null)
                {
                    // Check if playerRenderer exists in the GridElement and set the material
                    if (gridElement.playerRenderer != null)
                    {
                        gridElement.playerRenderer.material = gridMaterial;
                    }
                    else
                    {
                        Debug.LogWarning("playerRenderer is not set on GridElement for " + cube.name);
                    }

                    gridElement.Row = i;
                    gridElement.Column = j;
                    cube.transform.gameObject.name = "Grid (" + j + "," + i + ")";
                    PlayerPositions.Add(cube.transform);
                }
                else
                {
                    Debug.LogError("GridElement component not found on " + cube.name);
                }
#endif
            }
        }

        // Set the parent for each tile after the grid is generated
        foreach (Transform Tile in PlayerPositions)
        {
            Tile.parent = this.transform;
        }

        TileCount = PlayerPositions.Count;
        Generated = true;
        AddElementsToGrid();
    }

    //    public void GeneratePlayerGrid()
    //    {
    //        float x = transform.position.x;
    //        float y = transform.position.y;
    //        float z = transform.position.z;

    //        foreach(ColorMaterial colorMaterial in colorManager.colorMaterials)
    //        {
    //            if (colorMaterial.colorEnum == gridColor)
    //            {
    //                gridMaterial = colorMaterial.material;
    //            }
    //        }
    //        if (PlayerPositions.Count > 0)
    //        {
    //            foreach (Transform Tile in PlayerPositions)
    //            {
    //                DestroyImmediate(Tile.gameObject);
    //            }
    //            PlayerPositions.Clear();
    //        }

    //        for (int i = 0; i < rows; i++)
    //        {
    //            for (int j = 0; j < columns; j++)
    //            {
    //#if UNITY_EDITOR
    //                Vector3 position = new Vector3((j * Column_spacing) + x, y, (i * -Row_spacing) + z);
    //                GameObject cube = (GameObject)PrefabUtility.InstantiatePrefab(Tile, transform);
    //                cube.transform.position = position;
    //                GridElement gridElement = cube.GetComponent<GridElement>();
    //                gridElement.playerRenderer.material= gridMaterial;   
    //                gridElement.Row = i;
    //                gridElement.Column = j;

    //                cube.transform.gameObject.name = "Grid (" + j + "," + i + ")";
    //                PlayerPositions.Add(cube.transform);
    //#endif
    //            }
    //        }

    //        foreach (Transform Tile in PlayerPositions)
    //        {
    //            Tile.parent = this.transform;
    //        }
    //        TileCount = PlayerPositions.Count;
    //        Generated = true;
    //        AddElementsToGrid();
    //    }

    //    private void OnDrawGizmos()
    //    {
    //#if UNITY_EDITOR
    //        if (PlayerPositions != null && PlayerPositions.Count > 0)
    //        {
    //            Handles.color = Color.red;

    //            foreach (Transform Tile in PlayerPositions)
    //            {
    //                GridElement grid = Tile.GetComponent<GridElement>();
    //                if (grid != null)
    //                {
    //                    Vector3 position = Tile.position;
    //                    string label = $"({grid.Column}, {grid.Row})";
    //                    Handles.Label(position + new Vector3(-0.2f, 0, 0.3f), label);
    //                }
    //            }

    //            // Reset color if needed
    //            Handles.color = Color.white;
    //        }
    //#endif
    //    }

    //    [ContextMenu("Change TilePrefab")]
    //    public void ChangeTilePrefab()
    //    {
    //        foreach (GameObject playerGrid in PlayerGrid)
    //        {
    //            if (playerGrid == null) continue; // Skip null entries

    //#if UNITY_EDITOR
    //            // Check if this GameObject is a prefab instance
    //            if (PrefabUtility.IsPartOfPrefabInstance(playerGrid))
    //            {
    //                // Find the "Player" child within the prefab
    //                Transform playerChild = playerGrid.transform.Find("Player");
    //                if (playerChild != null)
    //                {
    //                    // Revert all overrides on the "Player" GameObject
    //                    PrefabUtility.RevertObjectOverride(playerChild.gameObject, InteractionMode.UserAction);

    //                    Debug.Log($"Reverted changes for 'Player' in {playerGrid.name}");
    //                }
    //                else
    //                {
    //                    Debug.LogWarning($"'Player' child not found in {playerGrid.name}");
    //                }
    //            }
    //            else
    //            {
    //                Debug.LogWarning($"GameObject {playerGrid.name} is not a prefab instance.");
    //            }
    //#endif
    //        }
    //    }

    public void AddElementsToGrid()
    {
        PlayerGrid = new GameObject[columns, rows];

        foreach (Transform Tile in PlayerPositions)
        {
            GridElement grid = Tile.gameObject.GetComponent<GridElement>();
            playerGridElements.Add(grid);
            int Row = grid.Row;
            int Column = grid.Column;
            if (grid.IsOccupied)
            {
                grid.Isempty = false;
                // grid.GeneratePlayer();
            }
            PlayerGrid[Column, Row] = Tile.gameObject;
        }
    }

    public GridElement GetGridElement(int column, int row)
    {
        GameObject grid = PlayerGrid[column, row];
        GridElement gridElement = grid.GetComponent<GridElement>();
        if (gridElement == null)
        {
            Debug.Log("Send_" + column);
        }
        return gridElement;
    }

    //public GameObject GetPlayer(int column, int row)
    //{
    //    GameObject grid = PlayerGrid[column, row];
    //    GridElement gridElement = grid.GetComponent<GridElement>();
    //    return gridElement.Player;
    //}

    //////public void movePlayerToHole(GameObject hole)
    //////{
    //////    obstacle.enabled = false;
    //////    Transform holePosition = hole.transform;
    //////    foreach (GridElement gridElement in playerGridElements)
    //////    {
    //////        gridElement.agent.enabled = true;
    //////        gridElement.agent.SetDestination(holePosition.position);
    //////        gridElement.agent.stoppingDistance = 2;

    //////        if (gridElement.animator != null)
    //////        {
    //////            gridElement.animator.SetTrigger("Run");
    //////        }

    //////        // Start the jump coroutine
    //////        gridElement.StartedRunning = true;
    //////        gridElement.Hole= hole;
    //////       // StartCoroutine(gridElement.JumpToHole());
    //////    }

    //////    foreach (PlayerGridGenerator gridGenerator in unBlockingGrid)
    //////    {
    //////        gridGenerator.blockingGrid.Remove(this);

    //////        if (gridGenerator.blockingGrid.Count <= 0)
    //////        {
    //////            gridGenerator.isMovable = true;
    //////        }
    //////    }

    //////}
    ///




    public void movePlayerToHole(GameObject hole)
    {
        if (!isGameplayActive)
        {
            Debug.Log("Gameplay stopped. No moves left!");
            return;
        }

        if (remainingMoves <= 0)
        {
            Debug.Log("No moves left.");
            StopGameplay();
            return;
        }

        obstacle.enabled = false;
        Transform holePosition = hole.transform;

        foreach (GridElement gridElement in playerGridElements)
        {
            gridElement.agent.enabled = true;
            gridElement.agent.SetDestination(holePosition.position);
            gridElement.agent.stoppingDistance = 2;

            if (gridElement.animator != null)
            {
                gridElement.animator.SetTrigger("Run");
            }

            gridElement.StartedRunning = true;
            gridElement.Hole = hole;
        }

        foreach (PlayerGridGenerator gridGenerator in unBlockingGrid)
        {
            gridGenerator.blockingGrid.Remove(this);

            if (gridGenerator.blockingGrid.Count <= 0)
            {
                gridGenerator.isMovable = true;
            }
        }
        remainingMoves--;
        UpdateMovesUI();
        if (remainingMoves <= 0)
        {
            StopGameplay();
        }
    }

    private void UpdateMovesUI()
    {
        if (movesText != null)
        {
            movesText.text = $" Move : {remainingMoves}";
        }
        else
        {
            Debug.LogWarning("Moves Text is not assigned in the Inspector.");
        }
    }
    private void StopGameplay()
    {
        isGameplayActive = false;
        Debug.Log("Gameplay stopped. No moves left!");

        // Stop all time-dependent operations in the scene
        Time.timeScale = 0;

        // Show the retry UI
        if (retryUI != null)
        {
            retryUI.SetActive(true);
        }
    }
     // Check if gameplay should stop
       
public void RetryGame()
{
    // Reset the game state and reload the scene
    Time.timeScale = 1;
    remainingMoves = maxMoves;
    UpdateMovesUI();

    if (retryUI != null)
    {
        retryUI.SetActive(false);
    }

    Debug.Log("Game restarted.");
    // Optionally, reload the scene if needed:
    // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}

private void OnMouseDown()
{
    // Block all mouse interactions if gameplay is stopped
    if (!isGameplayActive)
    {
        Debug.Log("Mouse interaction blocked. No moves left!");
        return;
    }

    Debug.Log("Mouse down detected. Processing interaction...");
    // Add logic for mouse interaction here
}
}
