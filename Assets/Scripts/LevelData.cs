using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "ScriptableObjects/LevelData", order = 51)]
public class LevelData : ScriptableObject
{
	public Vector3 cameraPosition;
	public float cameraSize;
	public int maxMoves;
	public Vector3 cameraRotation;
	public GameObject levelPrefab;
}
