using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("UI Elements")]
    public TextMeshProUGUI levelText; // Assign in the Inspector.
    public TextMeshProUGUI movesText; // Assign in the Inspector.

    [Header("Game Settings")]
    public int initialMoves = 10; // Set the initial number of moves per level.
    public List<GameObject> levelPrefabs; // Assign your 5 level prefabs in the Inspector.

    private int currentMoves;
    private GameObject currentLevelInstance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadLevel();
    }

    public static int GetCurrentLevelNumber()
    {
        return PlayerPrefs.GetInt("LevelProgress", 1);
    }

    public void LoadLevel()
    {
        int levelNumber = GetCurrentLevelNumber();

        // Destroy the previous level instance if it exists
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        // Instantiate the new level prefab
        if (levelNumber - 1 < levelPrefabs.Count && levelPrefabs[levelNumber - 1] != null)
        {
            currentLevelInstance = Instantiate(levelPrefabs[levelNumber - 1]);
            Debug.Log($"Level {levelNumber} prefab instantiated.");
        }
        else
        {
            Debug.LogError($"Level {levelNumber} prefab is missing or out of range.");
            return;
        }

        // Initialize UI and moves
        InitializeLevelUI();
    }

    public void InitializeLevelUI()
    {
        // Initialize level text
        UpdateLevelUI();

        // Reset and display the moves for the level
        currentMoves = initialMoves;
        UpdateMovesUI();
    }

    public void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + GetCurrentLevelNumber();
        }
        else
        {
            Debug.LogError("Level Text UI component is not assigned in the Inspector.");
        }
    }

    public void UpdateMovesUI()
    {
        if (movesText != null)
        {
            movesText.text = "Moves: " + currentMoves;
        }
        else
        {
            Debug.LogError("Moves Text UI component is not assigned in the Inspector.");
        }
    }

    public void UseMove()
    {
        if (currentMoves > 0)
        {
            currentMoves--;
            UpdateMovesUI();

            if (currentMoves <= 0)
            {
                Debug.Log("Out of moves! Trigger level fail condition.");
                // Add game-over logic here
            }
        }
    }

    public static void ResetLevelProgress()
    {
        PlayerPrefs.SetInt("LevelProgress", 1);
        PlayerPrefs.Save();
    }

    public void GoToNextLevel()
    {
        int currentLevel = GetCurrentLevelNumber();
        int nextLevel = currentLevel + 1;

        if (nextLevel <= levelPrefabs.Count)
        {
            PlayerPrefs.SetInt("LevelProgress", nextLevel);
            PlayerPrefs.Save();
            LoadLevel();
        }
        else
        {
            Debug.Log("No more levels to load. You reached the end!");
        }
    }
}



//using UnityEngine;
//using UnityEngine.SceneManagement;
//using TMPro;
//using System.Collections.Generic;

//public class LevelManager : MonoBehaviour
//{
//    public static LevelManager instance;
//    public TextMeshProUGUI levelText; // Make sure this is assigned in the Inspector.

//    private static List<int> loadedLevels = new List<int>();

//    public void Awake()
//    {
//        instance = this;
//    }

//    private void Start()
//    {
//        LoadLevel();
//    }

//    public static int GetCurrentLevelNumber()
//    {
//        return PlayerPrefs.GetInt("LevelProgress", 1);
//    }

//    public static void LoadLevel(int levelIndex)
//    {
//        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
//        {
//            SceneManager.LoadScene(levelIndex);
//            PlayerPrefs.SetInt("LevelProgress", levelIndex);
//            PlayerPrefs.Save();
//        }
//        else
//        {
//            Debug.LogError("Invalid level index: " + levelIndex);
//        }
//    }

//    public static void LoadLevel()
//    {
//        int progress = GetCurrentLevelNumber();
//        int totalLevels = SceneManager.sceneCountInBuildSettings;
//        Debug.Log("Current Level Progress: " + progress);
//        Debug.Log("Total Levels: " + totalLevels);
//        if (progress < totalLevels)
//        {
//            LoadLevel(progress);
//        }
//        else
//        {
//            int randomLevel = GetUniqueRandomLevel();
//            LoadLevel(randomLevel);
//        }
//        // Call UpdateLevelUI after the level is loaded
//        Instance.UpdateLevelUI();
//    }

//    public void UpdateLevelUI()
//    {
//        if (levelText != null)
//        {
//            levelText.text = "Level: " + GetCurrentLevelNumber();
//        }
//        else
//        {
//            Debug.LogError("Level Text UI component is not assigned in the Inspector.");
//        }
//    }

//    private static int GetUniqueRandomLevel()
//    {
//        string loadedLevelsStr = PlayerPrefs.GetString("LoadedLevels", "");
//        if (!string.IsNullOrEmpty(loadedLevelsStr))
//        {
//            loadedLevels = new List<int>(System.Array.ConvertAll(loadedLevelsStr.Split(','), int.Parse));
//        }

//        int totalLevels = SceneManager.sceneCountInBuildSettings;
//        List<int> availableLevels = new List<int>();

//        for (int i = 0; i < totalLevels; i++)
//        {
//            if (!loadedLevels.Contains(i))
//            {
//                availableLevels.Add(i);
//            }
//        }

//        if (availableLevels.Count == 0)
//        {
//            loadedLevels.Clear();
//            for (int i = 0; i < totalLevels; i++)
//            {
//                availableLevels.Add(i);
//            }
//        }

//        int randomIndex = UnityEngine.Random.Range(0, availableLevels.Count);
//        int randomLevel = availableLevels[randomIndex];

//        loadedLevels.Add(randomLevel);
//        PlayerPrefs.SetString("LoadedLevels", string.Join(",", loadedLevels));
//        PlayerPrefs.Save();

//        return randomLevel;
//    }

//    public static void ResetLevelProgress()
//    {
//        PlayerPrefs.SetInt("LevelProgress", 1);
//        PlayerPrefs.Save();
//    }

//    // Singleton pattern to access the LevelManager instance
//    private static LevelManager _instance;
//    public static LevelManager Instance
//    {
//        get
//        {
//            if (_instance == null)
//            {
//                _instance = FindObjectOfType<LevelManager>();
//                if (_instance == null)
//                {
//                    GameObject obj = new GameObject("LevelManager");
//                    _instance = obj.AddComponent<LevelManager>();
//                }
//            }
//            return _instance;
//        }
//    }
//}
