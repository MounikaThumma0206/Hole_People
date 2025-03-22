using UnityEngine;
using DT.GridSystem;
using Unity.AI.Navigation;
using System.Collections.Generic;

/// <summary>
/// Generates a 3D grid using a grid system and manages navigation mesh updates.
/// </summary>
public class GridGenerator : GridSystem3D<GameObject>
{
	/// <summary>
	/// The prefab used to create grid tiles.
	/// </summary>
	public GameObject gridPrefab;
	public GameObject cornerStonePrefab;
	public GameObject wallPrefab;
	public GameObject edgePlanePrefab;
	public float paddingforBoundary;
	public float planeWidth = 5;

	/// <summary>
	/// List of crowd generators that populate the grid with specific elements.
	/// </summary>
	[SerializeField] private List<GridItemGenerator> croudGenerators;

	/// <summary>
	/// The NavMesh surface used for AI navigation.
	/// </summary>
	[SerializeField] private NavMeshSurface navMeshSurface;

	/// <summary>
	/// Parent object for all generated grid related objects.
	/// </summary>
	private GameObject gridObjectsParent;
	/// <summary>
	/// Parent object for all generated grid tiles.
	/// </summary>
	private GameObject tileParent;
	/// <summary>
	/// Parent object for all generated boundary Objects.
	/// </summary>
	private GameObject boundaryParent;

	/// <summary>
	/// Initializes the grid, triggers the generation of crowd elements, and builds the NavMesh.
	/// </summary>
	private void Start()
	{
		// Generate all elements from crowd generators
		foreach (GridItemGenerator generator in croudGenerators)
		{
			generator.Generate();
		}
		PlaceCornerStonesAndWalls(paddingforBoundary);
		PlaceEdgePlanes();
		// Build the navigation mesh after grid generation
		navMeshSurface.BuildNavMesh();
	}

	/// <summary>
	/// Creates a grid tile at the specified coordinates.
	/// </summary>
	/// <param name="gridSystem">Reference to the grid system managing this grid.</param>
	/// <param name="x">The x-coordinate of the grid cell.</param>
	/// <param name="y">The y-coordinate of the grid cell.</param>
	/// <returns>The created grid tile object.</returns>
	public override GameObject CreateGridObject(GridSystem<GameObject> gridSystem, int x, int y)
	{
		if (gridObjectsParent == null)
		{
			gridObjectsParent = new GameObject("Grid Object Parent");
		}
		// Create a parent object for grid tiles if it doesn't exist
		if (tileParent == null)
		{
			tileParent = new GameObject("TileParent");
			tileParent.transform.SetParent(gridObjectsParent.transform);
		}
		if (boundaryParent == null)
		{
			boundaryParent = new GameObject("boundaryParent");
			boundaryParent.transform.SetParent(gridObjectsParent.transform);
		}

		// Instantiate the grid tile at the specified position
		GameObject tile = Instantiate(gridPrefab, GetWorldPosition(x, y, true), Quaternion.identity);
		tile.transform.SetParent(tileParent.transform);
		tile.transform.localScale = Vector3.one * CellSize;

		return tile;
	}

	/// <summary>
	/// Subscribes a new crowd generator to be part of the grid system.
	/// </summary>
	/// <param name="generator">The grid item generator to subscribe.</param>
	public void Subscribe(GridItemGenerator generator)
	{
		croudGenerators ??= new List<GridItemGenerator>();

		if (!croudGenerators.Contains(generator))
		{
			croudGenerators.Add(generator);
		}
	}

	/// <summary>
	/// Unsubscribes a crowd generator, removing it from the grid system.
	/// </summary>
	/// <param name="generator">The grid item generator to unsubscribe.</param>
	public void UnSubscribe(GridItemGenerator generator)
	{
		if (croudGenerators != null && croudGenerators.Contains(generator))
		{
			croudGenerators.Remove(generator);
		}
	}

	/// <summary>
	/// Updates the NavMesh to reflect any changes in the environment.
	/// </summary>
	public void UpdateNavmesh()
	{
		navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
	}

	/// <summary>
	/// Places corner stones and walls according to the grid size with the given padding or offset.
	/// </summary>
	/// <param name="padding">The padding or offset for placing the corner stones and walls.</param>

	private List<GameObject> cornerStones = new List<GameObject>();
	private List<GameObject> walls = new List<GameObject>();
	private List<GameObject> edgePlanes = new List<GameObject>();

