using System.Collections.Generic;
using UnityEngine;

namespace DT.GridSystem.Ruletile
{
	public enum RuleState
	{
		No_Mention = 0,
		No_Tile = 1,
		Tile_Exist = 2
	}
	[CreateAssetMenu(fileName = "New Ruletile", menuName = "DT/Grid/Ruletile")]
	public class Ruletile : ScriptableObject
	{
		[System.Serializable]
		public class Ruleset // Changed from struct to class for better serialization support
		{
			public GameObject prefab;
			public RuleState[] ruleState;
			public RuleState GetRuleState(int x, int y)
			{
				int index = Convert2DIndex(x, y);
				return ruleState[index];
			}
			int Convert2DIndex(int x, int y)
			{
				return (y * 3) + x;
			}
		}
		public GameObject defaultPrefab;
		[SerializeField] private List<Ruleset> gridObjects = new List<Ruleset>(); // Using List for Unity Serialization

		public List<Ruleset> GetRuleSets() => gridObjects;		
	}

}