using System.Collections.Generic;
using UnityEngine;

namespace DT.GridSystem.Ruletile
{
	[CreateAssetMenu(fileName = "GridData", menuName = "Grid/GridData")]
	public class GridData : ScriptableObject
	{
		[SerializeField] private List<RuleState> ruleStates = new List<RuleState>();
		Vector2Int bounds;
		public void InitGridState(Vector2Int bounds)
		{
			int length = bounds.x * bounds.y;
			if (ruleStates.Count == length) return;
			ruleStates = new List<RuleState>(new RuleState[length]);
		}

		public void StoreGridCellState(int x, int y, RuleState state)
		{
			int index = Convert2DIndex(x, y);
			if (index < 0 || index >= ruleStates.Count) return;
			ruleStates[index] = state;
		}

		public RuleState GetGridCellState(int x, int y)
		{
			int index = Convert2DIndex(x, y);

			return ruleStates[index];
		}
		int Convert2DIndex(int x, int y)
		{
			return (y * bounds.x) + x;
		}
		public IEnumerable<RuleState> GetRuleStates()
		{
			return ruleStates;
		}
	}

}