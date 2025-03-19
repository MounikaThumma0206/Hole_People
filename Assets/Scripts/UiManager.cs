using DG.Tweening;
using UnityEngine;
using TMPro;

public class UiManager : MonoBehaviour
{
	public static UiManager instance;
	public GameObject levelCompleteMenu;
	public GameObject retryPanel; // Reference to the retry panel
	private int moveCount = 5; // Start with 5 moves
	public TextMeshProUGUI moveText; // Reference to the UI Text component for showing remaining moves
	public TextMeshProUGUI levelText; // Reference to the UI Text component for showing remaining moves
	public TextMeshProUGUI levelTextInCompletePanel; // Reference to the UI Text component for showing remaining moves

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	private void Start()
	{
		UpdateLevelText();
	}

	public void UpdateLevelText()
	{
		if (levelText != null)
		{
			levelText.text = "Level " + GameManager.Instance.GetLevel();
		}
	}
	// Method to update the move text
	public void UpdateMoveText(int moveCount)
	{
		if (moveText != null)
		{
			moveText.text = "Moves: " + moveCount.ToString();
		}
	}		
	public void RestartLevel()
	{
		GameManager.Instance.Restart();
	}
	// Level complete method
	public void LevelComplete()
	{
		DOVirtual.DelayedCall(2f, () =>
		{
			if (levelCompleteMenu != null)
			{
				levelTextInCompletePanel.text = "Level " + GameManager.Instance.GetLevel();
				levelCompleteMenu.SetActive(true);
				CrowdAudioManager.MakeHappyMood();

			}
		});
	}
	public void OnLoadNextLevel()
	{
		GameManager.Instance.LoadNext();
	}
	public void OnLoadPreviousLevel()
	{
		GameManager.Instance.LoadPrevious();
	}

	// OnContinueButtonClicked is already fine
	public void OnContinueButtonClicked()
	{
		GameManager.Instance.LoadNext();
	}

	internal void GameOver()
	{
		retryPanel.SetActive(true);
	}
}