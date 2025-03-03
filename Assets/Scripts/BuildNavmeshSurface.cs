using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
[RequireComponent(typeof(NavMeshSurface))]
public class BuildNavmeshSurface : MonoBehaviour
{
	[SerializeField] NavMeshSurface navMeshSurface;
	[SerializeField] GameObject Characters;
	private void Start()
	{
		navMeshSurface = GetComponent<NavMeshSurface>();
		navMeshSurface.BuildNavMesh();
	}
	IEnumerator SpawnLevelElements()
	{
		yield return new WaitForEndOfFrame();
		navMeshSurface.BuildNavMesh();
	}
}