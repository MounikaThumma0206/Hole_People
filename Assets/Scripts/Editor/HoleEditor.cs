using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Hole))]
[CanEditMultipleObjects]
public class HoleEditor : Editor
{
	public override void OnInspectorGUI()
	{

		//Hole[] boxDataArray = new Hole[targets.Length];
		//for (int i = 0; i < targets.Length; i++)
		//{
		//	boxDataArray[i] = (Hole)targets[i];
		//}

		//if (GUILayout.Button("Calculate Navmesh Size"))
		//{
		//	foreach (Hole boxData in boxDataArray)
		//	{
		//		boxData.CalculateNavmeshSize();
		//	}
		//}
		DrawDefaultInspector();
	}
}
