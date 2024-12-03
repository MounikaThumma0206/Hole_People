using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private static List<int> loadedLevels = new List<int>();

    private void Start()
    {
        LoadLevel();  // Start with the current level
    }

    // Retrieves the current level progress
    public static int GetCurrentLevelNumber()
    {
        return PlayerPrefs.GetInt(PlayerPrefsManager.LevelProgress, 1);
    }

    // Reloads the current level
    public static void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Loads a specific level by index
    public static void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(levelIndex);
        }
        else
        {
            Debug.LogError("Invalid level index: " + levelIndex);
        }
    }

    // Progresses to the next level and updates PlayerPrefs
    public static void LevelProgressed()
    {
        int progress = GetCurrentLevelNumber();  // Retrieve the current progress
        progress++;
        PlayerPrefs.SetInt(PlayerPrefsManager.LevelProgress, progress);
        PlayerPrefs.Save();  // Save changes
    }

    // Loads the next level based on progress or selects a random level if all have been completed
    public static void LoadLevel()
    {
        int progress = GetCurrentLevelNumber();
        int totalLevels = SceneManager.sceneCountInBuildSettings;

        if (progress >= totalLevels)  // All levels completed, select a random unique level
        {
            int randomLevel = GetUniqueRandomLevel();
            LoadLevel(randomLevel);
        }
        else  // Load the next sequential level
        {
            LoadLevel(progress);
        }
    }

    // Generates a unique random level that hasn't been loaded recently
    private static int GetUniqueRandomLevel()
    {
        // Retrieve loaded levels from PlayerPrefs
        string loadedLevelsStr = PlayerPrefs.GetString(PlayerPrefsManager.LoadedLevels, "");
        if (!string.IsNullOrEmpty(loadedLevelsStr))
        {
            loadedLevels = new List<int>(Array.ConvertAll(loadedLevelsStr.Split(','), int.Parse));
        }

        int totalLevels = SceneManager.sceneCountInBuildSettings;
        List<int> availableLevels = new List<int>();

        // Collect levels from 10 onwards (assuming first levels are mandatory or tutorial)
        for (int i = 10; i < totalLevels; i++)
        {
            if (!loadedLevels.Contains(i))
            {
                availableLevels.Add(i);
            }
        }

        // If all levels have been played, reset the list
        if (availableLevels.Count == 0)
        {
            loadedLevels.Clear();
            for (int i = 10; i < totalLevels; i++)
            {
                availableLevels.Add(i);
            }
        }

        // Choose a random level from available options
        int randomIndex = UnityEngine.Random.Range(0, availableLevels.Count);
        int randomLevel = availableLevels[randomIndex];
        loadedLevels.Add(randomLevel);
        PlayerPrefs.SetString(PlayerPrefsManager.LoadedLevels, string.Join(",", loadedLevels));
        PlayerPrefs.Save();  // Save updated list

        return randomLevel;
    }
}
