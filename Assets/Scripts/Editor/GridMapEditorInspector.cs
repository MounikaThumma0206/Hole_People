using UnityEngine;
using UnityEditor;
namespace DT.GridSystem.Ruletile
{
	[CustomEditor(typeof(GridMapGenerator))]
	public class GridMapEditorInspector : Editor
	{
		private GridMapGenerator editor;

		private void OnEnable()
		{
			editor = (GridMapGenerator)target;
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if (GUILayout.Button(editor.editMode ? "Exit Edit Mode" : "Edit"))
			{
				editor.editMode = !editor.editMode;
				SceneView.RepaintAll();
			}
		}

		private void OnSceneGUI()
		{
			if (!editor.editMode || editor.GridData == null) return;
			Color col=Color.white;
			col.a = 0f;
			Handles.color =col;
			for (int x = 0; x < editor.GridSystem.GridSize.x; x++)
			{
				for (int y = 0; y < editor.GridSystem.GridSize.y; y++)
				{					
					Vector3 cellPosition = editor.GridSystem.GetWorldPosition(x, y, true);
					float cellSize = editor.GridSystem.CellSize * 0.8f;
					if (Handles.Button(cellPosition, Quaternion.identity, cellSize, cellSize, Handles.CubeHandleCap))
					{
						Undo.RecordObject(editor.gameObject, "Changed grid map");
						RuleState newState = editor.GridData.GetGridCellState(x,y) == RuleState.Tile_Exist ? RuleState.No_Tile : RuleState.Tile_Exist;
						editor.GridData.StoreGridCellState(x,y, newState);
						editor.PlaceObjectInTile(x,y);
						EditorUtility.SetDirty(editor.GridData);
					}
				}
			}
		}
	}
}