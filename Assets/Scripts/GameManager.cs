using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
	private const string CurrentLevelKey = "CurrentLevel";
	public static GameManager Instance;
	List<CroudManager> playerGrids = new List<CroudManager>();
	List<Hole> holes = new List<Hole>();
	//public List<GridElement> playerGridElements;

	[SerializeField] GameData gameData;
	private int usedMoves = 0;
	[SerializeField] private int maxMoves = 3;
	bool isGameOn = true;
	float gameActiveDelay = .1f;
	float lastTimeGameBecameActive = 0;

#if UNITY_EDITOR

	[SerializeField] int levelTOloadDebug = 1;
#endif
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

#if UNITY_EDITOR
	[ContextMenu("Load")]
	private void LoadLevel()
	{
		PlayerPrefs.SetInt(CurrentLevelKey, levelTOloadDebug);
		Restart();
	}
#endif
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
		return isGameOn && Time.time - lastTimeGameBecameActive > gameActiveDelay;
	}
	public void SetGameOn(bool value)
	{
		isGameOn = value;
		if (value == true)
		{
			lastTimeGameBecameActive = Time.time;
		}
	}
	void GenerateLevel()
	{
		int levelToLoad = GetLevel() - 1;
		var levelData = gameData.Levels[levelToLoad];
		maxMoves = levelData.maxMoves;
		usedMoves = 0;
		Instantiate(levelData.levelPrefab);

		foreach (var hole in holes)
		{
			//set hole's player count
		}
	}
	public void LoadNext()
	{
		int currentLevel = GetLevel() - 1;
		if (currentLevel >= gameData.Levels.Length - 1)
		{
			PlayerPrefs.SetInt(CurrentLevelKey, 0);
		}
		else
		{
			PlayerPrefs.SetInt(CurrentLevelKey, currentLevel + 1);
		}
		PlayerPrefs.Save();
		Restart();
	}
	public void Restart()
	{
		Instance.playerGrids.Clear();
		DOTween.KillAll();
		usedMoves = 0;
		var asyncOp = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
		asyncOp.completed += (op) =>
		{
			SetGameOn(true);
			CrowdAudioManager.MakeNormalMood();
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

		foreach (CroudManager generator in new List<CroudManager>(playerGrids))
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
	public void SusbscribeHole(Hole hole)
	{
		holes.Add(hole);
	}
	public void UnSubscribeHole(Hole hole)
	{
		holes.Remove(hole);
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
					SetGameOn(false);
					CrowdAudioManager.MakeSadMood();

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
			if (!_grid.Moved)
			{
				completed = false;
				break;
			}
		}

		if (completed)
		{
			if (UiManager.instance != null)
			{
				SetGameOn(false);
				UiManager.instance.LevelComplete();
			}
		}

	}

	internal int GetLevel()
	{
		return PlayerPrefs.GetInt(CurrentLevelKey, 0) + 1;
	}
}
