using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class CroudManager : GridItemGenerator
{

	public CroudManagerData croudManagerData;
	public GameObject[,] PlayerGrid;


	[HideInInspector] public List<GridElement> playerGridElements = new List<GridElement>();
	[HideInInspector] public List<Transform> PlayerPositions = new List<Transform>();



	[Header("Generate Grid")]
	[HideInInspector] public int rows;
	[HideInInspector] public int columns;
	[HideInInspector] public int TileCount;
	[Header("Player Movement")]
	public bool isMovable = false;
	private Material gridMaterial;


	[Header("Pillers")]
	public bool shouldGeneratePillers;
	[SerializeField] static Pillar pillerPrefab;
	[SerializeField] List<CroudManager> blockingGrid = new List<CroudManager>();
	private List<CroudManager> unBlockingGrid = new List<CroudManager>();
	NavMeshObstacle obstacle;

	[HideInInspector] public bool _moved;
	[Header("Events")]
	[SerializeField] UnityEvent OnCrowdCleared = new();


	public int runTriggerName => croudManagerData.RunId;
	public int jumpTriggerName => croudManagerData.JumpId;


	public ColorEnum GridColor => colorEnum;
	public GameObject Tile => croudManagerData.croudPrefab;
	public ColorManager colorManager => croudManagerData.colorManager;
	public float Column_spacing => croudManagerData.spacing.y;
	public float Row_spacing => croudManagerData.spacing.x;
	GameObject pillarParent;
	private PillarType pillarType = PillarType.DEACTIVE;
	private List<Pillar> pillars = new List<Pillar>();
	void Start()
	{
		foreach (CroudManager generator in blockingGrid)
		{
			generator.isMovable = false;
			generator.unBlockingGrid.Add(this);
		}
		if (GameManager.Instance != null)
		{
			//GameManager.Instance.playerGrids.Add(this);
		}
		if (obstacle != null)
		{
			obstacle.enabled = true;
		}
		else
		{
			obstacle = GetComponent<NavMeshObstacle>();
			if (obstacle != null)
			{
				obstacle.enabled = true;
			}
			else
			{
				Debug.LogError("NavMeshObstacle component missing on " + gameObject.name);
			}
		}
		foreach (Pillar pillar in pillars)
		{
			pillar.SwitchPillarType(GridColor);
		}

	}
	public override void OnEnable()
	{
		base.OnEnable();
		GameManager.Instance.SusbscribeGenerators(this);
		if (shouldGeneratePillers)
			Hole.OnAnyHoleClicked.AddListener(SwitchPillarType);
	}

	private void SwitchPillarType(ColorEnum colorType)
	{
		if (IsCleared && PillarType == PillarType.DEACTIVE) return;
		//if (gridColor == colorType && CanMove()) return;
		switch (PillarType)
		{
			case PillarType.ACTIVE:
				//do something
				PillarType = PillarType.DEACTIVE;
				break;
			case PillarType.DEACTIVE:
				//do something
				PillarType = PillarType.ACTIVE;
				break;
		}
		foreach (Pillar pillar in pillars)
		{
			pillar.SwitchPillarType(GridColor);
		}
	}

	public override void OnDisable()
	{
		base.OnDisable();
		GameManager.Instance.UnSubscribeGenerators(this);
		if (shouldGeneratePillers)
			Hole.OnAnyHoleClicked.RemoveListener(SwitchPillarType);
	}

	public bool Moved
	{
		get { return _moved; }
		set { _moved = value; }
	}

	public bool IsCleared { get; private set; }
	public PillarType PillarType { get => pillarType; set => pillarType = value; }

	public void MovePlayer()
	{
		// Movement logic
		Moved = true;
	}

	[ContextMenu("Generate Grid")]
	internal override void Generate()
	{
		if (pillerPrefab == null)
		{
			pillerPrefab = Resources.Load<Pillar>("Pillar");
		}
		float x = transform.position.x;
		float y = transform.position.y;
		float z = transform.position.z;
		if (pillarParent == null)
		{
			pillarParent = new GameObject("Pillars");
		}
		if (shouldGeneratePillers)
		{
			PillarType = Random.Range(0, 2) == 1 ? PillarType.ACTIVE : PillarType.DEACTIVE;
		}
		else
		{
			PillarType = PillarType.DEACTIVE;
		}
		//Get the color with same enum from the color manager
		foreach (ColorMaterial colorMaterial in colorManager.colorMaterials)
		{
			if (colorMaterial.colorEnum == GridColor)
			{
				gridMaterial = colorMaterial.material;
				break; // Exit the loop once we find the matching color
			}
		}

		if (gridMaterial == null)
		{
#if UNITY_EDITOR
			Debug.LogError("Grid material is not set! Make sure a valid color is selected.");
#endif
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

		Vector2Int minBounds = GetMinBounds();
		Vector2Int maxBounds = GetMaxBounds();
		rows = (maxBounds.x - minBounds.x) + 1;
		columns = (maxBounds.y - minBounds.y) + 1;

		var gridSize = gridGenerator.GridSize;

		// Generate the grid
		for (int i = minBounds.x; i <= maxBounds.x; i++)
		{
			for (int j = minBounds.y; j <= maxBounds.y; j++)
			{
				if (shouldGeneratePillers)
				{
					//if (
					//	(i == gridSize.x && j == gridSize.y - 1) || //top right
					//	(i == 0 && j == 0) ||//bottom left
					//	(i == gridSize.x - 1 && j == 0) ||//bottom right	
					//	(i == 0 && j == gridSize.y - 1) //top left
					//	)
					//{
					//	Instantiate(pillerPrefab, gridGenerator.GetWorldPosition(i, j, true), Quaternion.identity);
					//	continue;
					//}
					//else 
					#region Generate corner pillars
					if (
						(i == minBounds.x && j == minBounds.y) || //bottom left
						(i == maxBounds.x && j == maxBounds.y) ||//top right
						(i == minBounds.x && j == maxBounds.y) ||//bottom right	
						(i == maxBounds.x && j == minBounds.y) //top left
						)
					{
						//Take grid space bounds to consideration
						if (
							(i != gridSize.x - 1 || j != gridSize.y - 1) && //top right
							(i != 0 || j != 0) &&//bottom left
							(i != gridSize.x - 1 || j != 0) &&//bottom right	
							(i != 0 || j != gridSize.y - 1) //top left
							)
						{
							GeneratePillers(i, j, PillarType);
							continue;
						}

					}
					#endregion
					#region skips the world edges
					if (i == 0 || i == gridSize.x - 1 || j == 0 || j == gridSize.y - 1)
					{
						//I dont know how to invert this or may be I am too lazy to do it
					}
					#endregion
					#region Generates the edge pillars
					//this is good dont change it
					else if (i == minBounds.x || i == maxBounds.x || j == minBounds.y || j == maxBounds.y)
					{
						GeneratePillers(i, j, PillarType);
						continue;
					}
					#endregion
				}



				GameObject cube = Instantiate(Tile);
				cube.transform.position = gridGenerator.GetWorldPosition(i, j, true);

				// Ensure the cube has a GridElement component
				GridElement gridElement = cube.GetComponent<GridElement>();
				if (gridElement != null)
				{
					// Check if playerRenderer exists in the GridElement and set the material
					if (gridElement.playerRenderer != null)
					{
						gridElement.playerRenderer.sharedMaterial = gridMaterial;
					}
					else
					{
#if UNITY_EDITOR
						Debug.LogWarning("playerRenderer is not set on GridElement for " + cube.name);
#endif
					}

					gridElement.Row = i - minBounds.x;
					gridElement.Column = j - minBounds.y;
					cube.transform.gameObject.name = "Grid (" + j + "," + i + ")";
					PlayerPositions.Add(cube.transform);
				}
				else
				{
#if UNITY_EDITOR
					Debug.LogError("GridElement component not found on " + cube.name);
#endif

				}
			}
		}

		// Set the parent for each tile after the grid is generated
		foreach (Transform Tile in PlayerPositions)
		{
			Tile.parent = this.transform;
		}

		TileCount = PlayerPositions.Count;
		AddElementsToGrid();
	}
	private void GeneratePillers(int i, int j, PillarType pillarType)
	{
		var pillar = Instantiate(pillerPrefab, gridGenerator.GetWorldPosition(i, j, true), Quaternion.identity, parent: pillarParent.transform);
		pillar.SetOwner(this, pillarType);
		pillars.Add(pillar);
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
				grid.IsEmpty = false;
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


	public void movePlayerToHole(Hole hole)
	{
		if (IsCleared)
		{
			return;
		}
		if (obstacle == null)
		{
			obstacle = GetComponent<NavMeshObstacle>();
		}
		obstacle.enabled = false;
		StartCoroutine(StartMoving(hole));
		IsCleared = true;
		Moved = true;
		if (shouldGeneratePillers)
		{
			Hole.OnAnyHoleClicked.RemoveListener(SwitchPillarType);
		}
		OnCrowdCleared?.Invoke();
	}

	private IEnumerator StartMoving(Hole hole)
	{
		yield return new WaitForSeconds(0.1f);
		Transform holePosition = hole.transform;

		foreach (GridElement gridElement in playerGridElements)
		{

			// Enable the NavMeshAgent and set the destination
			gridElement.Hole = hole;
			gridElement.agent.enabled = true;
			gridElement.agent.SetDestination(holePosition.position);
			gridElement.agent.stoppingDistance = 0.5f;
			gridElement.StartedRunning = true;
			// Trigger the run animation if applicable
			if (gridElement.animator != null)
			{
				gridElement.animator.SetTrigger("Run");
			}

		}
		// Start movement to the hole
		//StartCoroutine(MoveAllPlayersToHole(hole));

		// Handle unblocking grids
		foreach (CroudManager gridGenerator in blockingGrid)
		{
			gridGenerator.unBlockingGrid.Remove(this);

			if (gridGenerator.unBlockingGrid.Count <= 0)
			{
				gridGenerator.isMovable = true;
			}
		}
	}

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

	internal void PlayNoMoves()
	{
		transform.DOPunchScale(transform.localScale * .1f, 0.2f, 1, 5);
	}

	internal bool CanMove()
	{
		return isMovable && PillarType == PillarType.DEACTIVE;
	}
}
