using UnityEngine;
using DT.GridSystem;
using Unity.AI.Navigation;
using System.Collections.Generic;
public class GridGenerator : GridSystem3D<GameObject>
{
	public GameObject gridPrefab;

	[SerializeField] private List<GridItemGenerator> croudGenerators;
	[SerializeField] NavMeshSurface navMeshSurface;
	GameObject tileParent;
	private void Start()
	{
		foreach (GridItemGenerator generator in croudGenerators)
		{
			generator.Generate();
		}
		navMeshSurface.BuildNavMesh();
	}
	public override GameObject CreateGridObject(GridSystem<GameObject> gridSystem, int x, int y)
	{ 
		if (tileParent == null)
		{
			tileParent = new GameObject("TileParent");
		}
		GameObject tile = Instantiate(gridPrefab, GetWorldPosition(x, y, true), Quaternion.identity);
		tile.transform.SetParent(tileParent.transform);
		tile.transform.localScale = Vector3.one * CellSize;
		return tile;
	}
	public void Subscribe(GridItemGenerator generator)
	{
		croudGenerators ??= new List<GridItemGenerator>();
		if (!croudGenerators.Contains(generator))
		{
			croudGenerators.Add(generator);
		}
	}
	public void UnSubscribe(GridItemGenerator generator)
	{
		if (croudGenerators != null && croudGenerators.Contains(generator))
		{
			croudGenerators.Remove(generator);
		}
	}
}
