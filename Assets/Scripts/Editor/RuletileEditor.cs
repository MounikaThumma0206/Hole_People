using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
namespace DT.GridSystem.Ruletile
{
	[CustomEditor(typeof(Ruletile))]
	public class RuletileEditor : Editor
	{
		private const int PrefabSlotWidth = 200;
		private Dictionary<int, bool> editMode = new Dictionary<int, bool>();
		private RuletileGridEditor gridEditor;
		private SerializedProperty gridObjectsProperty;
		private SerializedProperty defaultPrefabProperty;

		private void OnEnable()
		{
			gridEditor = new RuletileGridEditor();
			gridObjectsProperty = serializedObject.FindProperty("gridObjects");
			defaultPrefabProperty = serializedObject.FindProperty("defaultPrefab");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.LabelField("🟦 Ruletile Configuration 🟦", EditorStyles.boldLabel);
			EditorGUILayout.Space(5);

			defaultPrefabProperty.objectReferenceValue = EditorGUILayout.ObjectField("Default Prefab", defaultPrefabProperty.objectReferenceValue, typeof(GameObject), false);
			EditorGUILayout.Space(5);

			if (GUILayout.Button("Add Ruleset"))
			{
				gridObjectsProperty.arraySize++;
				SerializedProperty newRuleSet = gridObjectsProperty.GetArrayElementAtIndex(gridObjectsProperty.arraySize - 1);
				newRuleSet.FindPropertyRelative("prefab").objectReferenceValue = null;
				SerializedProperty ruleStateArray = newRuleSet.FindPropertyRelative("ruleState");
				ruleStateArray.arraySize = 9;
				for (int j = 0; j < 9; j++)
				{
					ruleStateArray.GetArrayElementAtIndex(j).enumValueIndex = (int)RuleState.No_Mention;
				}
				editMode[gridObjectsProperty.arraySize - 1] = false;
			}

			for (int i = 0; i < gridObjectsProperty.arraySize; i++)
			{
				if (!editMode.ContainsKey(i))
					editMode[i] = false;

				SerializedProperty ruleSetProperty = gridObjectsProperty.GetArrayElementAtIndex(i);
				SerializedProperty prefabProperty = ruleSetProperty.FindPropertyRelative("prefab");
				SerializedProperty ruleStateArray = ruleSetProperty.FindPropertyRelative("ruleState");

				EditorGUILayout.BeginHorizontal(GUI.skin.box);
				prefabProperty.objectReferenceValue = EditorGUILayout.ObjectField("Prefab", prefabProperty.objectReferenceValue, typeof(GameObject), false, GUILayout.Width(PrefabSlotWidth));

				if (GUILayout.Button(editMode[i] ? "Done" : "Edit", GUILayout.Width(50)))
				{
					editMode[i] = !editMode[i];
				}

				if (GUILayout.Button("↑", GUILayout.Width(20)) && i > 0)
				{
					gridObjectsProperty.MoveArrayElement(i, i - 1);
				}
				if (GUILayout.Button("↓", GUILayout.Width(20)) && i < gridObjectsProperty.arraySize - 1)
				{
					gridObjectsProperty.MoveArrayElement(i, i + 1);
				}

				if (GUILayout.Button("X", GUILayout.Width(20)))
				{
					gridObjectsProperty.DeleteArrayElementAtIndex(i);
					editMode.Remove(i);
					continue;
				}

				EditorGUILayout.EndHorizontal();

				if (editMode[i])
				{
					gridEditor.DrawGrid(ruleStateArray);
				}

				EditorGUILayout.Space(5);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}

	public class RuletileGridEditor
	{
		private const int GridSize = 3;
		private const int SlotSize = 40;
		private Texture2D noTileIcon;
		private Texture2D tileExistIcon;
		private Texture2D noMentionIcon;

		public RuletileGridEditor()
		{
			LoadIcons();
		}

		private void LoadIcons()
		{
			noTileIcon = EditorGUIUtility.IconContent("d_winbtn_mac_close").image as Texture2D;
			tileExistIcon = EditorGUIUtility.IconContent("d_Toggle Icon").image as Texture2D;
			noMentionIcon = EditorGUIUtility.IconContent("d_scenevis_visible_hover").image as Texture2D;
		}

		public void DrawGrid(SerializedProperty ruleStateArray)
		{
			EditorGUILayout.BeginVertical();
			for (int y = 0; y < GridSize; y++)
			{
				EditorGUILayout.BeginHorizontal();
				for (int x = 0; x < GridSize; x++)
				{
					int index = y * GridSize + x;
					SerializedProperty stateProperty = ruleStateArray.GetArrayElementAtIndex(index);

					Texture2D icon = noMentionIcon;
					if (stateProperty.enumValueIndex == (int)RuleState.No_Tile) icon = noTileIcon;
					if (stateProperty.enumValueIndex == (int)RuleState.Tile_Exist) icon = tileExistIcon;

					if (GUILayout.Button(icon, GUILayout.Width(SlotSize), GUILayout.Height(SlotSize)))
					{
						stateProperty.enumValueIndex = (stateProperty.enumValueIndex + 1) % 3;
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}
	}



}