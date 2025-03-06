using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	[SerializeField] List<CroudManager> playerGrids = new List<CroudManager>();
	//public List<GridElement> playerGridElements;

	[SerializeField] GameData gameData;
	private int usedMoves = 0;
	[SerializeField] private int maxMoves = 3;
	bool isGameOn = true;
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			DOTween.Init();
		}
		else
		{
			DOTween.KillAll();
			DestroyImmediate(gameObject);
		}

		usedMoves = 0;

	}

	private void OnEnable()
	{
		playerGrids.Clear();

	}
	private void Start()
	{
		UiManager.instance.UpdateMoveText(GetAvailableMoves());
		GenerateLevel();
	}

	public bool IsGameOn()
	{
		return isGameOn;
	}

	void GenerateLevel()
	{
		var levelData = gameData.Levels[Random.Range(0, gameData.Levels.Length)];
		maxMoves = levelData.maxMoves;
		usedMoves = 0;
		Instantiate(levelData.levelPrefab);
	}

	public void Restart()
	{
		Instance.playerGrids.Clear();
		DOTween.KillAll();
		usedMoves = 0;
		var asyncOp = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
		asyncOp.completed += (op) =>
		{
			isGameOn = true;
			GenerateLevel();
			UiManager.instance.UpdateMoveText(GetAvailableMoves());
		};
	}
	List<CroudManager> notMovables = new List<CroudManager>();
	public void IsPlayerMovable(ColorEnum color, Hole hole)
	{
		bool hadMovables = false;
		bool isAllClear = true;
		notMovables.Clear();
		int totalPeopleAttracted = 0;
		foreach (CroudManager generator in playerGrids)
		{
			if (generator.GridColor == color)
			{
				if (!generator.Moved)
				{
					if (generator.CanMove())
					{
						totalPeopleAttracted += generator.TileCount;
						generator.movePlayerToHole(hole);
						hadMovables = true;
						isAllClear = isAllClear && generator.IsCleared;
					}
					else
					{
						notMovables.Add(generator);
					}
				}
			}
		}
		if (!hadMovables)
		{
			hole.PlayNoMoves();
			foreach (CroudManager generator in notMovables)
			{
				generator.PlayNoMoves();
			}
		}
		else if (isAllClear)
		{
			hole.CloseHoleAfterEating(totalPeopleAttracted);
			CheckLevelComplete();
		}
		else
		{
			CheckLevelComplete();
		}
	}

	public void SusbscribeGenerators(CroudManager generator)
	{
		playerGrids.Add(generator);
	}
	public void UnSubscribeGenerators(CroudManager generator)
	{
		playerGrids.Remove(generator);
	}
	internal void UseMove()
	{
		usedMoves++;
		UiManager.instance.UpdateMoveText(GetAvailableMoves());
		if (GetAvailableMoves() > 0) return;
		foreach (CroudManager generator in playerGrids)
		{
			if (!generator.IsCleared)
			{
				if (UiManager.instance != null)
				{
					isGameOn = false;
					UiManager.instance.GameOver();
				}
			}
		}
	}

	public int GetAvailableMoves()
	{
		int max = maxMoves - usedMoves;
		return Mathf.Clamp(max, 0, maxMoves);
	}

	public void CheckLevelComplete()
	{
		bool completed = true;

		foreach (CroudManager _grid in playerGrids)
		{
			if (!_grid._moved)
			{
				completed = false;
				break;
			}
		}

		if (completed)
		{
			if (UiManager.instance != null)
			{
				UiManager.instance.LevelComplete();
			}
		}

	}
}
