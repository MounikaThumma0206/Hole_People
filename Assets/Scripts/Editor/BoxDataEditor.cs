using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CroudManager))]
[CanEditMultipleObjects]
public class BoxDataEditor : Editor
{
	public override void OnInspectorGUI()
	{

		CroudManager[] boxDataArray = new CroudManager[targets.Length];
		for (int i = 0; i < targets.Length; i++)
		{
			boxDataArray[i] = (CroudManager)targets[i];
		}

		if (GUILayout.Button("Calculate Navmesh Size"))
		{
			foreach (CroudManager boxData in boxDataArray)
			{
				boxData.CalculateNavmeshSize();
			}
		}
		DrawDefaultInspector();
	}
}