	public void PlaceCornerStonesAndWalls(float padding)
	{
		// Define corner stone positions
		Vector3[] cornerPositions = new Vector3[]
		{
		GetWorldPosition(0, 0, true) - new Vector3(padding, 0, padding),  // Bottom-left
        GetWorldPosition(gridSize.x - 1, 0, true) + new Vector3(padding, 0, -padding), // Bottom-right
        GetWorldPosition(0, gridSize.y - 1, true) - new Vector3(padding, 0, -padding), // Top-left
        GetWorldPosition(gridSize.x - 1, gridSize.y - 1, true) + new Vector3(padding, 0, padding) // Top-right
		};

		// Update or create corner stones
		for (int i = 0; i < 4; i++)
		{
			if (i < cornerStones.Count)
			{
				cornerStones[i].transform.position = cornerPositions[i];
			}
			else
			{
				GameObject newCornerStone = Instantiate(cornerStonePrefab, cornerPositions[i], Quaternion.identity, tileParent.transform);
				cornerStones.Add(newCornerStone);
			}
		}

		// Define **exact midpoints** between corner stones for correct wall placement
		Vector3 bottomWallMidPoint = (cornerPositions[0] + cornerPositions[1]) / 2;
		Vector3 topWallMidPoint = (cornerPositions[2] + cornerPositions[3]) / 2;
		Vector3 leftWallMidPoint = (cornerPositions[0] + cornerPositions[2]) / 2;
		Vector3 rightWallMidPoint = (cornerPositions[1] + cornerPositions[3]) / 2;

		Vector3[] wallPositions = new Vector3[]
		{
		bottomWallMidPoint, // Bottom wall
        topWallMidPoint, // Top wall
        leftWallMidPoint, // Left wall
        rightWallMidPoint  // Right wall
		};

		Quaternion[] wallRotations = new Quaternion[]
		{
		Quaternion.Euler(0, 90, 0), // Bottom wall
        Quaternion.Euler(0, 90, 0), // Top wall
        Quaternion.identity, // Left wall
        Quaternion.identity  // Right wall
		};

		Vector3[] wallScales = new Vector3[]
		{
		new Vector3(1, 1, ((gridSize.x - 1) * CellSize) + (2 * padding)), // Bottom wall
        new Vector3(1, 1, ((gridSize.x - 1) * CellSize) + (2 * padding)), // Top wall
        new Vector3(1, 1, ((gridSize.y - 1) * CellSize) + (2 * padding)), // Left wall
        new Vector3(1, 1, ((gridSize.y - 1) * CellSize) + (2 * padding))  // Right wall
		};

		// Update or create walls
		for (int i = 0; i < 4; i++)
		{
			if (i < walls.Count)
			{
				walls[i].transform.position = wallPositions[i];
				walls[i].transform.rotation = wallRotations[i];
				walls[i].transform.localScale = new Vector3(
					walls[i].transform.localScale.x,
					walls[i].transform.localScale.y,
					wallScales[i].z // Only update Z scale
				);
			}
			else
			{
				GameObject newWall = Instantiate(wallPrefab, wallPositions[i], wallRotations[i], boundaryParent.transform);
				newWall.transform.localScale = wallScales[i];
				walls.Add(newWall);
			}
		}
	}
	public void PlaceEdgePlanes()
	{
		if (walls.Count < 4 || edgePlanePrefab == null) return; // Ensure walls exist before placing planes

		float verticalPlaneWidth = planeWidth;
		float topPlaneWidth = planeWidth * 1.5f;
		float bottomPlaneWidth = planeWidth * 0.5f;




		float offset = verticalPlaneWidth / 2f; // Offset for plane width
		float topOffset = topPlaneWidth / 2f; // Offset for plane width
		float bottomOffset = bottomPlaneWidth / 2f; // Offset for plane width

		// Define corner stone positions
		Vector3[] cornerPositions = new Vector3[]
		{
		GetWorldPosition(0, 0, true) ,  // Bottom-left
        GetWorldPosition(gridSize.x - 1, 0, true), // Bottom-right
        GetWorldPosition(0, gridSize.y - 1, true), // Top-left
        GetWorldPosition(gridSize.x - 1, gridSize.y - 1, true) // Top-right
		};
		// Define **exact midpoints** between corner stones for correct wall placement
		Vector3 bottomWallMidPoint = (cornerPositions[0] + cornerPositions[1]) / 2;
		Vector3 topWallMidPoint = (cornerPositions[2] + cornerPositions[3]) / 2;
		Vector3 leftWallMidPoint = (cornerPositions[0] + cornerPositions[2]) / 2;
		Vector3 rightWallMidPoint = (cornerPositions[1] + cornerPositions[3]) / 2;

		// **Edge planes positioned fully outside the grid**
		Vector3 bottomPlanePos = bottomWallMidPoint - new Vector3(0, 0, bottomOffset + CellSize);  // Below the grid
		Vector3 topPlanePos = topWallMidPoint + new Vector3(0, 0, topOffset + CellSize); // Above the grid
		Vector3 leftPlanePos = leftWallMidPoint - new Vector3(offset + CellSize, 0, 0);  // Left of the grid
		Vector3 rightPlanePos = rightWallMidPoint + new Vector3(offset + CellSize, 0, 0); // Right of the grid

		Vector3[] planePositions = new Vector3[] { bottomPlanePos, topPlanePos, leftPlanePos, rightPlanePos };

		Quaternion[] planeRotations = new Quaternion[]
		{
		Quaternion.Euler(0, 90, 0), // Bottom plane (horizontal)
        Quaternion.Euler(0, 90, 0), // Top plane (horizontal)
        Quaternion.Euler(0, 0, 0),  // Left plane (vertical)
        Quaternion.Euler(0, 0, 0)   // Right plane (vertical)
		};

		Vector3[] planeScales = new Vector3[]
		{
		new Vector3(bottomPlaneWidth, 1, (gridSize.x * CellSize)+planeWidth*2+CellSize), // Bottom plane (horizontal)
        new Vector3(topPlaneWidth, 1, (gridSize.x * CellSize)+planeWidth*2+CellSize), // Top plane (horizontal)
        new Vector3(verticalPlaneWidth, 1, gridSize.y * CellSize+CellSize), // Left plane (vertical)
        new Vector3(verticalPlaneWidth, 1, gridSize.y * CellSize+CellSize)  // Right plane (vertical)
		};

		// Update or create edge planes
		for (int i = 0; i < 4; i++)
		{
			if (i < edgePlanes.Count)
			{
				edgePlanes[i].transform.position = planePositions[i];
				edgePlanes[i].transform.rotation = planeRotations[i];
				edgePlanes[i].transform.localScale = planeScales[i]; // Update scaling
			}
			else
			{
				GameObject newPlane = Instantiate(edgePlanePrefab, planePositions[i], planeRotations[i], boundaryParent.transform);
				newPlane.transform.localScale = planeScales[i];
				edgePlanes.Add(newPlane);
			}
		}
	}

}



