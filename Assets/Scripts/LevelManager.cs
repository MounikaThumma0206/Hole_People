using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public TextMeshProUGUI levelText; // Make sure this is assigned in the Inspector.

    private static List<int> loadedLevels = new List<int>();

    public void Awake()
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

    public static void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(levelIndex);
            PlayerPrefs.SetInt("LevelProgress", levelIndex);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogError("Invalid level index: " + levelIndex);
        }
    }

    public static void LoadLevel()
    {
        int progress = GetCurrentLevelNumber();
        int totalLevels = SceneManager.sceneCountInBuildSettings;
        Debug.Log("Current Level Progress: " + progress);
        Debug.Log("Total Levels: " + totalLevels);
        if (progress < totalLevels)
        {
            LoadLevel(progress);
        }
        else
        {
            int randomLevel = GetUniqueRandomLevel();
            LoadLevel(randomLevel);
        }
        // Call UpdateLevelUI after the level is loaded
        Instance.UpdateLevelUI();
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

    private static int GetUniqueRandomLevel()
    {
        string loadedLevelsStr = PlayerPrefs.GetString("LoadedLevels", "");
        if (!string.IsNullOrEmpty(loadedLevelsStr))
        {
            loadedLevels = new List<int>(System.Array.ConvertAll(loadedLevelsStr.Split(','), int.Parse));
        }

        int totalLevels = SceneManager.sceneCountInBuildSettings;
        List<int> availableLevels = new List<int>();

        for (int i = 0; i < totalLevels; i++)
        {
            if (!loadedLevels.Contains(i))
            {
                availableLevels.Add(i);
            }
        }

        if (availableLevels.Count == 0)
        {
            loadedLevels.Clear();
            for (int i = 0; i < totalLevels; i++)
            {
                availableLevels.Add(i);
            }
        }

        int randomIndex = UnityEngine.Random.Range(0, availableLevels.Count);
        int randomLevel = availableLevels[randomIndex];

        loadedLevels.Add(randomLevel);
        PlayerPrefs.SetString("LoadedLevels", string.Join(",", loadedLevels));
        PlayerPrefs.Save();

        return randomLevel;
    }

    public static void ResetLevelProgress()
    {
        PlayerPrefs.SetInt("LevelProgress", 1);
        PlayerPrefs.Save();
    }

    // Singleton pattern to access the LevelManager instance
    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("LevelManager");
                    _instance = obj.AddComponent<LevelManager>();
                }
            }
            return _instance;
        }
    }
}
