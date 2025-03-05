using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HoleGenerator))]
public class BoxDataEditor : Editor
{
	private void OnSceneGUI()
	{
		HoleGenerator boxData = (HoleGenerator)target;
		Transform transform = boxData.transform;

		// Fixed position (GameObject's transform position)
		Vector3 worldPosition = transform.position;

		// Handle for resizing the box
		EditorGUI.BeginChangeCheck();
		Vector3 newSize = Handles.ScaleHandle(boxData.BoxSize, worldPosition, Quaternion.identity, HandleUtility.GetHandleSize(worldPosition));
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(boxData, "Resize Box");
			boxData.BoxSize = newSize;
		}

		// Draw a box in Scene View
		Handles.color = new Color(0, 1, 0, 0.2f);
		Handles.DrawWireCube(worldPosition, boxData.BoxSize);
	}
}