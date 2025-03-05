using UnityEditor;
using UnityEngine;

public class NavMeshObstacleResizer : EditorWindow
{
	[MenuItem("Tools/Recalculate All NavMesh Obstacles")]
	public static void RecalculateAllNavMeshObstacles()
	{
		CroudManager[] updaters = FindObjectsOfType<CroudManager>();

		if (updaters.Length == 0)
		{
			Debug.LogWarning("No NavMeshObstacleUpdater components found in the scene.");
			return;
		}

		foreach (var updater in updaters)
		{
			updater.CalculateNavmeshSize();
		}

		Debug.Log($"Recalculated {updaters.Length} NavMesh Obstacles.");
	}
}
