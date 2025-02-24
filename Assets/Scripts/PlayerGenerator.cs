using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class PlayerGenerator : MonoBehaviour
{
    public static PlayerGenerator Instance;

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
    public List<PlayerGenerator> blockingGrid = new List<PlayerGenerator>();
    private List<PlayerGenerator> unBlockingGrid = new List<PlayerGenerator>();
    public NavMeshObstacle obstacle;
    public float moveDuration = 1.0f;

    public string runTriggerName = "Run";
    public string jumpTriggerName = "Jump";

    private int runID;
    private int jumpID;
    public bool _moved;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        AddElementsToGrid();
        foreach (PlayerGenerator generator in blockingGrid)
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
    public bool Moved
    {
        get { return _moved; }
        set { _moved = value; }
    }

    public void MovePlayer()
    {
        // Movement logic
        Moved = true;
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

    //public void movePlayerToHole(GameObject hole)
    //{
    //    obstacle.enabled = false;
    //    Transform holePosition = hole.transform;
    //    foreach (GridElement gridElement in playerGridElements)
    //    {
    //        gridElement.agent.enabled = true;
    //        gridElement.agent.SetDestination(holePosition.position);
    //        gridElement.agent.stoppingDistance = 2;

    //        if (gridElement.animator != null)
    //        {
    //            gridElement.animator.SetTrigger("Run");
    //        }

    //        // Start the jump coroutine
    //        gridElement.StartedRunning = true;
    //        gridElement.Hole = hole;
    //        // StartCoroutine(gridElement.JumpToHole());
    //    }

    //    foreach (PlayerGenerator gridGenerator in unBlockingGrid)
    //    {
    //        gridGenerator.blockingGrid.Remove(this);

    //        if (gridGenerator.blockingGrid.Count <= 0)
    //        {
    //            gridGenerator.isMovable = true;
    //        }
    //    }

    //}
    //public void movePlayerToHole(GameObject hole)
    //{
    //    obstacle.enabled = false;
    //    Transform holePosition = hole.transform;

    //    foreach (GridElement gridElement in playerGridElements)
    //    {
    //        gridElement.agent.enabled = true;
    //        gridElement.agent.SetDestination(holePosition.position);
    //        gridElement.agent.stoppingDistance = .5f;

    //        if (gridElement.animator != null)
    //        {
    //            gridElement.animator.SetTrigger("Run");
    //        }

    //        Start the coroutine to handle the jump animation
    //        gridElement.StartCoroutine(MoveAllPlayersToHole(hole));
    //    }

    //    foreach (PlayerGenerator gridGenerator in unBlockingGrid)
    //    {
    //        gridGenerator.blockingGrid.Remove(this);

    //        if (gridGenerator.blockingGrid.Count <= 0)
    //        {
    //            gridGenerator.isMovable = true;
    //        }
    //    }
    //}
    public void movePlayerToHole(GameObject hole)
    {
        obstacle.enabled = false;
        Transform holePosition = hole.transform;

        foreach (GridElement gridElement in playerGridElements)
        {
            // Enable the NavMeshAgent and set the destination
            gridElement.agent.enabled = true;
            gridElement.agent.SetDestination(holePosition.position);
            gridElement.agent.stoppingDistance = 0.5f;

            // Trigger the run animation if applicable
            if (gridElement.animator != null)
            {
                gridElement.animator.SetTrigger("Run");
            }

            // Start movement to the hole
            StartCoroutine(MoveAllPlayersToHole(hole));
        }

        // Handle unblocking grids
        foreach (PlayerGenerator gridGenerator in unBlockingGrid)
        {
            gridGenerator.blockingGrid.Remove(this);

            if (gridGenerator.blockingGrid.Count <= 0)
            {
                gridGenerator.isMovable = true;
            }
        }
    }
    private IEnumerator MoveAllPlayersToHole(GameObject hole)
    {
        Vector3 holePosition = hole.transform.position;
        float baseRunDuration = moveDuration * 1.5f;
        float baseJumpDuration = moveDuration * 0.5f;
        List<Tween> runTweens = new List<Tween>();

        foreach (GridElement gridElement in playerGridElements)
        {
            if (IsObstacleInPath(gridElement.transform.position, holePosition))
            {
                Debug.Log("Path blocked by an obstacle! Player can't move.");
                if (AudioManager.instance != null)
                {
                    // AudioManager.instance.Play("WrongMove");
                }
                yield break;
                //vibration.vibrate(20);
            }

            ParticleSystem ps = gridElement.GetComponentInChildren<ParticleSystem>(true);
            if (ps != null)
            {
                ps.gameObject.SetActive(true);
                ps.Play();
            }

            if (gridElement.animator != null)
            {
                float animSpeedMultiplier = UnityEngine.Random.Range(0.9f, 1.1f);
                gridElement.animator.SetTrigger(runID);
                gridElement.animator.speed = animSpeedMultiplier;
            }

            gridElement.StartedRunning = true;
            gridElement.Hole = hole;
            float runDuration = baseRunDuration * UnityEngine.Random.Range(0.5f, 1f);
            Vector3 startPos = gridElement.transform.position;

            float xPos = Random.Range((holePosition.x - 1f), (holePosition.x + 1f));
            float zPos = Random.Range((holePosition.z - 1f), (holePosition.z + 1f));
            Vector3 endPos = new Vector3(xPos, startPos.y, zPos);
            Vector3 controlPoint = Vector3.Lerp(startPos, endPos, 0.5f);
            controlPoint += new Vector3(
                UnityEngine.Random.Range(-1.5f, 1.5f),
                0,
                UnityEngine.Random.Range(-1.5f, 1.5f)
            );

            Tween runTween = DOTween.To(
                () => 0f,
                (float progress) =>
                {
                    Vector3 m1 = Vector3.Lerp(startPos, controlPoint, progress);
                    Vector3 m2 = Vector3.Lerp(controlPoint, endPos, progress);
                    gridElement.transform.position = Vector3.Lerp(m1, m2, progress);
                },
                1f,
                runDuration
            ).SetEase(Ease.OutQuad);
            runTween.Play().OnUpdate(() =>
            {
                float distance = Vector3.Distance(gridElement.transform.position, endPos);
                if (distance <= Random.Range(0.5f, 1f))
                {
                    if (gridElement.animator != null)
                    {
                        gridElement.animator.SetTrigger(jumpID);
                    }
                    if (AudioManager.instance != null)
                    {
                        // AudioManager.instance.Play("Jump");
                    }

                    float jumpDuration = baseJumpDuration * UnityEngine.Random.Range(0.1f, .2f);
                    float jumpHeight = UnityEngine.Random.Range(0.1f, 0.2f);

                    Vector3 startJumpPos = gridElement.transform.position;
                    Vector3 endJumpPos = new Vector3(
                        holePosition.x + UnityEngine.Random.Range(-0.3f, 0.3f),
                        holePosition.y - 10,
                        holePosition.z + UnityEngine.Random.Range(-0.3f, 0.3f)
                    );

                    Sequence jumpSequence = DOTween.Sequence();

                    jumpSequence.Append(gridElement.transform.DOMoveY(
                        startJumpPos.y + jumpHeight,
                        jumpDuration * 0.4f
                    ).SetEase(Ease.OutQuad));

                    jumpSequence.Append(gridElement.transform.DOMoveY(
                        endJumpPos.y,
                        jumpDuration * 0.6f
                    ).SetEase(Ease.InQuad));

                    jumpSequence.Join(gridElement.transform.DOMove(
                        endJumpPos,
                        jumpDuration
                    ).SetEase(Ease.InOutQuad));

                    jumpSequence.OnStart(() =>
                    {
                        // Disable gravity during the jump
                        if (gridElement.rb != null)
                        {
                            gridElement.rb.useGravity = false;
                        }
                    });

                    jumpSequence.OnComplete(() =>
                    {
                        // Enable gravity after the jump
                        if (gridElement.rb != null)
                        {
                            gridElement.rb.useGravity = true;
                        }

                        if (ps != null)
                        {
                            ps.Stop();
                            ps.gameObject.SetActive(false);
                        }
                    });
                }
            }).OnComplete(() =>
            {
                // Final actions for the tween
            });
        }
    }



    //private IEnumerator MoveAllPlayersToHole(GameObject hole)
    //{
    //    Vector3 holePosition = hole.transform.position;
    //    float baseRunDuration = moveDuration * 1.5f;
    //    float baseJumpDuration = moveDuration * 0.5f;
    //    List<Tween> runTweens = new List<Tween>();

    //    foreach (GridElement gridElement in playerGridElements)
    //    {
    //        if (IsObstacleInPath(gridElement.transform.position, holePosition))
    //        {
    //            Debug.Log("Path blocked by an obstacle! Player can't move.");
    //            if (AudioManager.instance != null)
    //            {
    //                // AudioManager.instance.Play("WrongMove");
    //            }
    //            yield break;
    //            //vibration.vibrate(20);
    //        }

    //        ParticleSystem ps = gridElement.GetComponentInChildren<ParticleSystem>(true);
    //        if (ps != null)
    //        {
    //            ps.gameObject.SetActive(true);
    //            ps.Play();
    //        }

    //        if (gridElement.animator != null)
    //        {
    //            float animSpeedMultiplier = UnityEngine.Random.Range(0.9f, 1.1f);
    //            gridElement.animator.SetTrigger(runID);
    //            gridElement.animator.speed = animSpeedMultiplier;
    //        }

    //        gridElement.StartedRunning = true;
    //        gridElement.Hole = hole;
    //        float runDuration = baseRunDuration * UnityEngine.Random.Range(0.5f, 1f);
    //        Vector3 startPos = gridElement.transform.position;

    //        float xPos = Random.Range((holePosition.x - 1f), (holePosition.x + 1f));
    //        float zPos = Random.Range((holePosition.z - 1f), (holePosition.z + 1f));
    //        Vector3 endPos = new Vector3(xPos, startPos.y, zPos);
    //        Vector3 controlPoint = Vector3.Lerp(startPos, endPos, 0.5f);
    //        controlPoint += new Vector3(
    //            UnityEngine.Random.Range(-1.5f, 1.5f),
    //            0,
    //            UnityEngine.Random.Range(-1.5f, 1.5f)
    //        );

    //        Tween runTween = DOTween.To(
    //            () => 0f,
    //            (float progress) =>
    //            {
    //                Vector3 m1 = Vector3.Lerp(startPos, controlPoint, progress);
    //                Vector3 m2 = Vector3.Lerp(controlPoint, endPos, progress);
    //                gridElement.transform.position = Vector3.Lerp(m1, m2, progress);
    //            },
    //            1f,
    //            runDuration
    //        ).SetEase(Ease.OutQuad);
    //        runTween.Play().OnUpdate(() =>
    //        {
    //            float distance = Vector3.Distance(gridElement.transform.position, endPos);
    //            if (distance <= Random.Range(0.5f, 1f))
    //            {
    //                if (gridElement.animator != null)
    //                {
    //                    gridElement.animator.SetTrigger(jumpID);
    //                }
    //                if (AudioManager.instance != null)
    //                {
    //                    // AudioManager.instance.Play("Jump");
    //                }

    //                float jumpDuration = baseJumpDuration * UnityEngine.Random.Range(0.1f, .2f);
    //                float jumpHeight = UnityEngine.Random.Range(0.1f, 0.2f);

    //                Vector3 startJumpPos = gridElement.transform.position;
    //                Vector3 endJumpPos = new Vector3(
    //                    holePosition.x + UnityEngine.Random.Range(-0.3f, 0.3f),
    //                    holePosition.y - 10,
    //                    holePosition.z + UnityEngine.Random.Range(-0.3f, 0.3f)
    //                );

    //                Sequence jumpSequence = DOTween.Sequence();

    //                jumpSequence.Append(gridElement.transform.DOMoveY(
    //                    startJumpPos.y + jumpHeight,
    //                    jumpDuration * 0.4f
    //                ).SetEase(Ease.OutQuad));

    //                jumpSequence.Append(gridElement.transform.DOMoveY(
    //                    endJumpPos.y,
    //                    jumpDuration * 0.6f
    //                ).SetEase(Ease.InQuad));

    //                jumpSequence.Join(gridElement.transform.DOMove(
    //                    endJumpPos,
    //                    jumpDuration
    //                ).SetEase(Ease.InOutQuad));

    //                jumpSequence.OnComplete(() =>
    //                {
    //                    ParticleSystem ps = gridElement.GetComponentInChildren<ParticleSystem>(true);
    //                    if (ps != null)
    //                    {
    //                        ps.Stop();
    //                        ps.gameObject.SetActive(false);
    //                    }
    //                });
    //            }
    //        }).OnComplete(() =>
    //        {

    //        });
    //    }
    //}

    private bool IsObstacleInPath(Vector3 startPosition, Vector3 targetPosition)
    {
        RaycastHit hit;
        Vector3 direction = targetPosition - startPosition;
        float distance = direction.magnitude;

      
       /* if (Physics.Raycast(startPosition, direction.normalized, out hit, distance))
        {

            if (hit.collider.CompareTag("Obstacle")) 
            {
                return true;
            }
        }*/

        return false; 
    }
}



