using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    public GameObject levelCompleteMenu;
    public GameObject retryPanel; // Reference to the retry panel
    private int moveCount = 5; // Start with 5 moves
    public TextMeshProUGUI moveText; // Reference to the UI Text component for showing remaining moves

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateMoveText(); // Initial update to show the starting number of moves
    }

    // Method to reduce the moves
    public void DecreaseMoveCount()
    {
        moveCount--; // Decrease move count on each action
        UpdateMoveText(); // Update the text with remaining moves

        if (moveCount <= 0) // Show retry panel when moves are 0
        {
            ShowRetryPanel();
        }
    }

    // Method to update the move text
    private void UpdateMoveText()
    {
        if (moveText != null)
        {
            moveText.text = "Moves: " + moveCount.ToString();
        }
    }

    // Method to show the retry panel
    private void ShowRetryPanel()
    {
        if (retryPanel != null)
        {
            retryPanel.SetActive(true);
        }
    }

    // Level complete method
    public void LevelComplete()
    {
        DOVirtual.DelayedCall(2f, () =>
        {
            if (levelCompleteMenu != null)
                levelCompleteMenu.SetActive(true);
        });
    }

    // OnContinueButtonClicked is already fine
    public void OnContinueButtonClicked()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels available. Returning to the main menu or restarting.");
            SceneManager.LoadScene(0);
        }
    }
}


//using DG.Tweening;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class UiManager : MonoBehaviour
//{
//    public static UiManager instance;
//    public GameObject levelCompleteMenu;
//    private void Awake()
//    {
//        if (instance == null)
//            instance = this;
//        else
//            Destroy(gameObject);
//    }
//    public void LevelComplete()
//    {
//        DOVirtual.DelayedCall(2f, () =>
//        {
//            if (levelCompleteMenu != null)
//                levelCompleteMenu.SetActive(true);
//        });
//    }
//    public void OnContinueButtonClicked()
//    {
//        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
//        int nextSceneIndex = currentSceneIndex + 1;

//        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
//        {
//            SceneManager.LoadScene(nextSceneIndex);
//        }
//        else
//        {
//            Debug.Log("No more levels available. Returning to the main menu or restarting.");
//            SceneManager.LoadScene(0);
//        }
//    }
//}

