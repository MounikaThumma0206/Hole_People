using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    public GameObject levelCompleteMenu;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    public void LevelComplete()
    {
        DOVirtual.DelayedCall(2f, () =>
        {
            if (levelCompleteMenu != null)
                levelCompleteMenu.SetActive(true);
        });
    }
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

