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

	/// <summary>
	/// List of crowd generators that populate the grid with specific elements.
	/// </summary>
	[SerializeField] private List<GridItemGenerator> croudGenerators;

	/// <summary>
	/// The NavMesh surface used for AI navigation.
	/// </summary>
	[SerializeField] private NavMeshSurface navMeshSurface;

	/// <summary>
	/// Parent object for all generated grid tiles.
	/// </summary>
	private GameObject tileParent;

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
		// Create a parent object for grid tiles if it doesn't exist
		if (tileParent == null)
		{
			tileParent = new GameObject("TileParent");
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
}
