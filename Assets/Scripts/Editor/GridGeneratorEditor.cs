using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridGenerator))]
[CanEditMultipleObjects]
public class GridGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{

		GridGenerator[] boxDataArray = new GridGenerator[targets.Length];
		for (int i = 0; i < targets.Length; i++)
		{
			boxDataArray[i] = (GridGenerator)targets[i];
		}

		if (GUILayout.Button("Generate grid tiles"))
		{
			//foreach (var
			//	boxData in boxDataArray)
			//{
			//	boxData.GenerateTiles();
			//}
		}
		DrawDefaultInspector();
	}
}
