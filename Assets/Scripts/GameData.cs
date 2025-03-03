using UnityEngine;

[CreateAssetMenu(fileName = "NewGameData", menuName = "ScriptableObjects/GameData", order = 50)]
public class GameData : ScriptableObject
{
	[SerializeField] LevelData[] levels;

	public LevelData[] Levels { get => levels;}
}