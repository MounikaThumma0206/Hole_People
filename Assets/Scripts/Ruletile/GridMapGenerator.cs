using UnityEditor;
using UnityEngine;
using static DT.GridSystem.Ruletile.Ruletile;

namespace DT.GridSystem.Ruletile
{
	[RequireComponent(typeof(GridSystem<>))]
	public class GridMapGenerator : MonoBehaviour
	{
		[SerializeField] private GridData gridData;
		[SerializeField] private Ruletile ruletile;
		[SerializeField] private GridSystem<GameObject> gridSystem;

		[HideInInspector] public bool editMode = false;
		Vector2Int bounds;

		public GridSystem<GameObject> GridSystem { get => gridSystem; }
		public Ruletile Ruletile { get => ruletile; }
		public GridData GridData { get => gridData; }

		void Start()
		{
			gridSystem = GetComponent<GridSystem<GameObject>>();
			bounds = GridSystem.GridSize;
			if (GridData == null) return;
			GridData.InitGridState(bounds);
		}
		public void PlaceObjectInTile(int x, int y)
		{
			GameObject gridObject = null;
			foreach (Ruletile.Ruleset ruleSet in ruletile.GetRuleSets())
			{
				if (CheckMatch(x, y, ruleSet) == 9)
				{
					//Note: delete if there is already a object exist
					if (gridSystem.GetGridObject(x, y) != null)
					{
						DestroyImmediate(gridSystem.RemoveGridObject(x, y));
					}
					gridObject = (GameObject)PrefabUtility.InstantiatePrefab(ruleSet.prefab, transform);
					gridSystem.AddGridObject(x, y, gridObject, true);
					UpdateAdjacentTiles(x, y);
					return;
				}
			}

			//Note: delete if there is already a object exist
			if (gridSystem.GetGridObject(x, y) != null)
			{
				DestroyImmediate(gridSystem.RemoveGridObject(x, y));
			}
			gridObject = (GameObject)PrefabUtility.InstantiatePrefab(ruletile.defaultPrefab, transform);
			gridSystem.AddGridObject(x, y, gridObject, true);
			UpdateAdjacentTiles(x, y);
		}
		void UpdateAdjacentTiles(int x, int y)
		{
			for (int i = x - 1; i <= x + 1; i++)
			{
				if (i < 0 || i >= bounds.x) continue;
				for (int j = y - 1; j <= y; j++)
				{
					if (j < 0 || j >= bounds.y) continue;
					if (i == x && j == y) continue;
					PlaceObjectInTile(i, j);
				}
			}
		}
		int CheckMatch(int x, int y, Ruleset ruleSet)
		{
			int count = 0;
			for (int i = x - 1; i <= x + 1; i++)
			{
				if (i < 0 || i >= bounds.x) continue;

				for (int j = y - 1; j <= y; j++)
				{
					if (j < 0 || j >= bounds.y) continue;
					if (ruleSet.GetRuleState(i, j) == gridData.GetGridCellState(i, j))
					{
						count++;
					}
				}
			}
			return count;
		}

#if UNITY_EDITOR
		void OnGUI()
		{
			if (GUILayout.Button(editMode ? "Exit Edit Mode" : "Edit"))
			{
				editMode = !editMode;
			}
		}
		private void OnDrawGizmos()
		{
			int i = 0;
			foreach (var item in GridData.GetRuleStates())
			{
				if (item == RuleState.No_Mention)
				{
					i++;
					continue;
				}
				int x = i % GridSystem.GridSize.y;
				int y = Mathf.FloorToInt(i / GridSystem.GridSize.y);
				Gizmos.color = item == RuleState.Tile_Exist ? Color.green : Color.red;
				Gizmos.DrawCube(GridSystem.GetWorldPosition(x, y, true), 0.8f * GridSystem.CellSize * Vector3.one);
				i++;
			}
		}
#endif
	}

}