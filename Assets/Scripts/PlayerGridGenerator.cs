using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerGridGenerator : MonoBehaviour
{
    public static PlayerGridGenerator Instance;

    [Header("Moves Configuration")]
    public int maxMoves = 5;
    private int remainingMoves;
    private bool isGameplayActive = true;
    private bool isGameSuccess = false;

    [Header("UI Elements")]
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI LevelTxt;
    public GameObject retryUI;
    public GameObject successUI;

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
    public bool Generated;

    [Header("Player Movement")]
    public bool isMovable = false;
    public List<PlayerGridGenerator> blockingGrid = new List<PlayerGridGenerator>();
    private List<PlayerGridGenerator> unBlockingGrid = new List<PlayerGridGenerator>();

    private int totalGrids;
    private int sortedGrids;
    public ParticleSystem particleSystem;
    public float moveDuration = 1f;
    public float jumpDuration = 0.5f;
    public float jumpHeight = 2f;
    [Header("Obstacles")]
    public GameObject[] obstacles;

    public bool moved = false;



    private int jumpID = Animator.StringToHash("Jump");
    private int runID = Animator.StringToHash("Run");

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
        sortedGrids = 0;
        totalGrids = rows * columns;

        UpdateMovesUI();
        if (retryUI != null)
        {
            retryUI.SetActive(false);
        }
        if (successUI != null)
        {
            successUI.SetActive(false);
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
    }

    [ContextMenu("Generate Grid")]
    public void GeneratePlayerGrid()
    {
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
                sortedGrids++;
            }

            PlayerGrid[Column, Row] = Tile.gameObject;
        }
    }

    public void IncrementSortedGrids()
    {
        sortedGrids++;
        if (sortedGrids == totalGrids)
        {
            DisplaySuccessPanel();
        }
    }

    private void CheckAllChildrenDestroyed()
    {
        bool allDestroyed = true;

        foreach (GridElement gridElement in playerGridElements)
        {
            if (gridElement != null)
            {
                allDestroyed = false;
                Debug.Log($"{gridElement.name} is still active.");
                break;
            }
        }

        if (allDestroyed)
        {
            DisplaySuccessPanel();
        }
    }

    public void movePlayerToHole(GameObject targetHole)
    {
        if (!isGameplayActive)
        {
            Debug.Log("Gameplay stopped. Cannot move player.");
            PlayWrongMoveSound();
            return;
        }
        if (remainingMoves <= 0)
        {
            Debug.Log("No moves left!");
            StopGameplay();
            return;
        }
        if (targetHole == null)
        {
            Debug.LogError("Target hole is null or destroyed. Cancelling movement.");
            return;
        }

        remainingMoves--;
        UpdateMovesUI();

        if (remainingMoves <= 0)
        {
            StopGameplay();
            return;
        }

        GameObject selectedHole = targetHole;
        if (IsObstacleInPath(transform.position, targetHole.transform.position))
        {
            Debug.Log("Path to target hole is blocked. Finding alternate hole...");
            selectedHole = FindAlternateHole(targetHole);

            if (selectedHole == null)
            {
                Debug.LogError("No alternate hole found. Cannot move player.");
                return;
            }
            StartCoroutine(MoveToIntermediateHoleAndContinue(selectedHole, targetHole));
        }
        else
        {
            StartCoroutine(SafeMoveAllPlayersToHole(targetHole));
        }
    }

    private IEnumerator MoveToIntermediateHoleAndContinue(GameObject intermediateHole, GameObject targetHole)
    {
        if (intermediateHole == null || targetHole == null)
        {
            Debug.LogError("Intermediate or Target hole is null. Cancelling movement.");
            yield break;
        }
        Debug.Log($"Moving to intermediate hole: {intermediateHole.name}");
        yield return StartCoroutine(SafeMoveAllPlayersToHole(intermediateHole));
        Debug.Log($"Moving to target hole: {targetHole.name}");
        yield return StartCoroutine(SafeMoveAllPlayersToHole(targetHole));
    }

    private GameObject FindAlternateHole(GameObject targetHole)
    {
        List<GameObject> allHoles = GetAllHoles();
        GameObject closestHole = null;
        float shortestDistance = float.MaxValue;

        foreach (GameObject hole in allHoles)
        {
            if (hole == targetHole) continue;

            if (!IsObstacleInPath(transform.position, hole.transform.position))
            {
                float distance = Vector3.Distance(transform.position, hole.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestHole = hole;
                }
            }
        }

        return closestHole;
    }

    private List<GameObject> GetAllHoles()
    {
        // Return a list of all holes in the scene. 
        // This is a placeholder. Replace this with your actual logic to fetch holes.
        return new List<GameObject>(GameObject.FindGameObjectsWithTag("Hole"));
    }

    //public void movePlayerToHole(GameObject hole)
    //{
    //    if (!isGameplayActive)
    //    {
    //        Debug.Log("Gameplay stopped. Cannot move player.");
    //        PlayWrongMoveSound();
    //        return;
    //    }
    //    if (remainingMoves <= 0)
    //    {
    //        Debug.Log("No moves left!");
    //        StopGameplay();
    //        return;
    //    }
    //    if (hole == null)
    //    {
    //        Debug.LogError("Hole object is null or destroyed. Cancelling movement.");
    //        return;
    //    }
    //    remainingMoves--;
    //    UpdateMovesUI();
    //    if (remainingMoves <= 0)
    //    {
    //        StopGameplay();
    //        return;
    //    }
    //    StartCoroutine(SafeMoveAllPlayersToHole(hole));
    //}
    private void PlayWrongMoveSound()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.Play("WrongMove");
        }
    }


    private bool IsObstacleInPath(GameObject hole)
    {

        Vector3 directionToHole = (hole.transform.position - transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToHole, out hit, Vector3.Distance(transform.position, hole.transform.position)))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                Debug.Log("Obstacle detected: " + hit.collider.name);
                return true;
            }
        }

        return false;
    }


    private IEnumerator SafeMoveAllPlayersToHole(GameObject hole)
    {
        if (hole == null || this == null || gameObject == null)
        {
            Debug.LogError("Cannot start coroutine: Hole or PlayerGridGenerator is destroyed.");
            yield break;
        }

        yield return StartCoroutine(MoveAllPlayersToHole(hole));
    }
    private bool IsObstacleInPath(Vector3 startPosition, Vector3 endPosition)
    {
        Debug.Log("Obstacle");
        LayerMask obstacleLayer = LayerMask.GetMask("ObstacleLayer");
        RaycastHit hit;
        if (Physics.Linecast(startPosition, endPosition, out hit, obstacleLayer))
        {
            return true;
        }
        return false;
    }
    private bool IsPlayerInFrontOfHole(GridElement gridElement, Vector3 directionToHole, Vector3 holePosition)
    {

        Vector3 playerDirection = (gridElement.transform.position - holePosition).normalized;
        float angleTolerance = 30f;
        float angle = Vector3.Angle(directionToHole, playerDirection);

        return angle < angleTolerance;
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

                    jumpSequence.OnComplete(() =>
                    {
                        ParticleSystem ps = gridElement.GetComponentInChildren<ParticleSystem>(true);
                        if (ps != null)
                        {
                            ps.Stop();
                            ps.gameObject.SetActive(false);
                        }
                    });
                }
            }).OnComplete(() =>
            {

            });
            // runTweens.Add(runTween);
        }

        /*  Sequence runSequence = DOTween.Sequence();
          foreach (Tween runTween in runTweens)
          {
              runSequence.Join(runTween);
          }
          runSequence.Play();
          runSequence.OnComplete(() =>
          {

          });
          yield return runSequence.WaitForCompletion();

          foreach (GridElement gridElement in playerGridElements)
          {
              if (gridElement.animator != null)
              {
                  gridElement.animator.speed = 1f;
              }
          }

          List<Tween> jumpTweens = new List<Tween>();
          foreach (GridElement gridElement in playerGridElements)
          {


              //jumpTweens.Add(jumpSequence);
          }

          Sequence finalJumpSequence = DOTween.Sequence();
          foreach (Tween jumpTween in jumpTweens)
          {
              finalJumpSequence.Join(jumpTween);
          }
          finalJumpSequence.Play();
          yield return finalJumpSequence.WaitForCompletion();*/
        CheckAllChildrenDestroyed();
        Debug.Log("All players have jumped into the hole!");
        if (AreAllPlayersInHole())
        {
            DisplaySuccessPanel();
        }
    }


    public IEnumerator RemoveGridElementAfterDelay(GridElement gridElement, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (gridElement != null)
        {
            Destroy(gridElement.gameObject);
            playerGridElements.Remove(gridElement);
            Debug.Log($"{gridElement.name} has been removed.");
        }

        CheckAllChildrenDestroyed();
    }



    private bool AreAllPlayersInHole()
    {
        foreach (GridElement gridElement in playerGridElements)
        {
            if (!gridElement.HasReachedHole())
            {
                Debug.Log($"{gridElement.name} has not reached the hole!");
                return false;
            }
        }
        Debug.Log("All players have reached the hole!");
        return true;
    }


    private void DisplaySuccessPanel()
    {
        Debug.Log("DisplaySuccessPanel called!");
        isGameplayActive = false;
        isGameSuccess = true;

        Time.timeScale = 0;
        if (successUI != null)
        {
            successUI.SetActive(true);
            Debug.Log("Success UI activated!");
        }
        else
        {
            Debug.LogError("Success UI is not assigned in the Inspector!");
        }
    }

    private void StopGameplay()
    {
        isGameplayActive = false;
        Debug.Log("Gameplay stopped. No moves left!");
        if (retryUI != null)
        {
            retryUI.SetActive(true);
        }

        Time.timeScale = 0;
    }


    private void UpdateMovesUI()
    {
        if (movesText != null)
        {
            movesText.text = $"Moves: {remainingMoves}";
        }
        else
        {
            Debug.LogError("Moves Text is not assigned in the Inspector.");
        }
    }

    public void RetryGame()
    {
        Time.timeScale = 1;
        remainingMoves = maxMoves;
        sortedGrids = 0;
        isGameplayActive = true;
        isGameSuccess = false;

        UpdateMovesUI();

        if (retryUI != null)
        {
            retryUI.SetActive(false);
        }
        if (successUI != null)
        {
            successUI.SetActive(false);
        }

        Debug.Log("Game restarted.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void OnMouseDown()
    {
        if (!isGameplayActive || remainingMoves <= 0)
        {
            Debug.Log("Mouse interaction blocked. No moves left!");
            if (AudioManager.instance != null)
            {
                //  AudioManager.instance.Play("Wrong");
            }

            return;
        }
        Debug.Log("Mouse down detected. Processing interaction...");
    }

}
